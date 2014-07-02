Imports PGSwiss.Data
Imports System.ComponentModel

Public Class GamesController
    Inherits BaseController
    Implements INotifyPropertyChanged

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
        BaseController.Model.CurrentGame = BaseController.Model.CurrentRound.Games.FirstOrDefault
        Me.View.DataContext = Me

        _Round = Model.CurrentRound
    End Sub


    Private _Round As doRound

    Public ReadOnly Property AcceptableWinners As List(Of String)
        Get
            Dim lReturn As New List(Of String)
            lReturn.Add("")
            lReturn.Add("Draw")
            If Model.CurrentGame IsNot Nothing Then
                If Model.CurrentGame.Player1 IsNot Nothing Then lReturn.Add(Model.CurrentGame.Player1.PPHandle)
                If Model.CurrentGame.Player2 IsNot Nothing Then lReturn.Add(Model.CurrentGame.Player2.PPHandle)
            End If

            Return lReturn
        End Get
    End Property

    Public Sub SetWinnerByScenario(WinnerPPHandle As String)
        If Model.CurrentGame.Player1.PPHandle = WinnerPPHandle Then
            Model.CurrentGame.Player1.ControlPoints = 5
        ElseIf Model.CurrentGame.Player2 IsNot Nothing AndAlso Model.CurrentGame.Player2.PPHandle = WinnerPPHandle Then
            Model.CurrentGame.Player2.ControlPoints = 5
        Else
            MessageBox.Show("Something bad just happened. Scenario winner is not a member of this game!")
        End If
    End Sub

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentRound = _Round

        If Model.CurrentRound.Bye IsNot Nothing Then
            Dim q = (From p In Model.CurrentRound.Games Where p.Player2 Is Nothing).FirstOrDefault
            If Not q Is Nothing Then
                SelectGame(q)
                Model.CurrentGame.Winner = q.Player1.PPHandle
                Model.CurrentGame.Condition = "Time"
                Model.CurrentGame.Player1.ControlPoints = 3
                Model.CurrentGame.Player1.ArmyPointsDestroyed = Math.Ceiling(Model.CurrentRound.Size / 2)
                AcceptGame()
            End If
        End If

        Dim totalPlayers = Model.WMEvent.Players.Count
        Dim Rounds As Integer = 1
        While 2 ^ Rounds < totalPlayers
            Rounds += 1
        End While
        Dim ValuePerRoundScreen = 80 / Rounds / 3 '85% to work with, diveded across all rounds, each round has 3 screens
        Model.CurrentProgress = ValuePerRoundScreen * (Model.CurrentRound.RoundNumber * 3 + 2) 'current round + the screen of the round

        'Pull the previous game screen's set duration. This is a dirty hack.
        If Model.CurrentRound.RoundNumber > 1 Then
            Dim CurrentIndex = _Stack.IndexOf(Me)
            Dim TargetIndex As Integer = CurrentIndex - 1
            While TargetIndex > 0 AndAlso _Stack.Item(TargetIndex).GetType IsNot GetType(GamesController)
                TargetIndex -= 1
            End While
            If TargetIndex > 0 Then
                CType(Me.View, Games).MasterClock.SetDuration = CType(CType(_Stack(TargetIndex), GamesController).View, Games).MasterClock.SetDuration
            End If
        End If
    End Sub

    Public Sub SelectGame(Game As doGame)
        Model.CurrentGame = Game
        OnPropertyChanged("AcceptableWinners")
    End Sub

    Public Function AcceptGame() As String
        Dim sReturn = String.Empty
        If Model.CurrentGame.Winner = String.Empty Then sReturn = "Winner not set" & ControlChars.CrLf
        If Model.CurrentGame.Condition = String.Empty Then sReturn &= "Condition not set"
        If sReturn = String.Empty Then
            If BaseController.Model.CurrentGame IsNot Nothing Then
                Dim Player1 = Model.CurrentGame.Player1
                Dim Player2 = Model.CurrentGame.Player2

                Dim Player1FromRound = GetPlayerFromRound(Player1)
                Dim Player1FromLastRound = GetPlayerFromLastRound(Player1)
                Dim Player2FromRound = GetPlayerFromRound(Player2)
                Dim Player2FromLastRound = GetPlayerFromLastRound(Player2)

                If Player1FromLastRound Is Nothing Then
                    If Model.CurrentGame.Winner = Player1FromRound.PPHandle Then Player1FromRound.TourneyPoints = 1
                    Player1FromRound.ControlPoints = Player1.ControlPoints
                    Player1FromRound.ArmyPointsDestroyed = Player1.ArmyPointsDestroyed
                Else
                    If Model.CurrentGame.Winner = Player1FromRound.PPHandle Then Player1FromRound.TourneyPoints = Player1FromLastRound.TourneyPoints + 1
                    Player1FromRound.ControlPoints = Player1.ControlPoints + Player1FromLastRound.ControlPoints
                    Player1FromRound.ArmyPointsDestroyed = Player1.ArmyPointsDestroyed + Player1FromLastRound.ArmyPointsDestroyed
                End If

                If Not Player2 Is Nothing Then
                    If Player2FromLastRound Is Nothing Then
                        If Model.CurrentGame.Winner = Player2FromRound.PPHandle Then Player2FromRound.TourneyPoints = 1
                        Player2FromRound.ControlPoints = Player2.ControlPoints
                        Player2FromRound.ArmyPointsDestroyed = Player2.ArmyPointsDestroyed
                    Else
                        If Model.CurrentGame.Winner = Player2FromRound.PPHandle Then Player2FromRound.TourneyPoints = Player2FromLastRound.TourneyPoints + 1
                        Player2FromRound.ControlPoints = Player2.ControlPoints + Player2FromLastRound.ControlPoints
                        Player2FromRound.ArmyPointsDestroyed = Player2.ArmyPointsDestroyed + Player2FromLastRound.ArmyPointsDestroyed
                    End If

                    Player1FromRound.Oppontnents.Add(Player2FromRound.PPHandle)
                    Player2FromRound.Oppontnents.Add(Player1FromRound.PPHandle)
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
        End If
        Return sReturn
    End Function

    Private Function GetPlayerFromRound(TargetPlayer As doPlayer) As doPlayer
        Dim dopReturn As doPlayer = Nothing
        If Not TargetPlayer Is Nothing Then dopReturn = (From p In Model.CurrentRound.Players Where p.PPHandle = TargetPlayer.PPHandle).FirstOrDefault
        Return dopReturn
    End Function

    Private Function GetPlayerFromLastRound(TargetPlayer As doPlayer) As doPlayer
        Dim dopReturn As doPlayer = Nothing
        If Model.CurrentRound.RoundNumber > 1 Then
            Dim TargetRound = (From p In Model.WMEvent.Rounds Where p.RoundNumber = Model.CurrentRound.RoundNumber - 1).FirstOrDefault
            If Not TargetPlayer Is Nothing Then dopReturn = (From p In TargetRound.Players Where p.PPHandle = TargetPlayer.PPHandle).FirstOrDefault
        End If

        Return dopReturn
    End Function

    Public Overrides Function Validate() As String
        Dim sReturn = String.Empty
        If (From p In BaseController.Model.CurrentRound.Games Where p.Reported = False).Count > 0 Then sReturn = "Unreported games"
        Return sReturn
    End Function

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

End Class
