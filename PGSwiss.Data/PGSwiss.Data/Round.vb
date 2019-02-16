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

    Protected Sub OnPropertyChanged(ByVal name As String)
        DirtyMonitor.IsDirty = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Function GetPlayers(WMEvent As doWMEvent) As List(Of doPlayer)
        Dim PlayerList As New List(Of doPlayer)
        For Each player In WMEvent.Players
            PlayerList.Add(player.Clone)
        Next

        For Each round In (From p In WMEvent.Rounds Where p.RoundNumber <= Me.RoundNumber)
            For Each game In round.Games
                Dim player = (From p In PlayerList Where p.Name = game.Player1.Name).FirstOrDefault
                If Not player Is Nothing Then
                    player.ArmyPointsDestroyed += game.Player1.ArmyPointsDestroyed
                    player.ControlPoints += game.Player1.ControlPoints
                    If game.Winner = player.Name Then player.TourneyPoints += 1
                End If
                player = (From p In PlayerList Where Not game.Player2 Is Nothing AndAlso p.Name = game.Player2.Name).FirstOrDefault
                If Not player Is Nothing Then
                    player.ArmyPointsDestroyed += game.Player2.ArmyPointsDestroyed
                    player.ControlPoints += game.Player2.ControlPoints
                    If game.Winner = player.Name Then player.TourneyPoints += 1
                End If
            Next

            'again, for pairdowns
            For Each game In round.Games
                If Not game.Player2 Is Nothing Then

                    Dim player = (From p In PlayerList Where p.Name = game.Player1.Name).FirstOrDefault
                    If Not player Is Nothing Then
                        If Not player.Opponents.Contains(game.Player2.Name) Then player.Opponents.Add(game.Player2.Name)
                        If Not player.Tables.Contains(game.TableNumber) Then player.Tables.Add(game.TableNumber)
                        If game.Player1.TourneyPoints > game.Player2.TourneyPoints Then player.PairedDownRound = round.RoundNumber
                    End If

                    player = (From p In PlayerList Where p.Name = game.Player2.Name).FirstOrDefault
                    If Not player Is Nothing Then
                        'player.PairedDownRound = round.RoundNumber
                        If Not player.Opponents.Contains(game.Player1.Name) Then player.Opponents.Add(game.Player1.Name)
                        If Not player.Tables.Contains(game.TableNumber) Then player.Tables.Add(game.TableNumber)
                        If game.Player1.TourneyPoints < game.Player2.TourneyPoints Then player.PairedDownRound = round.RoundNumber
                    End If
                End If
            Next
        Next

        'SOS
        For Each player In PlayerList
            Dim Opponents = (From p In PlayerList Where player.Opponents IsNot Nothing AndAlso player.Opponents.Contains(p.Name))
            For Each Opponent In Opponents
                player.StrengthOfSchedule += Opponent.TourneyPoints
            Next
        Next

        'drops
        For Each player In PlayerList
            Dim HasPlayed = False

            For Each round In WMEvent.Rounds
                Dim q = From p In PlayerList Where p.Name = player.Name

                'Check to see if the player has played a game. If they have, they could be a drop, if they aren't, they could be a late arrival
                If q IsNot Nothing Then
                    HasPlayed = True
                End If

                If round.Games.Count > 0 AndAlso (From p In round.Games Where p.Reported = False).Count = 0 Then
                    If q Is Nothing Then
                        player.Drop = True
                    End If
                End If

            Next
        Next

        Dim rank As Integer = 1
        For Each player In (From p In PlayerList Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending)
            player.Rank = rank
            rank += 1
        Next

        'bye volunteers
        For Each byeVolunteer In Me.ByeVolunteers
            Dim player = (From p In PlayerList Where p.Name = byeVolunteer.Name).FirstOrDefault
            If Not player Is Nothing Then player.ByeVol = True
        Next

        Return PlayerList
    End Function

End Class
