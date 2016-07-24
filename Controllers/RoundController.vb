Imports PGSwiss.Data

Public Class RoundController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New PairingsController
    End Function

    Private _Round As doRound

    Public Sub New()
        Me.View = New Round
        Dim TargetRoundNumber As Integer = 1
        If Not Model.CurrentRound Is Nothing Then TargetRoundNumber = Model.CurrentRound.RoundNumber + 1
        _Round = (From p In Model.WMEvent.Rounds Where p.RoundNumber = TargetRoundNumber).FirstOrDefault

        If _Round Is Nothing Then
            _Round = New doRound
            _Round.RoundNumber = TargetRoundNumber
            Model.WMEvent.Rounds.Add(_Round)
        End If

        If Model.CurrentRound IsNot Nothing Then _Round.Size = Model.CurrentRound.Size

        Model.CurrentRound = _Round

        Me.View.DataContext = Me

    End Sub

    Protected Overrides Sub Activated()

        Model.CurrentRound = _Round

        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 0) 'current round + the screen of the round

        If (From p In Model.CurrentRound.Games Where p.Reported).Count > 0 Then CType(Me.View, Round).dgPlayers.CanUserAddRows = False
        CType(Me.View, Round).Refresh()


        Dim PreviousRound = (From p In Model.WMEvent.Rounds Where p.RoundNumber = (Model.CurrentRound.RoundNumber - 1) Select p).FirstOrDefault
        If PreviousRound IsNot Nothing Then
            For Each player In PreviousRound.Games.DropNextRound
                Dim dropPlayer = (From p In Model.CurrentRoundPlayers Where p.Name = player Select p).FirstOrDefault
                If dropPlayer IsNot Nothing Then dropPlayer.Drop = True
            Next
        End If

    End Sub


    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If BaseController.Model.CurrentRound.Scenario = String.Empty Then sReturn = "Scenario not selected" & ControlChars.CrLf
        If BaseController.Model.CurrentRound.Size = 0 Then sReturn &= "Size not selected"

        Dim CurrentRoundPlayers = Model.CurrentRoundPlayers
        If CurrentRoundPlayers.Count = 0 Then sReturn = "No players" & ControlChars.CrLf
        If (From p In CurrentRoundPlayers Where p.Name = String.Empty).Count > 0 Then sReturn &= "Players without Name, Name, Faction, and/or Meta" & ControlChars.CrLf
        If (From p In CurrentRoundPlayers Select p.Name Distinct).Count < CurrentRoundPlayers.Count Then sReturn &= "Players must have unique Names."
        Model.CurrentRound.ByeVolunteers.AddRange(From p In CurrentRoundPlayers Where p.ByeVol)

        Return sReturn.TrimEnd(ControlChars.CrLf)
    End Function

End Class
