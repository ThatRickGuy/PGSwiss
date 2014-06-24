Public Class ReportingController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return Nothing
    End Function

    Public Sub New()
        _NextEnabled = False
        Me.View = New Reporting
        Me.View.DataContext = Me
    End Sub

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 100
    End Sub

    Private Sub ReportingController_ActivationCompleted(sender As Object, e As EventArgs) Handles Me.ActivationCompleted
        Attendees = String.Empty
        For Each p In BaseController.Model.WMEvent.Players
            Attendees &= p.Name & "(" & p.PPHandle & "), "
        Next
        If Attendees <> String.Empty Then Attendees = Attendees.Substring(0, Attendees.Length - 2)

        Notes = BaseController.Model.WMEvent.EventFormat & " - " & BaseController.Model.WMEvent.Name & " - " & BaseController.Model.WMEvent.EventDate & ControlChars.CrLf

        For Each r In BaseController.Model.WMEvent.Rounds
            Dim NonByeGames = From p In r.Games Where p.Player2 IsNot Nothing
            Notes &= r.Scenario & " " & r.Size & "pts " & (NonByeGames).Count & " games" & ControlChars.CrLf
            Notes &= ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Assassination").Count & " Assassination" & ControlChars.CrLf
            Notes &= ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Scenario").Count & " Scenario" & ControlChars.CrLf
            Notes &= ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Time").Count & " Time" & ControlChars.CrLf
            'Get all the CP's for Player1 where they have an opponent (ie: no Byes!)
            Dim q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ControlPoints).ToList
            'Get all the CP's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ControlPoints)
            Notes &= ControlChars.Tab & q.Average.ToString("#.#") & " CP average" & ControlChars.CrLf
            'Get all the APD's for Player1 where they have an opponent (ie: no Byes!)
            q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ArmyPointsDestroyed).ToList
            'Get all the APD's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ArmyPointsDestroyed)
            Notes &= ControlChars.Tab & q.Average.ToString("#.#") & " APD average" & ControlChars.CrLf
        Next
    End Sub


    Public Property Attendees As String
    Public Property Notes As String
End Class
