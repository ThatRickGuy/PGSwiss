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
                If Model.CurrentGame.Player1 IsNot Nothing Then lReturn.Add(Model.CurrentGame.Player1.PPHandle & " (" & Model.CurrentGame.Player1.Name & ")")
                If Model.CurrentGame.Player2 IsNot Nothing Then lReturn.Add(Model.CurrentGame.Player2.PPHandle & " (" & Model.CurrentGame.Player2.Name & ")")
            End If

            Return lReturn
        End Get
    End Property

    Public Sub SetWinnerByScenario(WinnerPPHandle As String)
        If Model.CurrentGame.Player1.PPHandle & " (" & Model.CurrentGame.Player1.Name & ")" = WinnerPPHandle Then
            Model.CurrentGame.Player1.ControlPoints = 5
        ElseIf Model.CurrentGame.Player2 IsNot Nothing AndAlso Model.CurrentGame.Player2.PPHandle & " (" & Model.CurrentGame.Player2.Name & ")" = WinnerPPHandle Then
            Model.CurrentGame.Player2.ControlPoints = 5
        Else
            MessageBox.Show("Something bad just happened. Scenario winner is not a member of this game!")
        End If
    End Sub

    Public Sub SetWinnerByConcession(WinnerPPHandle As String)
        If Model.CurrentGame.Player1.PPHandle & " (" & Model.CurrentGame.Player1.Name & ")" = WinnerPPHandle Then
            Model.CurrentGame.Player1.ControlPoints = 3
        ElseIf Model.CurrentGame.Player2 IsNot Nothing AndAlso Model.CurrentGame.Player2.PPHandle & " (" & Model.CurrentGame.Player2.Name & ")" = WinnerPPHandle Then
            Model.CurrentGame.Player2.ControlPoints = 3
        Else
            MessageBox.Show("Something bad just happened. Concession winner is not a member of this game!")
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
                Model.CurrentGame.Condition = "Bye"
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

        Dim SortedGames = (From p In Model.CurrentRound.Games Order By p.Reported, p.TableNumber).ToList
        Model.CurrentRound.Games.Clear()
        Model.CurrentRound.Games.AddRange(SortedGames)
        CType(Me.View, Games).ForceUpdate()
    End Sub

    Public Sub SelectGame(Game As doGame)
        Model.CurrentGame = Game
        If Game.Winner = Game.Player1.PPHandle Then Game.Winner = Game.Player1.PPHandle & " (" & Game.Player1.Name & ")"
        If Game.Player2 IsNot Nothing AndAlso Game.Winner = Game.Player2.PPHandle Then Game.Winner = Game.Player2.PPHandle & " (" & Game.Player2.Name & ")"

        OnPropertyChanged("AcceptableWinners")
    End Sub


    Public Function AcceptGame() As String
        Dim sReturn = String.Empty
        If Model.CurrentGame.Winner = String.Empty Then sReturn = "Winner not set" & ControlChars.CrLf
        If Model.CurrentGame.Condition = String.Empty Then sReturn &= "Condition not set"
        If sReturn = String.Empty Then
            If BaseController.Model.CurrentGame IsNot Nothing Then
                Dim q = (From p In Model.WMEvent.Players Where p.PPHandle = Model.CurrentGame.Player1.PPHandle Select p).FirstOrDefault
                If Not q Is Nothing Then q.Tables.Add(Model.CurrentGame.TableNumber)

                If Not Model.CurrentGame.Player2 Is Nothing Then
                    Model.CurrentGame.Player1.Opponents.Add(Model.CurrentGame.Player2.PPHandle)
                    Model.CurrentGame.Player2.Opponents.Add(Model.CurrentGame.Player1.PPHandle)
                    q = (From p In Model.WMEvent.Players Where p.PPHandle = Model.CurrentGame.Player2.PPHandle Select p).FirstOrDefault
                    If Not q Is Nothing Then q.Tables.Add(Model.CurrentGame.TableNumber)
                End If

                Model.CurrentGame.Reported = True
                Model.Save()

                Dim SortedGames = (From p In Model.CurrentRound.Games Order By p.Reported, p.TableNumber).ToList
                Model.CurrentRound.Games.Clear()
                Model.CurrentRound.Games.AddRange(SortedGames)

                Dim RemainingGames = From p In Model.CurrentRound.Games Where p.Reported = False

                If RemainingGames.Count = 0 Then
                    Dim q2 = From p In Model.CurrentRoundPlayers Where p.TourneyPoints = Model.CurrentRound.RoundNumber

                    If q2.Count < 2 Then
                        Model.CurrentRound.IsLastRound = True
                    End If
                End If
            End If
        End If
        Return sReturn
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
