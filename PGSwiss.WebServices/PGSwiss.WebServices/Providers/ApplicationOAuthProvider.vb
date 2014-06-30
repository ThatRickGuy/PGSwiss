Imports System.Security.Claims
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.WebPages
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OAuth

Public Class ApplicationOAuthProvider
    Inherits OAuthAuthorizationServerProvider
    Private ReadOnly _publicClientId As String
    Private ReadOnly _userManagerFactory As Func(Of UserManager(Of IdentityUser))

    Public Sub New(publicClientId As String, userManagerFactory As Func(Of UserManager(Of IdentityUser)))
        If publicClientId Is Nothing Then
            Throw New ArgumentNullException("publicClientId")
        End If

        If userManagerFactory Is Nothing Then
            Throw New ArgumentNullException("userManagerFactory")
        End If

        _publicClientId = publicClientId
        _userManagerFactory = userManagerFactory
    End Sub

    Public Overrides Async Function GrantResourceOwnerCredentials(context As OAuthGrantResourceOwnerCredentialsContext) As Task
        Using userManager As UserManager(Of IdentityUser) = _userManagerFactory()
            Dim user As IdentityUser = Await userManager.FindAsync(context.UserName, context.Password)

            If user Is Nothing Then
                context.SetError("invalid_grant", "The user name or password is incorrect.")
                Return
            End If

            Dim oAuthIdentity As ClaimsIdentity = Await userManager.CreateIdentityAsync(user, context.Options.AuthenticationType)
            Dim cookiesIdentity As ClaimsIdentity = Await userManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType)
            Dim properties As AuthenticationProperties = CreateProperties(user.UserName)
            Dim ticket As New AuthenticationTicket(oAuthIdentity, properties)
            context.Validated(ticket)
            context.Request.Context.Authentication.SignIn(cookiesIdentity)
        End Using
    End Function

    Public Overrides Function TokenEndpoint(context As OAuthTokenEndpointContext) As Task
        For Each [property] As KeyValuePair(Of String, String) In context.Properties.Dictionary
            context.AdditionalResponseParameters.Add([property].Key, [property].Value)
        Next

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientAuthentication(context As OAuthValidateClientAuthenticationContext) As Task
        ' Resource owner password credentials does not provide a client ID.
        If context.ClientId Is Nothing Then
            context.Validated()
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientRedirectUri(context As OAuthValidateClientRedirectUriContext) As Task
        If context.ClientId = _publicClientId Then
            Dim expectedRootUri As Uri = New Uri(context.Request.Uri, "/")
            If expectedRootUri.AbsoluteUri = context.RedirectUri Then
                context.Validated()
            End If
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Shared Function CreateProperties(userName As String) As AuthenticationProperties
        Dim data As IDictionary(Of String, String) = New Dictionary(Of String, String)() From {
            {"userName", userName}
        }
        Return New AuthenticationProperties(data)
    End Function
End Class
