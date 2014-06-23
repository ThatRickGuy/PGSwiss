Imports System.ComponentModel

Public Class doRound
    Implements INotifyPropertyChanged
    Implements IRound


    Public Property RoundNumber As Integer Implements IRound.RoundNumber
    Public Property Scenario As String Implements IRound.Scenario
    Public Property Players As New doPlayerCollection Implements IRound.Players
    Public Property Games As New doGameCollection Implements IRound.Games
    Public Property Bye As doPlayer Implements IRound.Bye
    Public Property Size As Integer Implements IRound.Size
    Private _IsLastRound As Boolean
    Public Property IsLastRound As Boolean Implements IRound.IsLastRound
        Get
            Return _IsLastRound
        End Get
        Set(value As Boolean)
            _IsLastRound = value
            OnPropertyChanged("IsLastRound")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged, IRound.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class
