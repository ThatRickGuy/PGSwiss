Imports System.Text
Imports PGSwiss.Data
Imports System.ComponentModel
Imports System.IO

Public Class PairingsController
    Inherits BaseController
    Implements INotifyPropertyChanged

    Public ReadOnly Property Players
        Get
            Return Model.CurrentRoundPlayers
        End Get
    End Property

    Protected Overrides Function CreateNext() As BaseController
        Return New GamesController
    End Function

    Public Sub New()
        Me.View = New Pairings

        Me.View.DataContext = Me
    End Sub

    Public ReadOnly Property RegenerateAvailable As Boolean
        Get
            Dim bReturn As Boolean = False
            If (From p In BaseController.Model.CurrentRound.Games
                     Where p.Winner <> String.Empty AndAlso p.Player2 IsNot Nothing).Count = 0 Then
                bReturn = True
            End If

            Return bReturn
        End Get
    End Property



    Public Sub PrintPairingsByTableNumber()
        'If Model.CurrentRound.Games.Count > 0 Then
        '    Dim sbOutput As New StringBuilder
        '    sbOutput.Append(My.Resources.HTMLHeader)
        '    sbOutput.AppendFormat(My.Resources.Header, Model.WMEvent.Name, Model.CurrentRound.RoundNumber, Model.CurrentRound.Scenario & " " & Model.CurrentRound.Size & "pts")

        '    Dim sbLeftBlock As New StringBuilder()
        '    Dim sbRightBlock As New StringBuilder()

        '    Dim i As Integer
        '    Dim LastTableInLeftColumn As Integer = 0
        '    Dim FirstTableInRightColumn As Integer = 0
        '    Dim LastTableInRightColumn As Integer = 0
        '    For Each game In (From p In Model.CurrentRound.Games Order By p.TableNumber Ascending)
        '        If game.Player2 Is Nothing Then
        '            If i < Math.Ceiling(Model.CurrentRound.Games.Count / 2) Then
        '                sbLeftBlock.AppendFormat(My.Resources.Table, game.TableNumber, game.Player1.Name, game.Player1.PPHandle, "BYE", "")
        '                LastTableInLeftColumn = game.TableNumber
        '            Else
        '                sbRightBlock.AppendFormat(My.Resources.Table, game.TableNumber, game.Player1.Name, game.Player1.PPHandle, "BYE", "")
        '                If FirstTableInRightColumn = 0 Then FirstTableInRightColumn = game.TableNumber
        '                LastTableInRightColumn = game.TableNumber
        '            End If
        '        Else
        '            If i < Math.Ceiling(Model.CurrentRound.Games.Count / 2) Then
        '                sbLeftBlock.AppendFormat(My.Resources.Table, game.TableNumber, game.Player1.Name, game.Player1.PPHandle, game.Player2.Name, game.Player2.PPHandle)
        '                LastTableInLeftColumn = game.TableNumber
        '            Else
        '                sbRightBlock.AppendFormat(My.Resources.Table, game.TableNumber, game.Player1.Name, game.Player1.PPHandle, game.Player2.Name, game.Player2.PPHandle)
        '                If FirstTableInRightColumn = 0 Then FirstTableInRightColumn = game.TableNumber
        '                LastTableInRightColumn = game.TableNumber
        '            End If
        '        End If
        '        i += 1
        '    Next

        '    sbOutput.AppendFormat(My.Resources.LeftColumn, "Tables 1 - " & LastTableInLeftColumn, sbLeftBlock.ToString)
        '    sbOutput.AppendFormat(My.Resources.RightColumn, "Tables " & FirstTableInRightColumn & " - " & LastTableInRightColumn, sbRightBlock.ToString)
        '    sbOutput.Append(My.Resources.Footer)

        '    IO.File.WriteAllText(".\PairingsList.html", sbOutput.ToString)
        '    Process.Start(".\PairingsList.html")
        'End If
    End Sub

    Public Sub PrintPairingsAlphaBetical()
        If Model.CurrentRound.Games.Count > 0 Then
            Dim sbOutput As New StringBuilder()
            sbOutput.Append(My.Resources.TablesAlpha)
            sbOutput.Replace("[Event Title]", Model.WMEvent.Name)
            sbOutput.Replace("[Date]", Model.WMEvent.EventDate.ToShortDateString)
            sbOutput.Replace("[Location]", "")
            sbOutput.Replace("[Format]", Model.WMEvent.EventFormat.Name)
            sbOutput.Replace("[PG]", "")
            sbOutput.Replace("[Version]", My.Application.Info.Version.ToString())
            sbOutput.Replace("[RoundNum]", Model.CurrentRound.RoundNumber)


            Dim PlayerGames As New List(Of PlayerGame)
            For Each player In (From p In Model.CurrentRoundPlayers Order By p.Name)
                Dim game = (From p In Model.CurrentRound.Games Where p.Player1.PPHandle = player.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = player.PPHandle)).FirstOrDefault
                If Not game Is Nothing Then
                    Dim pg As PlayerGame = Nothing
                    If game.Player2 Is Nothing Then
                        'bye
                        pg.Player = game.Player1.Name
                        pg.PlayerHandle = game.Player1.PPHandle
                        pg.Table = 0
                        pg.Opponent = "Bye"
                    ElseIf game.Player1.PPHandle = player.PPHandle Then
                        pg.Player = game.Player1.Name
                        pg.PlayerHandle = game.Player1.PPHandle
                        pg.Table = game.TableNumber
                        pg.Opponent = game.Player2.Name
                        pg.OpponentHandle = game.Player2.PPHandle
                    ElseIf game.Player2.PPHandle = player.PPHandle Then
                        pg.Player = game.Player2.Name
                        pg.PlayerHandle = game.Player2.PPHandle
                        pg.Table = game.TableNumber
                        pg.Opponent = game.Player1.Name
                        pg.OpponentHandle = game.Player1.PPHandle
                    End If
                    PlayerGames.Add(pg)
                End If
            Next

            Dim sbLeftBlock As New StringBuilder()
            Dim sbRightBlock As New StringBuilder()

            Dim LastNameInFirstColumn As String = String.Empty
            For i As Integer = 0 To PlayerGames.Count - 1
                If i < Math.Ceiling(PlayerGames.Count / 2) Then
                    sbLeftBlock.Append("<div class=""row""><h3>[PlayerName] </h3><h4>Table [TableNumber]<small> vs [Opponent]</small></h4></div>")
                    Dim name = PlayerGames(i).Player
                    If PlayerGames(i).Player.ToLower <> PlayerGames(i).PlayerHandle.ToLower AndAlso PlayerGames(i).PlayerHandle IsNot Nothing Then name &= " <small>(" & PlayerGames(i).PlayerHandle & ")</small>"
                    sbLeftBlock.Replace("[PlayerName]", name)
                    sbLeftBlock.Replace("[TableNumber]", PlayerGames(i).Table)
                    name = PlayerGames(i).Opponent
                    If PlayerGames(i).Opponent.ToLower <> PlayerGames(i).OpponentHandle.ToLower AndAlso PlayerGames(i).OpponentHandle IsNot Nothing Then name &= " <small>(" & PlayerGames(i).OpponentHandle & ")</small>"
                    sbLeftBlock.Replace("[Opponent]", name)
                    LastNameInFirstColumn = PlayerGames(i).Player
                Else
                    sbRightBlock.Append("<div class=""row""><h3>[PlayerName] </h3><h4>Table [TableNumber]<small> vs [Opponent]</small></h4></div>")
                    Dim name = PlayerGames(i).Player
                    If PlayerGames(i).Player.ToLower <> PlayerGames(i).PlayerHandle.ToLower AndAlso PlayerGames(i).PlayerHandle IsNot Nothing Then name &= " <small>(" & PlayerGames(i).PlayerHandle & ")</small>"
                    sbRightBlock.Replace("[PlayerName]", name)
                    sbRightBlock.Replace("[TableNumber]", PlayerGames(i).Table)
                    name = PlayerGames(i).Opponent
                    If PlayerGames(i).Opponent.ToLower <> PlayerGames(i).OpponentHandle.ToLower AndAlso PlayerGames(i).OpponentHandle IsNot Nothing Then name &= " <small>(" & PlayerGames(i).OpponentHandle & ")</small>"
                    sbRightBlock.Replace("[Opponent]", name)
                End If
            Next



            sbOutput.Replace("[ColumnHeader1]", "A - " & LastNameInFirstColumn.Substring(0, 1).ToUpper)
            sbOutput.Replace("[Rows1]", sbLeftBlock.ToString())

            sbOutput.Replace("[ColumnHeader2]", Chr(Asc(LastNameInFirstColumn.Substring(0, 1).ToUpper) + 1) & " - Z")
            sbOutput.Replace("[Rows2]", sbRightBlock.ToString())

            If Not IO.File.Exists(".\Ringdev.png") Then My.Resources.RingDev.Save(".\Ringdev.png")
            If Not IO.File.Exists(".\pgswiss_small.png") Then My.Resources.pgswiss_small.Save(".\pgswiss_small.png")
            If Not IO.File.Exists(".\pgswiss_icon.png") Then My.Resources.pgswiss_icon.Save(".\pgswiss_icon.png")
            IO.File.WriteAllText(".\PairingsList.html", sbOutput.ToString)
            Process.Start(".\PairingsList.html")
        End If
    End Sub

    Public Function GeneratePairings() As String
        Dim rnd As New Random
        Dim sReturn As String = String.Empty

        Try
            'Clear the current pairings in case this is a re-generate
            Model.CurrentRound.Games.Clear()
            Model.CurrentRound.Bye = Nothing

            Dim EligablePlayers As New List(Of doPlayer)
            EligablePlayers.AddRange(From p In Model.CurrentRoundPlayers Where p.Drop = False Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending)

            '***************************************************************
            '*** Bye                                                
            '***************************************************************
            If EligablePlayers.Count Mod 2 = 1 Then
                'Remove a bye volunteer
                'volunteers at 0 points
                Model.CurrentRound.Bye = (From p In EligablePlayers Where p.ByeVol = True And p.TourneyPoints = 0).FirstOrDefault
                If Model.CurrentRound.Bye Is Nothing Then
                    'volunteers at >0 points
                    Model.CurrentRound.Bye = (From p In EligablePlayers Where p.ByeVol = True).FirstOrDefault
                    If Model.CurrentRound.Bye Is Nothing Then
                        Dim EligableBye = (From p In EligablePlayers Where p.TourneyPoints = 0).ToList
                        If EligableBye.Count > 0 Then
                            'random person at 0 points (since they are at 0 points, we know they haven't been byed already
                            Model.CurrentRound.Bye = EligableBye.Item(rnd.Next(EligableBye.Count))
                        Else
                            'no one at 0 points
                            'get a list of people who have been byed
                            Dim AlreadyByed = (From p In Model.WMEvent.Rounds Select p.Bye).ToList
                            'get the bottom ranking person who hasn't been byed
                            Model.CurrentRound.Bye = (From p In EligablePlayers Where Not AlreadyByed.Contains(p) Order By p.Rank Descending).FirstOrDefault
                            If Model.CurrentRound.Bye Is Nothing Then
                                'no idea how this is possible, but when in doubt, get the bottom ranked person
                                Model.CurrentRound.Bye = (From p In EligablePlayers Order By p.Rank Descending).FirstOrDefault
                            End If
                        End If

                    End If
                End If
                EligablePlayers.Remove((From p In EligablePlayers Where p.PPHandle = Model.CurrentRound.Bye.PPHandle).FirstOrDefault)
            End If
            If Model.CurrentRound.Bye IsNot Nothing Then
                Model.CurrentRound.Games.Add(New doGame)
                Model.CurrentRound.Games.FirstOrDefault.Player1 = Model.CurrentRound.Bye.Clone
                Model.CurrentRound.Games.FirstOrDefault.GameID = Guid.NewGuid
            End If
            '***************************************************************

            Dim Player1 As doPlayer = Nothing
            Dim Player2 As doPlayer = Nothing
            If Model.CurrentRound.RoundNumber > 1 Then
                'not the first round, use wins model
                Dim TopTP = Model.CurrentRound.RoundNumber '  (From p In EligablePlayers Select p.TourneyPoints).Max

                For WinsBucket As Integer = TopTP To 0 Step -1
                    Dim i As Integer = WinsBucket
                    While (From p In EligablePlayers Where p.TourneyPoints = i).Count > 0
                        Dim EligablePlayersInBucket = (From p In EligablePlayers Where p.TourneyPoints = i).ToList

                        If EligablePlayersInBucket.Count Mod 2 = 1 Then
                            'Pairdown!
                            Player1 = (From p In EligablePlayersInBucket Where p.HasBeenPairedDown = False Select p Order By Guid.NewGuid()).FirstOrDefault()
                            If Player1 Is Nothing Then Player1 = (From p In EligablePlayersInBucket Select p Order By Guid.NewGuid()).First()
                            Player1.HasBeenPairedDown = True
                            Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 1 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 2 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 3 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 4 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                        ElseIf EligablePlayersInBucket.Count > 0 Then
                            'Standard pairing
                            Player1 = EligablePlayersInBucket.First
                            Player2 = (From p In EligablePlayersInBucket Where Player1.PPHandle <> p.PPHandle AndAlso Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 1 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 2 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 3 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 4 And Not Player1.Opponents.Contains(p.PPHandle) Order By Guid.NewGuid()).FirstOrDefault
                        End If

                        EligablePlayers.Remove(Player1)
                        EligablePlayers.Remove(Player2)

                        Dim g As New doGame
                        g.Player1 = Player1.Clone
                        g.Player2 = Player2.Clone
                        g.GameID = Guid.NewGuid
                        If Player1.TourneyPoints <> Player2.TourneyPoints Then g.IsPairdown = True
                        Model.CurrentRound.Games.Add(g)
                    End While
                Next
            Else
                'first round, use difference model

                Dim UnpairedPlayers = True
                While UnpairedPlayers
                    If EligablePlayers.Count > 1 Then
                        Player1 = EligablePlayers(rnd.Next(EligablePlayers.Count - 1))
                        'Exclude player self-match
                        Dim EligableOpponents = From p In EligablePlayers Where Not p Is Player1
                        'Exclude previous matchups
                        EligableOpponents = From p In EligableOpponents Where Not Player1.Opponents.Contains(p.PPHandle)
                        'check for most distant pairing first:
                        Dim MatchedOpponents = From p In EligableOpponents Where p.Meta <> Player1.Meta AndAlso p.Faction <> Player1.Faction
                        'No one from a different meta with a different faction
                        If MatchedOpponents.Count = 0 Then
                            MatchedOpponents = From p In EligableOpponents Where p.Meta <> Player1.Meta
                            'No one from a different meta
                            If MatchedOpponents.Count = 0 Then
                                MatchedOpponents = From p In EligableOpponents Where p.Faction <> Player1.Faction
                                'No one from teh same meta with a different faction
                                If MatchedOpponents.Count = 0 Then
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

            'Tables
            Dim NonByeGames = (From p In Model.CurrentRound.Games Where p.Player2 IsNot Nothing).ToArray
            Dim Tables As New List(Of Integer)
            For i = 1 To NonByeGames.Count
                Tables.Add(i)
            Next
            For Each game In (From p In NonByeGames Order By p.Player1.Rank + p.Player2.Rank Ascending)
                Dim InvalidTables As New List(Of Integer)
                Dim EventPlayer1 = (From p In Model.WMEvent.Players Where p.PPHandle = game.Player1.PPHandle Select p).FirstOrDefault
                Dim EventPlayer2 = (From p In Model.WMEvent.Players Where p.PPHandle = game.Player2.PPHandle Select p).FirstOrDefault
                If Not EventPlayer1 Is Nothing AndAlso Not EventPlayer2 Is Nothing Then
                    InvalidTables.AddRange(EventPlayer1.Tables)
                    InvalidTables.AddRange(EventPlayer2.Tables)
                End If
                game.TableNumber = (From p In Tables Where Not InvalidTables.Contains(p)).FirstOrDefault
                If game.TableNumber = 0 Then game.TableNumber = Tables.Item(rnd.Next(Tables.Count))
                Tables.Remove(game.TableNumber)
                SetPairingCondition(game)
            Next
            _ErrorRetryCount = 0
        Catch e As Exception
            _ErrorRetryCount += 1
            If _ErrorRetryCount <= 5 Then
                GeneratePairings()
            Else
                sReturn = e.Message & ControlChars.CrLf & "Please regenerate the pairing!"
                Model.CurrentRound.Games.Clear()
            End If

        End Try

        Return sReturn
    End Function

    Private _ErrorRetryCount As Integer = 0

    Private Sub SetPairingCondition(game As Data.doGame)
        If game.Player1.Meta = game.Player2.Meta Then
            game.PairingCondition += 1
        End If
        Dim EventPlayer1 = (From p In Model.WMEvent.Players Where p.PPHandle = game.Player1.PPHandle Select p).FirstOrDefault
        Dim EventPlayer2 = (From p In Model.WMEvent.Players Where p.PPHandle = game.Player2.PPHandle Select p).FirstOrDefault
        If EventPlayer1 IsNot Nothing AndAlso EventPlayer2 IsNot Nothing Then
            If EventPlayer1.Tables.Contains(game.TableNumber) OrElse EventPlayer2.Tables.Contains(game.TableNumber) Then
                game.PairingCondition += 2
            End If
        End If

        If game.Player1.Opponents.Contains(game.Player2.PPHandle) OrElse game.Player2.Opponents.Contains(game.Player1.PPHandle) Then
            game.PairingCondition += 4
        End If
        If game.IsPairdown Then
            game.PairingCondition += 8
        End If
    End Sub

    Public Sub SwapPlayers(PlayerA As doPlayer, PlayerB As doPlayer)
        Dim SourceGame = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = PlayerA.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = PlayerA.PPHandle)).FirstOrDefault
        Dim TargetGame = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = PlayerB.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = PlayerB.PPHandle)).FirstOrDefault

        If SourceGame.Player1.PPHandle = PlayerA.PPHandle Then
            If TargetGame.Player1.PPHandle = PlayerB.PPHandle Then
                'Source.1 => Target.1
                'Target.1 => Source.1
                Dim temp = TargetGame.Player1
                TargetGame.Player1 = SourceGame.Player1
                SourceGame.Player1 = temp
            Else
                'Source.1 => Target.2
                'Target.2 => Source.1
                Dim temp = TargetGame.Player2
                TargetGame.Player2 = SourceGame.Player1
                SourceGame.Player1 = temp
            End If
        Else
            If TargetGame.Player1.PPHandle = PlayerB.PPHandle Then
                'Source.1 => Target.1
                'Target.1 => Source.1
                Dim temp = TargetGame.Player1
                TargetGame.Player1 = SourceGame.Player2
                SourceGame.Player2 = temp
            Else
                'Source.1 => Target.2
                'Target.2 => Source.1
                Dim temp = TargetGame.Player2
                TargetGame.Player2 = SourceGame.Player2
                SourceGame.Player2 = temp
            End If
        End If



        'Dim game1 = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = PlayerA.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = PlayerA.PPHandle)).FirstOrDefault
        'Dim Game1Player1 As Boolean = False
        'If game1.Player1.PPHandle = PlayerA.PPHandle Then Game1Player1 = True
        'Dim game2 = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = PlayerB.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = PlayerB.PPHandle)).FirstOrDefault
        'Dim Game2Player1 As Boolean = False
        'If game2.Player1.PPHandle = PlayerB.PPHandle Then Game2Player1 = True


        ''todo:This needs to be fixed to not use currentRound.player!!!
        'If Game1Player1 Then

        '    game1.Player1 = game1.Player1 ' (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player2.PPHandle).FirstOrDefault
        'Else
        '    game1.Player2 = game1.Player2 '(From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = Player2.PPHandle).FirstOrDefault
        'End If

        'If Game2Player1 Then
        '    game2.Player1 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = PlayerA.PPHandle).FirstOrDefault
        'Else
        '    game2.Player2 = (From p In BaseController.Model.CurrentRound.Players Where p.PPHandle = PlayerA.PPHandle).FirstOrDefault
        'End If
    End Sub

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If BaseController.Model.CurrentRound.Games.Count = 0 Then sReturn = "No pairings"
        Return sReturn
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        If Model.CurrentRound.Games.Count = 0 Then
            Dim result = GeneratePairings()
            If result <> String.Empty Then MessageBox.Show(result)
        End If

        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 1) 'current round + the screen of the round

        OnPropertyChanged("RegenerateAvailable")
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class


Public Structure PlayerGame
    Public Player As String
    Public PlayerHandle As String
    Public Table As Integer
    Public Opponent As String
    Public OpponentHandle As String
End Structure