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


    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty

        If BaseController.Model.WMEvent.Players.Count = 0 Then sReturn = "No players" & ControlChars.CrLf
        If BaseController.Model.WMEvent.Name = String.Empty Then sReturn &= "Event name not set" & ControlChars.CrLf
        If BaseController.Model.WMEvent.EventFormat = String.Empty Then sReturn &= "Event format not set" & ControlChars.CrLf
        If (From p In BaseController.Model.WMEvent.Players Where p.PPHandle = String.Empty Or p.Name = String.Empty).Count > 0 Then sReturn &= "Players without PPHandle, Name, Faction, and/or Meta"

        Return sReturn.TrimEnd(ControlChars.CrLf)
    End Function
End Class
