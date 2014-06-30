Imports System.Net.Http
Imports System.Security.Claims
Imports System.Security.Cryptography
Imports System.Threading.Tasks
Imports System.Web.Http
Imports System.Web.Http.ModelBinding
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OAuth

<Authorize>
<RoutePrefix("api/Account")>
Public Class AccountController
    Inherits ApiController
    Private Const LocalLoginProvider As String = "Local"
    Private _userManager As UserManager(Of IdentityUser)
    Private _accessTokenFormat As ISecureDataFormat(Of AuthenticationTicket)

    Public Sub New()
        Me.New(Startup.UserManagerFactory.Invoke(), Startup.OAuthOptions.AccessTokenFormat)
    End Sub

    Public Sub New(userManager As UserManager(Of IdentityUser), accessTokenFormat As ISecureDataFormat(Of AuthenticationTicket))
        Me.UserManager = userManager
        Me.AccessTokenFormat = accessTokenFormat
    End Sub

    Public Property UserManager() As UserManager(Of IdentityUser)
        Get
            Return _userManager
        End Get
        Private Set(value As UserManager(Of IdentityUser))
            _userManager = Value
        End Set
    End Property

    Public Property AccessTokenFormat() As ISecureDataFormat(Of AuthenticationTicket)
        Get
            Return _accessTokenFormat
        End Get
        Private Set(value As ISecureDataFormat(Of AuthenticationTicket))
            _accessTokenFormat = Value
        End Set
    End Property

    ' GET api/Account/UserInfo
    <HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)>
    <Route("UserInfo")>
    Public Function GetUserInfo() As UserInfoViewModel
        Dim externalLogin As ExternalLoginData = ExternalLoginData.FromIdentity(TryCast(User.Identity, ClaimsIdentity))

        Return New UserInfoViewModel() With {
            .UserName = User.Identity.GetUserName(),
            .HasRegistered = externalLogin Is Nothing,
            .LoginProvider = If(externalLogin IsNot Nothing, externalLogin.LoginProvider, Nothing)
        }
    End Function

    ' POST api/Account/Logout
    <Route("Logout")>
    Public Function Logout() As IHttpActionResult
        Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType)
        Return Ok()
    End Function

    ' GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
    <Route("ManageInfo")>
    Public Async Function GetManageInfo(returnUrl As String, Optional generateState As Boolean = False) As Task(Of ManageInfoViewModel)
        Dim userInfo As IdentityUser = Await UserManager.FindByIdAsync(User.Identity.GetUserId())

        If userInfo Is Nothing Then
            Return Nothing
        End If

        Dim logins As New List(Of UserLoginInfoViewModel)()

        For Each linkedAccount As IdentityUserLogin In userInfo.Logins
            logins.Add(New UserLoginInfoViewModel() With {
                .LoginProvider = linkedAccount.LoginProvider,
                .ProviderKey = linkedAccount.ProviderKey
            })
        Next

        If userInfo.PasswordHash IsNot Nothing Then
            logins.Add(New UserLoginInfoViewModel() With {
                .LoginProvider = LocalLoginProvider,
                .ProviderKey = userInfo.UserName
            })
        End If

        Return New ManageInfoViewModel() With {
            .LocalLoginProvider = LocalLoginProvider,
            .UserName = userInfo.UserName,
            .Logins = logins,
            .ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        }
    End Function

    ' POST api/Account/ChangePassword
    <Route("ChangePassword")>
    Public Async Function ChangePassword(model As ChangePasswordBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Dim result As IdentityResult = Await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword)
        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    ' POST api/Account/SetPassword
    <Route("SetPassword")>
    Public Async Function SetPassword(model As SetPasswordBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Dim result As IdentityResult = Await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword)
        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    ' POST api/Account/AddExternalLogin
    <Route("AddExternalLogin")>
    Public Async Function AddExternalLogin(model As AddExternalLoginBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie)

        Dim ticket As AuthenticationTicket = AccessTokenFormat.Unprotect(model.ExternalAccessToken)

        If ticket Is Nothing OrElse ticket.Identity Is Nothing OrElse (ticket.Properties IsNot Nothing AndAlso ticket.Properties.ExpiresUtc.HasValue AndAlso ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow) Then
            Return BadRequest("External login failure.")
        End If

        Dim externalData As ExternalLoginData = ExternalLoginData.FromIdentity(ticket.Identity)

        If externalData Is Nothing Then
            Return BadRequest("The external login is already associated with an account.")
        End If

        Dim result As IdentityResult = Await UserManager.AddLoginAsync(User.Identity.GetUserId(), New UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey))
        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    ' POST api/Account/RemoveLogin
    <Route("RemoveLogin")>
    Public Async Function RemoveLogin(model As RemoveLoginBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Dim result As IdentityResult

        If model.LoginProvider = LocalLoginProvider Then
            result = Await UserManager.RemovePasswordAsync(User.Identity.GetUserId())
        Else
            result = Await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), New UserLoginInfo(model.LoginProvider, model.ProviderKey))
        End If

        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    ' GET api/Account/ExternalLogin
    <OverrideAuthentication>
    <HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)>
    <AllowAnonymous>
    <Route("ExternalLogin", Name:="ExternalLogin")>
    Public Async Function GetExternalLogin(provider As String, Optional [error] As String = Nothing) As Task(Of IHttpActionResult)
        If [error] IsNot Nothing Then
            Return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString([error]))
        End If

        If Not User.Identity.IsAuthenticated Then
            Return New ChallengeResult(provider, Me)
        End If

        Dim externalLogin As ExternalLoginData = ExternalLoginData.FromIdentity(TryCast(User.Identity, ClaimsIdentity))

        If externalLogin Is Nothing Then
            Return InternalServerError()
        End If

        If externalLogin.LoginProvider <> provider Then
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie)
            Return New ChallengeResult(provider, Me)
        End If

        Dim userInfo As IdentityUser = Await UserManager.FindAsync(New UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey))

        Dim hasRegistered As Boolean = userInfo IsNot Nothing

        If hasRegistered Then
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie)
            Dim oAuthIdentity As ClaimsIdentity = Await UserManager.CreateIdentityAsync(userInfo, OAuthDefaults.AuthenticationType)
            Dim cookieIdentity As ClaimsIdentity = Await UserManager.CreateIdentityAsync(userInfo, CookieAuthenticationDefaults.AuthenticationType)
            Dim properties As AuthenticationProperties = ApplicationOAuthProvider.CreateProperties(userInfo.UserName)
            Authentication.SignIn(properties, oAuthIdentity, cookieIdentity)
        Else
            Dim claims As IEnumerable(Of Claim) = externalLogin.GetClaims()
            Dim identity As ClaimsIdentity = New ClaimsIdentity(claims, OAuthDefaults.AuthenticationType)
            Authentication.SignIn(identity)
        End If

        Return Ok()
    End Function

    ' GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
    <AllowAnonymous>
    <Route("ExternalLogins")>
    Public Function GetExternalLogins(returnUrl As String, Optional generateState As Boolean = False) As IEnumerable(Of ExternalLoginViewModel)
        Dim descriptions As IEnumerable(Of AuthenticationDescription) = Authentication.GetExternalAuthenticationTypes()
        Dim logins As New List(Of ExternalLoginViewModel)()

        Dim state As String

        If generateState Then
            Const strengthInBits As Integer = 256
            state = RandomOAuthStateGenerator.Generate(strengthInBits)
        Else
            state = Nothing
        End If

        For Each description As AuthenticationDescription In descriptions
            Dim login As New ExternalLoginViewModel() With {
                .Name = description.Caption,
                .Url = Url.Route("ExternalLogin", New With {
                    .provider = description.AuthenticationType,
                    .response_type = "token",
                    .client_id = Startup.PublicClientId,
                    .redirect_uri = New Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                    .state = state
              }),
                .State = state
            }
            logins.Add(login)
        Next

        Return logins
    End Function

    ' POST api/Account/Register
    <AllowAnonymous>
    <Route("Register")>
    Public Async Function Register(model As RegisterBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Dim userInfo As IdentityUser = New IdentityUser With {
            .UserName = model.UserName
        }

        Dim result As IdentityResult = Await UserManager.CreateAsync(userInfo, model.Password)
        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    ' POST api/Account/RegisterExternal
    <OverrideAuthentication>
    <HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)>
    <Route("RegisterExternal")>
    Public Async Function RegisterExternal(model As RegisterExternalBindingModel) As Task(Of IHttpActionResult)
        If Not ModelState.IsValid Then
            Return BadRequest(ModelState)
        End If

        Dim externalLogin As ExternalLoginData = ExternalLoginData.FromIdentity(TryCast(User.Identity, ClaimsIdentity))

        If externalLogin Is Nothing Then
            Return InternalServerError()
        End If

        Dim userInfo As IdentityUser = New IdentityUser With {
            .UserName = model.UserName
        }
        userInfo.Logins.Add(New IdentityUserLogin With {
            .LoginProvider = externalLogin.LoginProvider,
            .ProviderKey = externalLogin.ProviderKey
        })
        Dim result As IdentityResult = Await UserManager.CreateAsync(userInfo)
        Dim errorResult As IHttpActionResult = GetErrorResult(result)

        If errorResult IsNot Nothing Then
            Return errorResult
        End If

        Return Ok()
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            UserManager.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

#Region "Helpers"

    Private ReadOnly Property Authentication() As IAuthenticationManager
        Get
            Return Request.GetOwinContext().Authentication
        End Get
    End Property

    Private Function GetErrorResult(result As IdentityResult) As IHttpActionResult
        If result Is Nothing Then
            Return InternalServerError()
        End If

        If Not result.Succeeded Then
            If result.Errors IsNot Nothing Then
                For Each [error] As String In result.Errors
                    ModelState.AddModelError("", [error])
                Next
            End If

            If ModelState.IsValid Then
                ' No ModelState errors are available to send, so just return an empty BadRequest.
                Return BadRequest()
            End If

            Return BadRequest(ModelState)
        End If

        Return Nothing
    End Function

    Private Class ExternalLoginData
        Public Property LoginProvider As String
        Public Property ProviderKey As String
        Public Property UserName As String

        Public Function GetClaims() As IList(Of Claim)
            Dim claims As IList(Of Claim) = New List(Of Claim)()
            claims.Add(New Claim(ClaimTypes.NameIdentifier, ProviderKey, Nothing, LoginProvider))

            If UserName IsNot Nothing Then
                claims.Add(New Claim(ClaimTypes.Name, UserName, Nothing, LoginProvider))
            End If

            Return claims
        End Function

        Public Shared Function FromIdentity(identity As ClaimsIdentity) As ExternalLoginData
            If identity Is Nothing Then
                Return Nothing
            End If

            Dim providerKeyClaim As Claim = identity.FindFirst(ClaimTypes.NameIdentifier)

            If providerKeyClaim Is Nothing OrElse String.IsNullOrEmpty(providerKeyClaim.Issuer) OrElse String.IsNullOrEmpty(providerKeyClaim.Value) Then
                Return Nothing
            End If

            If providerKeyClaim.Issuer = ClaimsIdentity.DefaultIssuer Then
                Return Nothing
            End If

            Return New ExternalLoginData() With {
                .LoginProvider = providerKeyClaim.Issuer,
                .ProviderKey = providerKeyClaim.Value,
                .UserName = identity.FindFirstValue(ClaimTypes.Name)
            }
        End Function
    End Class

    Private NotInheritable Class RandomOAuthStateGenerator
        Private Shared _random As RandomNumberGenerator = New RNGCryptoServiceProvider()

        Public Shared Function Generate(strengthInBits As Integer) As String
            Const bitsPerByte As Integer = 8

            If strengthInBits Mod bitsPerByte <> 0 Then
                Throw New ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits")
            End If

            Dim strengthInBytes As Integer = strengthInBits \ bitsPerByte

            Dim data As Byte() = New Byte(strengthInBytes - 1) {}
            _random.GetBytes(data)
            Return HttpServerUtility.UrlTokenEncode(data)
        End Function
    End Class
#End Region
End Class
