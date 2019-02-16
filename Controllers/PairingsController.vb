Imports System.Text
Imports PGSwiss.Data
Imports System.ComponentModel
Imports System.IO
Imports System.Net

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


    Private _setUpload As Boolean? = Nothing
    Public Property UploadAvailable As Boolean
        Get
            If _setUpload Is Nothing Then

                Dim bReturn As Boolean = False
                Try
                    bReturn = CheckForInternetConnection()
                Catch ex As Exception
                    bReturn = False
                End Try
                Return bReturn
            Else
                Return _setUpload
            End If
        End Get
        Set(value As Boolean)
            'do nothing
            _setUpload = value
        End Set
    End Property



    Public Sub PrintPairingsByTableNumber(UploadPairing As Boolean)
        Dim RowString = "<tr><td rowspan=2 class=""tableNumber""><center>Table <br>[ColATableNum]</center></td><td><h4>[ColAPlayer1]</h4><h4><small>[ColAPlayer1alt]</small></h4></td><td class=""blankCell""></td><td><h4>[ColBPlayer1]</h4><h4><small>[ColBPlayer1alt]</small></h4></td><td rowspan=""2"" class=""tableNumber""><center>Table <br>[ColBTableNum]</center></td></tr>" &
                                                                                                          "<tr><td><h4>[ColAPlayer2]</h4><h4><small>[ColAPlayer2alt]</small></h4></td><td class=""blankCell""></td><td><h4>[ColBPlayer2]</h4><h4><small>[ColBPlayer2alt]</small></h4></td></tr><tr><td colspan=""9""></td></tr>"
        If Model.CurrentRound.Games.Count > 0 Then
            Dim sbOutput As New StringBuilder()
            sbOutput.Append(My.Resources.TablesNumber)
            sbOutput.Replace("[Event Title]", Model.WMEvent.Name)
            sbOutput.Replace("[Date]", Model.WMEvent.EventDate.Year & "-" & Model.WMEvent.EventDate.Month & "-" & Model.WMEvent.EventDate.Day)
            sbOutput.Replace("[Location]", "")
            sbOutput.Replace("[Format]", Model.WMEvent.EventFormat.Name)
            sbOutput.Replace("[PG]", "")
            sbOutput.Replace("[Version]", My.Application.Info.Version.ToString())
            sbOutput.Replace("[RoundNum]", Model.CurrentRound.RoundNumber)
            sbOutput.Replace("[Scenario]", Model.CurrentRound.Scenario)
            sbOutput.Replace("[EventDate]", Model.WMEvent.EventDate.ToShortDateString)
            sbOutput.Replace("[EventTitle]", Model.WMEvent.Name)

            Dim index = 1
            Dim row = String.Empty
            Dim rows = String.Empty
            Dim q = (From p In Model.CurrentRound.Games Order By p.TableNumber).ToList()
            For Each game In q
                If index Mod 2 <> 0 Then
                    row = RowString
                    row = row.Replace("[ColATableNum]", game.TableNumber)
                    row = row.Replace("[ColAPlayer1]", game.Player1.Name)
                    row = row.Replace("[ColAPlayer1alt]", game.Player1.Faction)
                    If Not game.Player2 Is Nothing Then
                        row = row.Replace("[ColAPlayer2]", game.Player2.Name)
                        row = row.Replace("[ColAPlayer2alt]", game.Player2.Faction)
                    Else
                        row = row.Replace("[ColAPlayer2]", "Bye")
                    End If
                Else
                    row = row.Replace("[ColBTableNum]", game.TableNumber)
                    row = row.Replace("[ColBPlayer1]", game.Player1.Name)
                    row = row.Replace("[ColBPlayer1alt]", game.Player1.Faction)
                    If Not game.Player2 Is Nothing Then
                        row = row.Replace("[ColBPlayer2]", game.Player2.Name)
                        row = row.Replace("[ColBPlayer2alt]", game.Player2.Faction)
                    Else
                        row = row.Replace("[ColBPlayer2]", "Bye")
                    End If
                    rows &= row
                End If
                index += 1
            Next

            If index Mod 2 = 0 Then
                row = row.Replace("[ColBTableNum]", String.Empty)
                row = row.Replace("[ColBPlayer1]", String.Empty)
                row = row.Replace("[ColBPlayer2]", String.Empty)
                row = row.Replace("[ColBPlayer1alt]", String.Empty)
                row = row.Replace("[ColBPlayer2alt]", String.Empty)
                rows &= row
            End If

            sbOutput.Replace("[Rows]", rows)
            If Not IO.File.Exists(".\Ringdev.png") Then My.Resources.RingDev.Save(".\Ringdev.png")
            If Not IO.File.Exists(".\pgswiss_small.png") Then My.Resources.pgswiss_small.Save(".\pgswiss_small.png")
            If Not IO.File.Exists(".\pgswiss_icon.png") Then My.Resources.pgswiss_icon.Save(".\pgswiss_icon.png")
            IO.File.WriteAllText(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html", sbOutput.ToString)

            If UploadPairing Then
                WebAPIHelper.UploadFile(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html")
                Process.Start("http://ringdev.com/swiss/standings/" & Model.WMEvent.EventID.ToString & "PairingsList.html")
            Else
                Process.Start(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html")
            End If
        End If

    End Sub

    Public Sub PrintPairingsAlphaBetical(UploadPairing As Boolean)
        If Model.CurrentRound.Games.Count > 0 Then
            Dim sbOutput As New StringBuilder()
            sbOutput.Append(My.Resources.TablesAlpha)
            sbOutput.Replace("[Event Title]", Model.WMEvent.Name)
            sbOutput.Replace("[Date]", Model.WMEvent.EventDate.Year & "-" & Model.WMEvent.EventDate.Month & "-" & Model.WMEvent.EventDate.Day)
            sbOutput.Replace("[Location]", "")
            sbOutput.Replace("[Format]", Model.WMEvent.EventFormat.Name)
            sbOutput.Replace("[PG]", "")
            sbOutput.Replace("[Version]", My.Application.Info.Version.ToString())
            sbOutput.Replace("[RoundNum]", Model.CurrentRound.RoundNumber)
            sbOutput.Replace("[Scenario]", Model.CurrentRound.Scenario)
            sbOutput.Replace("[EventDate]", Model.WMEvent.EventDate.ToShortDateString)
            sbOutput.Replace("[EventTitle]", Model.WMEvent.Name)

            Dim PlayerGames As New List(Of PlayerGame)
            For Each player In (From p In Model.CurrentRoundPlayers Order By p.Name)
                Dim game = (From p In Model.CurrentRound.Games Where p.Player1.Name = player.Name Or (p.Player2 IsNot Nothing AndAlso p.Player2.Name = player.Name)).FirstOrDefault
                If Not game Is Nothing Then
                    Dim pg As PlayerGame = Nothing
                    If game.Player2 Is Nothing Then
                        'bye
                        pg.Player = game.Player1.Name
                        pg.Table = 0
                        pg.Opponent = "Bye"
                    ElseIf game.Player1.Name = player.Name Then
                        pg.Player = game.Player1.Name
                        pg.Table = game.TableNumber
                        pg.Opponent = game.Player2.Name
                    ElseIf game.Player2.Name = player.Name Then
                        pg.Player = game.Player2.Name
                        pg.Table = game.TableNumber
                        pg.Opponent = game.Player1.Name
                    End If
                    PlayerGames.Add(pg)
                End If
            Next

            Dim sbLeftBlock As New StringBuilder()
            Dim sbRightBlock As New StringBuilder()

            Dim LastNameInFirstColumn As String = String.Empty
            For i As Integer = 0 To PlayerGames.Count - 1
                If i < Math.Ceiling(PlayerGames.Count / 2) Then
                    sbLeftBlock.Append("<div class=""row""><h3>[Name] </h3><h4>Table [TableNumber]<small> vs [Opponent]</small></h4></div>")
                    Dim name = PlayerGames(i).Player
                    sbLeftBlock.Replace("[Name]", name)
                    sbLeftBlock.Replace("[TableNumber]", PlayerGames(i).Table)
                    name = PlayerGames(i).Opponent
                    sbLeftBlock.Replace("[Opponent]", name)
                    LastNameInFirstColumn = PlayerGames(i).Player
                Else
                    sbRightBlock.Append("<div class=""row""><h3>[Name] </h3><h4>Table [TableNumber]<small> vs [Opponent]</small></h4></div>")
                    Dim name = PlayerGames(i).Player
                    sbRightBlock.Replace("[Name]", name)
                    sbRightBlock.Replace("[TableNumber]", PlayerGames(i).Table)
                    name = PlayerGames(i).Opponent
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
            IO.File.WriteAllText(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html", sbOutput.ToString)

            If UploadPairing Then
                WebAPIHelper.UploadFile(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html")
                Process.Start("http://ringdev.com/swiss/standings/" & Model.WMEvent.EventID.ToString & "PairingsList.html")
            Else
                Process.Start(".\" & Model.WMEvent.EventID.ToString & "PairingsList.html")
            End If
        End If

        If UploadPairing Then
            Dim Standings As New doStandings
            Standings.Standings = Model.CurrentRound.GetPlayers(Model.WMEvent)
            Model.CurrentRound.GetPlayers(Model.WMEvent)

            Dim ctrlStandings As New StandingsController
            ctrlStandings.SetEventStandings(Standings)
            ctrlStandings.PrintStandings(True)
        End If
    End Sub


    Private rnd As New Random()

    Public Function GeneratePairings(ScrewWithExistingPairings As Boolean) As String
        Dim rnd As New Random
        Dim sReturn As String = String.Empty

        Dim Player1 As doPlayer = Nothing
        Dim Player2 As doPlayer = Nothing

        Model.CurrentRound.Bye = Nothing

        Try
            Dim EligablePlayers As New List(Of doPlayer)
            If ScrewWithExistingPairings Then


                'clear the opponents for the current games
                For Each game In Model.CurrentRound.Games
                    If game.Player1 IsNot Nothing AndAlso game.Player2 IsNot Nothing Then
                        Player1 = (From p In Model.CurrentRoundPlayers Where p.Name = game.Player1.Name Select p).FirstOrDefault
                        Player2 = (From p In Model.CurrentRoundPlayers Where p.Name = game.Player2.Name Select p).FirstOrDefault


                        Player1.Opponents.Remove(game.Player2.Name)
                        If Player1.PairedDownRound = Model.CurrentRound.RoundNumber Then Player1.PairedDownRound = 0



                        Player2.Opponents.Remove(game.Player1.Name)
                        If Player2.PairedDownRound = Model.CurrentRound.RoundNumber Then Player2.PairedDownRound = 0


                    End If
                Next
                EligablePlayers.AddRange(From p In Model.CurrentRoundPlayers Where p.Drop = False Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending)
                'Clear the current pairings in case this is a re-generate
                Model.CurrentRound.Games.Clear()
            Else
                EligablePlayers.AddRange(From p In Model.CurrentRoundPlayers Where p.Drop = False Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending)

                For Each game In Model.CurrentRound.Games
                    If game.Player2 Is Nothing OrElse game.Player2.Name <> "Bye" Then
                        EligablePlayers.Remove((From p In EligablePlayers Where p.Name = game.Player1.Name).FirstOrDefault)
                        EligablePlayers.Remove((From p In EligablePlayers Where p.Name = game.Player2.Name).FirstOrDefault)
                    End If
                Next
            End If



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
                EligablePlayers.Remove((From p In EligablePlayers Where p.Name = Model.CurrentRound.Bye.Name).FirstOrDefault)
            End If
            If Model.CurrentRound.Bye IsNot Nothing Then
                Dim byeGame = New doGame
                byeGame.Player1 = Model.CurrentRound.Bye.Clone
                byeGame.GameID = Guid.NewGuid
                Model.CurrentRound.Games.Add(byeGame)
            End If
            '***************************************************************


            If Model.CurrentRound.RoundNumber > 1 Then
                'not the first round, use wins model
                Dim TopTP = Model.CurrentRound.RoundNumber '  (From p In EligablePlayers Select p.TourneyPoints).Max

                For WinsBucket As Integer = TopTP To 0 Step -1
                    Dim i As Integer = WinsBucket
                    While (From p In EligablePlayers Where p.TourneyPoints = i).Count > 0
                        Dim EligablePlayersInBucket = (From p In EligablePlayers Where p.TourneyPoints = i).ToList

                        If EligablePlayersInBucket.Count Mod 2 = 1 Then
                            'Pairdown!
                            Dim PairdownEligablePlayers = (From p In EligablePlayersInBucket Where p.PairedDownRound <= 0 Select p Order By p.Name)
                            If PairdownEligablePlayers.Count = 0 Then PairdownEligablePlayers = (From p In EligablePlayersInBucket Select p Order By Guid.NewGuid())
                            Player1 = PairdownEligablePlayers.ElementAt(rnd.Next(0, PairdownEligablePlayers.Count))

                            Player1.PairedDownRound = Model.CurrentRound.RoundNumber
                            Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 1 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 2 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 3 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 4 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                        ElseIf EligablePlayersInBucket.Count > 0 Then
                            'Standard pairing
                            Player1 = EligablePlayersInBucket.First
                            Player2 = (From p In EligablePlayersInBucket Where Player1.Name <> p.Name AndAlso Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 1 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 2 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 3 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
                            If Player2 Is Nothing Then Player2 = (From p In EligablePlayers Where p.TourneyPoints = i - 4 And Not Player1.Opponents.Contains(p.Name) Order By Guid.NewGuid()).FirstOrDefault
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
                        EligableOpponents = From p In EligableOpponents Where Not Player1.Opponents.Contains(p.Name)
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
            For Each game In Model.CurrentRound.Games
                If game.Player1 IsNot Nothing Then game.Player1.TotalTourneyPoints = GetPlayerTotalTourneyPoints(game.Player1.Name, Model.CurrentRound.RoundNumber)
                If game.Player2 IsNot Nothing Then game.Player2.TotalTourneyPoints = GetPlayerTotalTourneyPoints(game.Player2.Name, Model.CurrentRound.RoundNumber)
            Next


            'Tables
            Dim NonByeGames = (From p In Model.CurrentRound.Games Where p.Player2 IsNot Nothing).ToArray
            Dim Tables As New List(Of Integer)
            For i = 1 To NonByeGames.Count
                Tables.Add(i)
            Next
            For Each game In (From p In NonByeGames Order By p.Player1.Rank + p.Player2.Rank Ascending)
                Dim InvalidTables As New List(Of Integer)
                Dim EventPlayer1 = (From p In Model.WMEvent.Players Where p.Name = game.Player1.Name Select p).FirstOrDefault
                Dim EventPlayer2 = (From p In Model.WMEvent.Players Where p.Name = game.Player2.Name Select p).FirstOrDefault
                If Not EventPlayer1 Is Nothing AndAlso Not EventPlayer2 Is Nothing Then
                    InvalidTables.AddRange(EventPlayer1.Tables)
                    InvalidTables.AddRange(EventPlayer2.Tables)
                End If
                game.TableNumber = (From p In Tables Where Not InvalidTables.Contains(p)).FirstOrDefault
                If game.TableNumber = 0 Then game.TableNumber = Tables.Item(rnd.Next(Tables.Count))
                Tables.Remove(game.TableNumber)
                SetPairingCondition(game)

            Next


            'Default Scenario
            For Each game In (From p In NonByeGames Order By p.Player1.Rank + p.Player2.Rank Ascending)
                If game.Scenario = String.Empty Then game.Scenario = Model.CurrentRound.Scenario
            Next
            _ErrorRetryCount = 0
        Catch e As Exception
            _ErrorRetryCount += 1
            If _ErrorRetryCount <= 5 Then
                GeneratePairings(ScrewWithExistingPairings)
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
        Dim EventPlayer1 = (From p In Model.WMEvent.Players Where p.Name = game.Player1.Name Select p).FirstOrDefault
        Dim EventPlayer2 = (From p In Model.WMEvent.Players Where p.Name = game.Player2.Name Select p).FirstOrDefault
        If EventPlayer1 IsNot Nothing AndAlso EventPlayer2 IsNot Nothing Then
            If EventPlayer1.Tables.Contains(game.TableNumber) OrElse EventPlayer2.Tables.Contains(game.TableNumber) Then
                game.PairingCondition += 2
            End If
        End If

        If game.Player1.Opponents.Contains(game.Player2.Name) OrElse game.Player2.Opponents.Contains(game.Player1.Name) Then
            game.PairingCondition += 4
        End If
        If game.IsPairdown Then
            game.PairingCondition += 8
        End If
    End Sub

    Public Sub SwapPlayers(PlayerA As doPlayer, PlayerB As doPlayer)
        Dim SourceGame = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.Name = PlayerA.Name Or (p.Player2 IsNot Nothing AndAlso p.Player2.Name = PlayerA.Name)).FirstOrDefault
        Dim TargetGame = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.Name = PlayerB.Name Or (p.Player2 IsNot Nothing AndAlso p.Player2.Name = PlayerB.Name)).FirstOrDefault

        If SourceGame.Player1.Name = PlayerA.Name Then
            If TargetGame.Player1.Name = PlayerB.Name Then
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
            If TargetGame.Player1.Name = PlayerB.Name Then
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
    End Sub

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If BaseController.Model.CurrentRound.Games.Count = 0 Then sReturn = "No pairings"

        'cleanup for reopening old files and if someone manually pairs a pairdown
        For Each game In Model.CurrentRound.Games
            If game.Player2 IsNot Nothing Then
                If GetPlayerTotalTourneyPoints(game.Player1.Name, Model.CurrentRound.RoundNumber - 1) > GetPlayerTotalTourneyPoints(game.Player2.Name, Model.CurrentRound.RoundNumber - 1) Then
                    game.Player1.PairedDownRound = Model.CurrentRound.RoundNumber
                    Dim q = (From p In Model.WMEvent.Players Where p.Name = game.Player1.Name Select p).First()
                    q.PairedDownRound = Model.CurrentRound.RoundNumber()
                ElseIf GetPlayerTotalTourneyPoints(game.Player2.Name, Model.CurrentRound.RoundNumber - 1) > GetPlayerTotalTourneyPoints(game.Player1.Name, Model.CurrentRound.RoundNumber - 1) Then
                    game.Player2.PairedDownRound = Model.CurrentRound.RoundNumber
                    Dim q = (From p In Model.WMEvent.Players Where p.Name = game.Player2.Name Select p).First()
                    q.PairedDownRound = Model.CurrentRound.RoundNumber
                Else
                    If game.Player1.PairedDownRound < 0 Or game.Player1.PairedDownRound = Model.CurrentRound.RoundNumber Then game.Player1.PairedDownRound = 0
                    If game.Player2.PairedDownRound < 0 Or game.Player1.PairedDownRound = Model.CurrentRound.RoundNumber Then game.Player2.PairedDownRound = 0
                End If
            Else
                Model.CurrentRound.Bye = game.Player1
            End If
        Next



        Return sReturn
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        'If Model.CurrentRound.Games.Count = 0 Then
        Dim GamePlayerCount = 0
        For Each game In Model.CurrentRound.Games
            If game.Player2 Is Nothing OrElse game.Player2.Name = "Bye" Then
                GamePlayerCount += 1
            Else
                GamePlayerCount += 2
            End If
        Next

        If Model.CurrentRound.GetPlayers(WMEventViewModel.GetSingleton.WMEvent).Count <> GamePlayerCount Then
            View.DataContext = Nothing
            Dim result = GeneratePairings(False)
            If result <> String.Empty Then MessageBox.Show(result)
            View.DataContext = Me
        End If



        'End If

        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 1) 'current round + the screen of the round
        OnPropertyChanged("RegenerateAvailable")
        OnPropertyChanged("UploadAvailable")
    End Sub


    Public Shared Function CheckForInternetConnection() As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead("http://www.google.com")
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function


    Private Overloads Function GetPlayerTotalTourneyPoints(PlayerName As String, RoundNumber As Integer) As Integer
        Dim iReturn As Integer = 0
        For Each Round In Model.WMEvent.Rounds
            If Round.RoundNumber <= RoundNumber Then
                For Each game In Round.Games
                    If game.Winner = PlayerName Then
                        iReturn += 1
                        Exit For
                    End If
                Next
            End If
        Next
        Return iReturn
    End Function

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal name As String)
        PGSwiss.Data.DirtyMonitor.IsDirty = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class


Public Structure PlayerGame
    Public Player As String
    Public Table As Integer
    Public Opponent As String
End Structure