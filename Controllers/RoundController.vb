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
            If _Round.RoundNumber = 1 Then
                _Round.Players.AddRange(Model.WMEvent.Players)
            Else
                _Round.Players.AddRange(Model.WMEvent.Rounds.Last.Players)
            End If
            Model.WMEvent.Rounds.Add(_Round)
        End If

        _Round.Players.ForEach(Sub(s As doPlayer) s.ByeVol = False)

        If Model.CurrentRound IsNot Nothing Then _Round.Size = Model.CurrentRound.Size

        Model.CurrentRound = _Round

        Me.View.DataContext = Me

    End Sub

    Protected Overrides Sub Activated()

        Model.CurrentRound = _Round

        Dim tempPlayers As New doPlayerCollection
        tempPlayers.AddRange(BaseController.Model.CurrentRound.Players.ToArray)

        Dim PlayersToAdd As New List(Of doPlayer)
        For Each p In BaseController.Model.WMEvent.Players
            If (From p1 In tempPlayers Where p1.PPHandle = p.PPHandle).Count = 0 Then PlayersToAdd.Add(p)
        Next
        tempPlayers.AddRange(PlayersToAdd)

        Dim PlayersToRemove As New List(Of doPlayer)
        For Each p In tempPlayers
            If (From p1 In BaseController.Model.WMEvent.Players Where p1.PPHandle = p.PPHandle).Count = 0 Then PlayersToRemove.Add(p)
        Next
        For Each p In PlayersToRemove
            tempPlayers.Remove(p)
        Next
        Model.CurrentRound.Players.Clear()
        Model.CurrentRound.Players.AddRange(tempPlayers)


        For Each Player In Model.CurrentRound.Players
            Player.StrengthOfSchedule = 0
            For Each Opponent In Player.Oppontnents
                Player.StrengthOfSchedule += (From p In Model.CurrentRound.Players Where p.PPHandle = Opponent).FirstOrDefault.TourneyPoints
            Next
        Next

        Dim temp = (From p In Model.CurrentRound.Players Order By p.TourneyPoints Descending,
                                                                p.StrengthOfSchedule Descending,
                                                                p.ControlPoints Descending,
                                                                p.ArmyPointsDestroyed Descending).ToList
        Model.CurrentRound.Players.Clear()
        Model.CurrentRound.Players.AddRange(temp)
        Dim i As Integer = 1
        For Each p In Model.CurrentRound.Players
            p.Rank = i
            i += 1
        Next


        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 0) 'current round + the screen of the round

        If (From p In Model.CurrentRound.Games Where p.Reported).Count > 0 Then CType(Me.View, Round).dgPlayers.CanUserAddRows = False
        CType(Me.View, Round).dgPlayers.Items.Refresh()
    End Sub

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If BaseController.Model.CurrentRound.Scenario = String.Empty Then sReturn = "Scenario not selected" & ControlChars.CrLf
        If BaseController.Model.CurrentRound.Size = 0 Then sReturn &= "Size not selected"
        Return sReturn.TrimEnd(ControlChars.CrLf)
    End Function
End Class
