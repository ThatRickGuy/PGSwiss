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
