Imports System.ComponentModel

Public Class doRound
    Implements INotifyPropertyChanged

    Public Property RoundNumber As Integer
    Public Property Scenario As String
    Public Property ByeVolunteers As New doPlayerCollection

    Private _Games As New doGameCollection
    Public Property Games As doGameCollection
        Get
            Return _Games
        End Get
        Set(value As doGameCollection)
            _Games = value
            OnPropertyChanged("Games")
        End Set
    End Property
    Public Property Bye As doPlayer
    Public Property Size As Integer

    Private _IsLastRound As Boolean
    Public Property IsLastRound As Boolean
        Get
            Return _IsLastRound
        End Get
        Set(value As Boolean)
            _IsLastRound = value
            OnPropertyChanged("IsLastRound")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Function GetPlayers(WMEvent As doWMEvent) As List(Of doPlayer)
        Dim PlayerList As New List(Of doPlayer)
        For Each player In WMEvent.Players
            PlayerList.Add(player.Clone)
        Next

        For Each round In (From p In WMEvent.Rounds Where p.RoundNumber <= Me.RoundNumber)
            For Each game In round.Games
                Dim player = (From p In PlayerList Where p.PPHandle = game.Player1.PPHandle).FirstOrDefault
                If Not player Is Nothing Then
                    player.ArmyPointsDestroyed += game.Player1.ArmyPointsDestroyed
                    player.ControlPoints += game.Player1.ControlPoints
                    If game.Winner = player.PPHandle Then player.TourneyPoints += 1
                End If
                player = (From p In PlayerList Where Not game.Player2 Is Nothing AndAlso p.PPHandle = game.Player2.PPHandle).FirstOrDefault
                If Not player Is Nothing Then
                    player.ArmyPointsDestroyed += game.Player2.ArmyPointsDestroyed
                    player.ControlPoints += game.Player2.ControlPoints
                    If game.Winner = player.PPHandle Then player.TourneyPoints += 1
                End If
            Next

            'again, for pairdowns
            For Each game In round.Games
                If Not game.Player2 Is Nothing Then
                    If game.Player1.TourneyPoints > game.Player2.TourneyPoints Then
                        Dim player = (From p In PlayerList Where p.PPHandle = game.Player1.PPHandle).FirstOrDefault
                        If Not player Is Nothing Then
                            player.HasBeenPairedDown = True
                        End If
                    ElseIf game.Player1.TourneyPoints < game.Player2.TourneyPoints Then
                        Dim player = (From p In PlayerList Where p.PPHandle = game.Player2.PPHandle).FirstOrDefault
                        If Not player Is Nothing Then
                            player.HasBeenPairedDown = True
                        End If
                    End If
                End If
            Next
        Next

        'SOS
        For Each player In PlayerList
            Dim Opponents = (From p In PlayerList Where player.Opponents IsNot Nothing AndAlso player.Opponents.Contains(p.PPHandle))
            For Each Opponent In Opponents
                player.StrengthOfSchedule += Opponent.TourneyPoints
            Next
        Next

        'drops
        'todo: late arrivals check (should have no pairings at all!)
        Dim LastReportedRound As doRound = Nothing
        For Each round In WMEvent.Rounds
            If round.Games.Count > 0 AndAlso (From p In round.Games Where p.Reported = False).Count = 0 Then LastReportedRound = round
        Next
        If Not LastReportedRound Is Nothing Then
            For Each player In PlayerList
                player.Drop = True
            Next
            For Each game In LastReportedRound.Games
                Dim player = (From p In PlayerList Where p.PPHandle = game.Player1.PPHandle).FirstOrDefault
                If Not player Is Nothing Then
                    player.Drop = False
                End If
                player = (From p In PlayerList Where Not game.Player2 Is Nothing AndAlso p.PPHandle = game.Player2.PPHandle).FirstOrDefault
                If Not player Is Nothing Then
                    player.Drop = False
                End If
            Next
        End If

        Dim rank As Integer = 1
        For Each player In (From p In PlayerList Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending)
            player.Rank = rank
            rank += 1
        Next

        'bye volunteers
        For Each byeVolunteer In Me.ByeVolunteers
            Dim player = (From p In PlayerList Where p.PPHandle = byeVolunteer.PPHandle).FirstOrDefault
            If Not player Is Nothing Then player.ByeVol = True
        Next

        Return PlayerList
    End Function

End Class
