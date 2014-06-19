Public Class RoundController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New PairingsController
    End Function

    Public Sub New()
        Me.View = New Round
        Dim TargetRoundNumber As Integer = 1
        If Not Model.CurrentRound Is Nothing Then TargetRoundNumber = Model.CurrentRound.RoundNumber + 1
        Dim round = (From p In Model.WMEvent.Rounds Where p.RoundNumber = TargetRoundNumber).FirstOrDefault

        If round Is Nothing Then
            round = New doRound
            round.RoundNumber = TargetRoundNumber
            If round.RoundNumber = 1 Then
                round.Players.AddRange(Model.WMEvent.Players)
            Else
                round.Players.AddRange(Model.WMEvent.Rounds.Last.Players)
            End If
            Model.WMEvent.Rounds.Add(round)
        End If

        Model.CurrentRound = round

        Me._Title = "Round " & Model.CurrentRound.RoundNumber



        Me.View.DataContext = Me
    End Sub

    Protected Overrides Sub Activated()

        Dim tempPlayers As New doPlayerCollection
        tempPlayers.AddRange(BaseController.Model.CurrentRound.Players.ToArray)

        Dim PlayersToAdd As New List(Of doPlayer)
        For Each p In BaseController.Model.WMEvent.Players
            If (From p1 In tempPlayers Where p1.PlayerID = p.PlayerID).Count = 0 Then PlayersToAdd.Add(p)
        Next
        tempPlayers.AddRange(PlayersToAdd)

        Dim PlayersToRemove As New List(Of doPlayer)
        For Each p In tempPlayers
            If (From p1 In BaseController.Model.WMEvent.Players Where p1.PlayerID = p.PlayerID).Count = 0 Then PlayersToRemove.Add(p)
        Next
        For Each p In PlayersToRemove
            tempPlayers.Remove(p)
        Next
        Model.CurrentRound.Players.Clear()
        Model.CurrentRound.Players.AddRange(tempPlayers)

        CType(Me.View, Round).dgPlayers.Items.Refresh()
    End Sub
End Class
