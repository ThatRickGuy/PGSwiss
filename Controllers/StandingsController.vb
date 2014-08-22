Imports PGSwiss.Data

Public Class StandingsController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New ReportingController
    End Function

    Public Sub New()
        Me.View = New Standings

        For Each Player In Model.CurrentRound.Players
            Player.StrengthOfSchedule = 0
            For Each Opponent In Player.Opponents
                Player.StrengthOfSchedule += (From p In Model.CurrentRound.Players Where p.PPHandle = Opponent).FirstOrDefault.TourneyPoints
            Next
        Next

        Dim Standings As New doStandings
        Standings.Standings.AddRange((From p In Model.CurrentRound.Players Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending).ToList)
        Dim i As Integer = 1
        For Each s In Standings.Standings
            s.Rank = i
            i += 1
        Next
        Me.View.DataContext = Standings
    End Sub

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 90

        For Each Player In Model.CurrentRound.Players
            Player.StrengthOfSchedule = 0
            For Each Opponent In Player.Opponents
                Player.StrengthOfSchedule += (From p In Model.CurrentRound.Players Where p.PPHandle = Opponent).FirstOrDefault.TourneyPoints
            Next
        Next
    End Sub
End Class
