Imports System.Net.Http
Imports System.Web.Http
Imports Newtonsoft.Json.Serialization

Public Module WebApiConfig
    Public Sub Register(config As HttpConfiguration)
      ' Web API configuration and services
      ' Configure Web API to use only bearer token authentication.
      config.SuppressDefaultHostAuthentication()
      config.Filters.Add(New HostAuthenticationFilter(Startup.OAuthOptions.AuthenticationType))

      ' Web API routes
      config.MapHttpAttributeRoutes()

      config.Routes.MapHttpRoute(
          name:="DefaultApi",
          routeTemplate:="api/{controller}/{id}",
          defaults:=New With { .id = RouteParameter.Optional }
      )
    End Sub
End Module
