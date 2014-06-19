﻿Public Class GamesController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Dim bcReturn As BaseController

        If Model.CurrentRound.IsLastRound Then
            bcReturn = New StandingsController
        Else
            bcReturn = New RoundController
        End If
        Return bcReturn
    End Function

    Public Sub New()
        Me.View = New Games
        Me._Title = "Round " & BaseController.Model.CurrentRound.RoundNumber & " - Games"
        BaseController.Model.CurrentGame = BaseController.Model.CurrentRound.Games.FirstOrDefault
        Me.View.DataContext = Me
    End Sub

    Public Sub SelectGame(Game As doGame)
        Model.CurrentGame = Game
    End Sub

    Public Sub AcceptGame()
        If BaseController.Model.CurrentGame IsNot Nothing Then
            Dim Player1 = Model.CurrentGame.Player1
            Dim Player2 = Model.CurrentGame.Player2

            Dim Player1FromRound = GetPlayerFromRound(Player1)
            Dim Player1FromLastRound = GetPlayerFromLastRound(Player1)
            Dim Player2FromRound = GetPlayerFromRound(Player2)
            Dim Player2FromLastRound = GetPlayerFromLastRound(Player2)

            If Player1FromLastRound Is Nothing Then
                If Model.CurrentGame.Winner = "Player 1" Then Player1FromRound.TourneyPoints = 1
                Player1FromRound.ControlPoints = Player1.ControlPoints
                Player1FromRound.ArmyPointsDestroyed = Player1.ArmyPointsDestroyed
            Else
                If Model.CurrentGame.Winner = "Player 1" Then Player1FromRound.TourneyPoints = Player1FromLastRound.TourneyPoints + 1
                Player1FromRound.ControlPoints = Player1.ControlPoints + Player1FromLastRound.ControlPoints
                Player1FromRound.ArmyPointsDestroyed = Player1.ArmyPointsDestroyed + Player1FromLastRound.ArmyPointsDestroyed
            End If

            If Not Player2 Is Nothing Then
                If Player2FromLastRound Is Nothing Then
                    If Model.CurrentGame.Winner = "Player 2" Then Player2FromRound.TourneyPoints = 1
                    Player2FromRound.ControlPoints = Player2.ControlPoints
                    Player2FromRound.ArmyPointsDestroyed = Player2.ArmyPointsDestroyed
                Else
                    If Model.CurrentGame.Winner = "Player 2" Then Player2FromRound.TourneyPoints = Player2FromLastRound.TourneyPoints + 1
                    Player2FromRound.ControlPoints = Player2.ControlPoints + Player2FromLastRound.ControlPoints
                    Player2FromRound.ArmyPointsDestroyed = Player2.ArmyPointsDestroyed + Player2FromLastRound.ArmyPointsDestroyed
                End If

                Player1FromRound.Oppontnents.Add(Player2FromRound.PlayerID)
                Player2FromRound.Oppontnents.Add(Player1FromRound.PlayerID)
            End If

            Model.CurrentGame.Reported = True
            Model.Save()

            Dim SortedGames = (From p In Model.CurrentRound.Games Order By p.Reported, p.TableNumber).ToList
            Model.CurrentRound.Games.Clear()
            Model.CurrentRound.Games.AddRange(SortedGames)

            Dim RemainingGames = From p In Model.CurrentRound.Games Where p.Reported = False

            If RemainingGames.Count = 0 Then
                Dim q = From p In Model.CurrentRound.Players Where p.TourneyPoints = Model.CurrentRound.RoundNumber

                If q.Count < 2 Then
                    Model.CurrentRound.IsLastRound = True
                End If
            End If
        End If
    End Sub

    Private Function GetPlayerFromRound(TargetPlayer As doPlayer) As doPlayer
        Dim dopReturn As doPlayer = Nothing
        If Not TargetPlayer Is Nothing Then dopReturn = (From p In Model.CurrentRound.Players Where p.PlayerID = TargetPlayer.PlayerID).FirstOrDefault
        Return dopReturn
    End Function

    Private Function GetPlayerFromLastRound(TargetPlayer As doPlayer) As doPlayer
        Dim dopReturn As doPlayer = Nothing
        If Model.CurrentRound.RoundNumber > 1 Then
            Dim TargetRound = (From p In Model.WMEvent.Rounds Where p.RoundNumber = Model.CurrentRound.RoundNumber - 1).FirstOrDefault
            If Not TargetPlayer Is Nothing Then dopReturn = (From p In TargetRound.Players Where p.PlayerID = TargetPlayer.PlayerID).FirstOrDefault
        End If

        Return dopReturn
    End Function
End Class
