Imports System.ComponentModel

Public Class doRound
    Implements INotifyPropertyChanged


    Public Property RoundNumber As Integer
    Public Property Scenario As String
    Public Property Players As New doPlayerCollection

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
End Class
