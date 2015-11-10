Imports System.ComponentModel
Imports PGSwiss.Data

Public Class WMEventViewModel
    Inherits BaseViewModel
    Implements INotifyPropertyChanged

    Public Overrides Sub Load(FileName As String)
        _WMEvent = New doWMEvent(FileName)

        OnPropertyChanged("Title")
    End Sub

    Private Shared _SingletonInstance As WMEventViewModel

    Public Shared Function GetSingleton() As WMEventViewModel
        If _SingletonInstance Is Nothing Then
            _SingletonInstance = New WMEventViewModel
        End If
        Return _SingletonInstance
    End Function

    Private _CurrentProgress As Integer
    Public Property CurrentProgress As Integer
        Get
            Return _CurrentProgress
        End Get
        Set(value As Integer)
            _CurrentProgress = value
            OnPropertyChanged("CurrentProgress")
        End Set
    End Property

    Private Sub New()
        Factions.load()
        Metas.load()
        AllPlayers.load()
        Formats.load()
        Sizes.load()
    End Sub

    Private WithEvents _WMEvent As doWMEvent
    Public Property WMEvent As doWMEvent
        Get
            Return _WMEvent
        End Get
        Set(value As doWMEvent)
            _WMEvent = value
        End Set
    End Property

    Public Property Factions As New doFactionCollection
    Public Property AllPlayers As New doPlayerCollection
    Public Property Metas As New doMetaCollection
    Public Property Formats As New doEventFormatCollection
    Public Property Sizes As New doRoundSizeCollection

    Private _CurrentRound As doRound
    Public Property CurrentRound As doRound
        Get
            Return _CurrentRound
        End Get
        Set(value As doRound)
            _CurrentRound = value
            OnPropertyChanged("CurrentRound")
        End Set
    End Property

    Private _LastRoundRetrieved As Integer
    Private _LastGamesCountRetrived As Integer
    Private _cachedRoundPlayers As List(Of doPlayer)
    Public ReadOnly Property CurrentRoundPlayers As List(Of doPlayer)
        Get
            If _LastRoundRetrieved <> _CurrentRound.RoundNumber OrElse _LastGamesCountRetrived <> (From p In _CurrentRound.Games Where p.Reported = True).Count Then
                _cachedRoundPlayers = _CurrentRound.GetPlayers(_WMEvent)
                _LastRoundRetrieved = _CurrentRound.RoundNumber
                _LastGamesCountRetrived = (From p In _CurrentRound.Games Where p.Reported = True).Count
            End If
            Return _cachedRoundPlayers
        End Get
    End Property

    Public Function PlayerOpponents(Player As doPlayer) As List(Of doPlayer)
        Dim oReturn As New List(Of doPlayer)
        For Each Round In WMEvent.Rounds
            Dim game = (From p In Round.Games Where p.Player1.PPHandle = Player.PPHandle Or p.Player2.PPHandle = Player.PPHandle).FirstOrDefault
            If Not game Is Nothing Then
                If game.Player1.PPHandle = Player.PPHandle Then
                    If Not game.Player2 Is Nothing Then oReturn.Add(game.Player2)
                Else
                    If Not game.Player1 Is Nothing Then oReturn.Add(game.Player1)
                End If
            End If
        Next
        Return oReturn
    End Function

    Public Function PlayerTables(Player As doPlayer) As List(Of Integer)
        Dim ilReturn As New List(Of Integer)
        For Each Round In WMEvent.Rounds
            Dim game = (From p In Round.Games Where p.Player1.PPHandle = Player.PPHandle Or p.Player2.PPHandle = Player.PPHandle).FirstOrDefault
            If Not game Is Nothing Then
                ilReturn.Add(game.TableNumber)
            End If
        Next
        Return ilReturn
    End Function


    Private _CurrentGame As doGame
    Public Property CurrentGame As doGame
        Get
            Return _CurrentGame
        End Get
        Set(value As doGame)
            _CurrentGame = value
            OnPropertyChanged("CurrentGame")
        End Set
    End Property

    Public ReadOnly Property Title As String
        Get
            Dim sReturn As String = "PG Swiss"
            If _WMEvent IsNot Nothing Then sReturn = _WMEvent.Name
            Return sReturn
        End Get
    End Property

    Public Overrides Sub Save()
        If Me.WMEvent IsNot Nothing Then
            Me.WMEvent.Save()

            For Each p As doPlayer In WMEvent.Players
                Dim q = From i In AllPlayers Where i.PPHandle = p.PPHandle
                If q.Count = 0 Then AllPlayers.Add(p)
            Next
        End If
        Factions.Save()
        Metas.Save()
        AllPlayers.Save()
        Formats.Save()
        Sizes.Save()
    End Sub


    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Private Sub _WMEvent_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _WMEvent.PropertyChanged
        If e.PropertyName = "Name" Then
            OnPropertyChanged("Title")
        End If
    End Sub

    Public Sub NotifyButtonVisibilityChange()
        OnPropertyChanged("PreviousButtonVisibility")
        OnPropertyChanged("NextButtonVisibility")
    End Sub

    Public ReadOnly Property PreviousButtonVisibility As Visibility
        Get
            Dim vReturn As Visibility = Visibility.Collapsed
            If BaseController.CurrentController.PreviousEnabled Then vReturn = Visibility.Visible
            Return vReturn
        End Get
    End Property

    Public ReadOnly Property NextButtonVisibility As Visibility
        Get
            Dim vReturn As Visibility = Visibility.Collapsed
            If BaseController.CurrentController.NextEnabled Then vReturn = Visibility.Visible
            Return vReturn
        End Get
    End Property
End Class
