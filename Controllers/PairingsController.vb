Imports System.Text

Public Class PairingsController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New GamesController
    End Function

    Public Sub New()
        Me.View = New Pairings

        Me.View.DataContext = Me
    End Sub

    Public Sub PrintPairings()
        If BaseController.Model.CurrentRound.Games.Count > 0 Then
            Dim sbOutput As New StringBuilder
            sbOutput.Append(My.Resources.HTMLHeader)
            sbOutput.AppendFormat(My.Resources.Header, BaseController.Model.WMEvent.Name, BaseController.Model.CurrentRound.RoundNumber)

            Dim sbLeftBlock As New StringBuilder()
            Dim sbRightBlock As New StringBuilder()

            Dim PlayerGames As New List(Of PlayerGame)
            For Each player In (From p In BaseController.Model.CurrentRound.Players Order By p.Name)
                Dim game = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = player.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = player.PPHandle)).FirstOrDefault

                Dim pg As PlayerGame = Nothing
                If game.Player2 Is Nothing Then
                    'bye
                    pg.Player = game.Player1.Name
                    pg.Table = "-"
                    pg.Opponent = "Bye"
                ElseIf game.Player1.PPHandle = player.PPHandle Then
                    pg.Player = game.Player1.Name
                    pg.Table = game.TableNumber
                    pg.Opponent = game.Player2.Name
                ElseIf game.Player2.PPHandle = player.PPHandle Then
                    pg.Player = game.Player2.Name
                    pg.Table = game.TableNumber
                    pg.Opponent = game.Player1.Name
                End If
                PlayerGames.Add(pg)
            Next

            For i As Integer = 0 To Math.Ceiling(PlayerGames.Count / 2) - 1
                sbLeftBlock.AppendFormat(My.Resources.Item, New String() {PlayerGames(i).Player, PlayerGames(i).Table, PlayerGames(i).Opponent})
                sbRightBlock.AppendFormat(My.Resources.Item, New String() {PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Player, PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Table, PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Opponent})
            Next

            sbOutput.AppendFormat(My.Resources.LeftColumn, sbLeftBlock.ToString)
            sbOutput.AppendFormat(My.Resources.RightColumn, sbRightBlock.ToString)
            sbOutput.Append(My.Resources.Footer)


            IO.File.WriteAllText(".\PairingsList.html", sbOutput.ToString)
            Process.Start(".\PairingsList.html")
        End If
    End Sub

    Public Sub GeneratePairings()
        Dim UnpairedPlayers = True
        Dim rnd As New Random

        'Clear the current pairings in case this is a re-generate
        Model.CurrentRound.Games.Clear()

        Dim EligablePlayers = (From p In Model.CurrentRound.Players Where p.Drop = False Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending).ToList

        If EligablePlayers.Count Mod 2 = 1 Then
            'Remove a bye volunteer
            Model.CurrentRound.Bye = (From p In EligablePlayers Where p.ByeVol = True And p.TourneyPoints = 0).FirstOrDefault
            If Model.CurrentRound.Bye Is Nothing Then
                Dim EligableBye = (From p In EligablePlayers Where p.TourneyPoints = 0).ToList
                If EligableBye.Count = 0 Then
                    Dim AlreadyByed = (From p In Model.WMEvent.Rounds Select p.Bye).ToList
                    Model.CurrentRound.Bye = (From p In EligablePlayers Where Not AlreadyByed.Contains(p) Order By p.Rank Ascending).FirstOrDefault
                    If Model.CurrentRound.Bye Is Nothing Then
                        Model.CurrentRound.Bye = EligablePlayers.Item(rnd.Next(EligableBye.Count - 1))
                    End If
                Else
                    Model.CurrentRound.Bye = EligableBye.Item(rnd.Next(EligableBye.Count - 1))
                End If


            End If
            EligablePlayers.Remove(Model.CurrentRound.Bye)
        End If
        If Model.CurrentRound.Bye IsNot Nothing Then
            Model.CurrentRound.Games.Add(New doGame)
            Model.CurrentRound.Games.FirstOrDefault.Player1 = Model.CurrentRound.Bye.Clone
            Model.CurrentRound.Games.FirstOrDefault.GameID = Guid.NewGuid
        End If

        Dim Player1 As doPlayer
        Dim Player2 As doPlayer
        If Model.CurrentRound.RoundNumber > 1 Then
            'not the first round, use scoring model
            While UnpairedPlayers
                If EligablePlayers.Count > 1 Then
                    Player1 = EligablePlayers.First

                    'Exclude player self-match
                    Dim EligableOpponents = From p In EligablePlayers Where Not p Is Player1
                    'Exclude previous matchups
                    EligableOpponents = From p In EligableOpponents Where Not Player1.Oppontnents.Contains(p.PPHandle)
                    'Eligable Opponents should already be listed in TP/SOS/CP/APD order, so the next option should be the best
                    Player2 = EligableOpponents.FirstOrDefault

                    EligablePlayers.Remove(Player1)
                    EligablePlayers.Remove(Player2)

                    Dim g As New doGame
                    g.Player1 = Player1.Clone
                    g.Player2 = Player2.Clone
                    g.GameID = Guid.NewGuid
                    Model.CurrentRound.Games.Add(g)
                Else
                    UnpairedPlayers = False
                End If
            End While

        Else
            'use difference model
            While UnpairedPlayers
                If EligablePlayers.Count > 1 Then
                    Player1 = EligablePlayers(rnd.Next(EligablePlayers.Count - 1))
                    'Exclude player self-match
                    Dim EligableOpponents = From p In EligablePlayers Where Not p Is Player1
                    'Exclude previous matchups
                    EligableOpponents = From p In EligableOpponents Where Not Player1.Oppontnents.Contains(p.PPHandle)
                    'check for most distant pairing first:
                    Dim MatchedOpponents = From p In EligableOpponents Where p.Meta <> Player1.Meta AndAlso p.Faction <> Player1.Faction
                    'No one from a different meta with a different faction
                    If MatchedOpponents.Count = 0 Then
                        MatchedOpponents = From p In EligableOpponents Where p.Meta <> Player1.Meta
                        'No one from a different meta
                        If MatchedOpponents.Count = 0 Then
                            MatchedOpponents = From p In EligableOpponents Where p.Faction <> Player1.Faction
                            'No one from teh same meta with a different faction
                            If MatchedOpponents.Count Then
                                'Screw it, give me anyone
                                MatchedOpponents = From p In EligableOpponents
                                If MatchedOpponents.Count = 0 Then
                                    MessageBox.Show("Something bad happened in pairing: No possible matches in the first round!")
                                End If
                            End If
                        End If
                    End If
                    Player2 = MatchedOpponents.ToList.Item(rnd.Next(MatchedOpponents.Count - 1))
                    EligablePlayers.Remove(Player1)
                    EligablePlayers.Remove(Player2)

                    Dim g As New doGame
                    g.Player1 = Player1.Clone
                    g.Player2 = Player2.Clone
                    g.GameID = Guid.NewGuid
                    Model.CurrentRound.Games.Add(g)
                Else
                    UnpairedPlayers = False
                End If

            End While
        End If
    End Sub

    Public Sub SwapPlayers(Player1 As doPlayer, Player2 As doPlayer)
        Dim game1 = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = Player1.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = Player1.PPHandle)).FirstOrDefault
        Dim Game1Player1 As Boolean = False
        If game1.Player1.PPHandle = Player1.PPHandle Then Game1Player1 = True
        Dim game2 = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = Player2.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = Player2.PPHandle)).FirstOrDefault
        Dim Game2Player1 As Boolean = False
        If game2.Player1.PPHandle = Player2.PPHandle Then Game2Player1 = True


        'todo:This needs to be fixed to not use currentRound.player!!!
        If Game1Player1 Then
            game1.Player1 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player2.PPHandle).FirstOrDefault
        Else
            game1.Player2 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player2.PPHandle).FirstOrDefault
        End If

        If Game2Player1 Then
            game2.Player1 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player1.PPHandle).FirstOrDefault
        Else
            game2.Player2 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player1.PPHandle).FirstOrDefault
        End If
    End Sub

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If BaseController.Model.CurrentRound.Games.Count = 0 Then sReturn = "No pairings"
        Return sReturn
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()


        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 1) 'current round + the screen of the round

    End Sub
End Class


Public Structure PlayerGame
    Public Player As String
    Public Table As Integer
    Public Opponent As String
End Structure