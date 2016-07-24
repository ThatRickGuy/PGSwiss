Imports System.ComponentModel

Public Class WMEventController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New RoundController
    End Function

    Public Sub New()
        _Stack.Add(Me)
        _PreviousEnabled = False
        Me.View = New WMEvent
        Me.View.DataContext = Me
    End Sub

    Public ReadOnly Property PlayerCount As Integer
        Get
            Return BaseController.Model.WMEvent.Players.Count
        End Get
    End Property

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty

        If BaseController.Model.WMEvent.Players.Count = 0 Then sReturn = "No players" & ControlChars.CrLf
        If BaseController.Model.WMEvent.Name = String.Empty Then sReturn &= "Event name not set" & ControlChars.CrLf
        If BaseController.Model.WMEvent.EventFormat Is Nothing Then sReturn &= "Event format not set" & ControlChars.CrLf
        If (From p In BaseController.Model.WMEvent.Players Where p.Name = String.Empty).Count > 0 Then sReturn &= "Players without Name, Name, Faction, and/or Meta" & ControlChars.CrLf
        If (From p In BaseController.Model.WMEvent.Players Select p.Name Distinct).Count < BaseController.Model.WMEvent.Players.Count Then sReturn &= "Players must have unique Names."

        Return sReturn.TrimEnd(ControlChars.CrLf)
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 10
    End Sub

End Class
