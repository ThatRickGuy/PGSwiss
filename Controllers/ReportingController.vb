Imports System.Text

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

        Dim sbNotes As New StringBuilder
        sbNotes.Append(BaseController.Model.WMEvent.EventFormat.Name & " - " & BaseController.Model.WMEvent.Name & " - " & BaseController.Model.WMEvent.EventDate & ControlChars.CrLf)

        For Each r In BaseController.Model.WMEvent.Rounds
            Dim NonByeGames = From p In r.Games Where p.Player2 IsNot Nothing
            sbNotes.Append(r.Scenario & " " & r.Size & "pts " & (NonByeGames).Count & " games" & ControlChars.CrLf)
            sbNotes.Append(ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Assassination").Count & " Assassination" & ControlChars.CrLf)
            sbNotes.Append(ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Scenario").Count & " Scenario" & ControlChars.CrLf)
            sbNotes.Append(ControlChars.Tab & (From p In NonByeGames Where p.Condition = "Time").Count & " Time" & ControlChars.CrLf)
            'Get all the CP's for Player1 where they have an opponent (ie: no Byes!)
            Dim q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ControlPoints).ToList
            'Get all the CP's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ControlPoints)
            sbNotes.Append(ControlChars.Tab & q.Average.ToString("#.#") & " CP average" & ControlChars.CrLf)
            'Get all the APD's for Player1 where they have an opponent (ie: no Byes!)
            q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ArmyPointsDestroyed).ToList
            'Get all the APD's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ArmyPointsDestroyed)
            sbNotes.Append(ControlChars.Tab & q.Average.ToString("#.#") & " APD average" & ControlChars.CrLf)
        Next


        sbNotes.Append(ControlChars.CrLf & ControlChars.CrLf)
        sbNotes.AppendLine("Standings")
        sbNotes.AppendLine("RANK".PadLeft(5) & "NAME".PadRight(30) & "HANDLE".PadRight(30) & "FACTION".PadRight(30) & "META".PadRight(30) & _
                           "TP".PadLeft(4) & "SOS".PadLeft(4) & "CP".PadLeft(3) & "APD".PadLeft(5))
        For Each player In (From p In Model.CurrentRoundPlayers Order By p.Rank)
            sbNotes.AppendLine(player.Rank.ToString.PadLeft(5) & player.Name.PadRight(30) & player.PPHandle.PadRight(30) & player.Faction.PadRight(30) & player.Meta.PadRight(30) & _
                               player.TourneyPoints.ToString.PadLeft(4) & player.StrengthOfSchedule.ToString.PadLeft(4) & player.ControlPoints.ToString.PadLeft(3) & player.ArmyPointsDestroyed.ToString.PadLeft(5))
        Next

        Notes = sbNotes.ToString
        
    End Sub


    Public Property Attendees As String
    Public Property Notes As String
End Class
