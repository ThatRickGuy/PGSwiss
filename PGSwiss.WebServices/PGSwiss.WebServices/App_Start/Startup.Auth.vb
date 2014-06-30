Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OAuth
Imports Owin

Public Partial Class Startup
    Private Shared _oAuthOptions As OAuthAuthorizationServerOptions
    Private Shared _publicClientId As String

    Shared Sub New()
      PublicClientId = "self"

      UserManagerFactory = Function() New UserManager(Of IdentityUser)(New UserStore(Of IdentityUser))

      OAuthOptions = New OAuthAuthorizationServerOptions() With {
          .TokenEndpointPath = New PathString("/Token"),
          .Provider = New ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
          .AuthorizeEndpointPath = New PathString("/api/Account/ExternalLogin"),
          .AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
          .AllowInsecureHttp = True
      }
    End Sub

    Public Shared Property OAuthOptions() As OAuthAuthorizationServerOptions
        Get
            Return _oAuthOptions
        End Get
        Private Set
            _oAuthOptions = Value
        End Set
    End Property

    Public Shared Property PublicClientId() As String
        Get
            Return _publicClientId
        End Get
        Private Set
            _publicClientId = Value
        End Set
    End Property

    Public Shared Property UserManagerFactory As Func(Of UserManager(Of IdentityUser))

    ' For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
    Public Sub ConfigureAuth(app As IAppBuilder)
        ' Enable the application to use a cookie to store information for the signed in user
        ' and to use a cookie to temporarily store information about a user logging in with a third party login provider
        app.UseCookieAuthentication(New CookieAuthenticationOptions)
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie)

        ' Enable the application to use bearer tokens to authenticate users
        app.UseOAuthBearerTokens(OAuthOptions)

        ' Uncomment the following lines to enable logging in with third party login providers
        'app.UseMicrosoftAccountAuthentication(
        '    clientId := "",
        '    clientSecret := "")

        'app.UseTwitterAuthentication(
        '    consumerKey := "",
        '    consumerSecret := "")

        'app.UseFacebookAuthentication(
        '    appId := "",
        '    appSecret := "")

        'app.UseGoogleAuthentication()
    End Sub
End Class
