Imports System.ComponentModel

Public Class WMEventViewModel
    Inherits BaseViewModel
    Implements INotifyPropertyChanged

    Public Property WMEvent As New doWMEvent
    Public Property Factions As New doFactionCollection
    Public Property Players As New doPlayerCollection
    Public Property Scenarios As New doScenarioCollection
    Public Property Metas As New doMetaCollection
    Public Property Formats As New doEventFormatCollection

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

    Public Sub New()
        Factions.load()
        Scenarios.load()
        Metas.load()
        Players.load()
        Formats.load()
    End Sub

    Public Overrides Sub Save()
        Me.WMEvent.Save()

        For Each p As doPlayer In WMEvent.Players
            Dim q = From i In Players Where i.PPHandle = p.PPHandle
            If q.Count = 0 Then Players.Add(p)
        Next
        Players.Save()
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
End Class
