Imports System.ComponentModel

Public Class WMEventViewModel
    Inherits BaseViewModel
    Implements INotifyPropertyChanged

    Private WithEvents _WMEvent As New doWMEvent
    Public Property WMEvent As doWMEvent
        Get
            Return _WMEvent
        End Get
        Set(value As doWMEvent)
            _WMEvent = Value
        End Set
    End Property
    Public Property Factions As New doFactionCollection
    Public Property AllPlayers As New doPlayerCollection
    Public Property Scenarios As New doScenarioCollection
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
            If WMEvent IsNot Nothing AndAlso WMEvent.Name <> String.Empty Then
                sReturn = WMEvent.Name
            End If
            Return sReturn
        End Get
    End Property

    Public Sub New()
        Factions.load()
        Scenarios.load()
        Metas.load()
        AllPlayers.load()
        Formats.load()
        Sizes.load()
    End Sub

    Public Overrides Sub Save()
        Me.WMEvent.Save()

        For Each p As doPlayer In WMEvent.Players
            Dim q = From i In AllPlayers Where i.PPHandle = p.PPHandle
            If q.Count = 0 Then AllPlayers.Add(p)
        Next
        AllPlayers.Save()
        Metas.Save()
    End Sub

    Public Overrides Sub Load()
        Me.WMEvent.load()
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
End Class
