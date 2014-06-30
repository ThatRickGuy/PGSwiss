﻿Imports System
Imports System.Net.Http
Imports System.Net.Http.Headers

Imports System.Net
Imports Newtonsoft.Json
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Security
Imports System.Runtime.InteropServices

Public Class WebAPIHelper
    Private Shared SecurityToken As String = String.Empty
    Private Shared SecurityTokenExpires As Date = Now

    Private Shared _Password As SecureString
    Private Shared _Username As String


    Private Shared Async Function GenerateSecurityToken() As Task
        Dim AccountWindow As New AccountManagement
        AccountWindow.ShowDialog()
        _Username = AccountWindow.txtUserName.Text
        _Password = AccountWindow.txtPassword.SecurePassword

        Dim result = Await APIPOST("/token", String.Format("username={0}&password={1}&grant_type=password", _Username, ConvertToUnsecureString(_Password)), True)
        Dim json = JObject.Parse(result)
        SecurityToken = json("access_token").ToString()
        Dim s = json(".expires").ToString
        Date.TryParse(s, SecurityTokenExpires)
    End Function

    Public Shared Function APIPOST(target As String, content As String) As Task(Of String)
        Return APIPOST(target, content, False)
    End Function

    Private Shared Async Function APIPOST(target As String, content As String, SkipValidation As Boolean) As Task(Of String)
        If Not SkipValidation Then Await ValidateSecurityToken()

        Dim sReturn As String = String.Empty
        Using client As New HttpClient()
            client.BaseAddress = New Uri(My.Settings.EndPoint)
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " & SecurityToken)
            Dim response = Await client.PostAsync(target, New StringContent(content))
            If (response.IsSuccessStatusCode) Then sReturn = response.Content.ReadAsStringAsync.Result
        End Using
        Return sReturn
    End Function

    Public Shared Async Function APIGET(target As String) As Task(Of String)
        Await ValidateSecurityToken()

        Dim sReturn As String = String.Empty
        Using client As New HttpClient()
            client.BaseAddress = New Uri(My.Settings.EndPoint)
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " & SecurityToken)

            Dim response = Await client.GetAsync(target)
            If response.IsSuccessStatusCode Then
                Dim values = Await response.Content.ReadAsStreamAsync()
                Using reader = New StreamReader(values)
                    sReturn = reader.ReadToEnd
                End Using
            End If
        End Using

        Return sReturn
    End Function

    Private Shared Async Function ValidateSecurityToken() As Task
        Dim attempt As Integer = 0
        While SecurityToken = String.Empty OrElse SecurityTokenExpires < Now
            If attempt > 2 Then Throw New Exception("Too many login attempts!")
            attempt += 1
            Dim t = GenerateSecurityToken()
            Await t
        End While
    End Function

    Private Shared Function ConvertToUnsecureString(password As SecureString) As String
        Dim sReturn As String = String.Empty
        If password Is Nothing Then Throw New ArgumentNullException("securePassword")
        Dim unmanagedString = IntPtr.Zero
        Try
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(password)
            sReturn = Marshal.PtrToStringUni(unmanagedString)
        Catch
        Finally
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString)
        End Try

        Return sReturn
    End Function
End Class
