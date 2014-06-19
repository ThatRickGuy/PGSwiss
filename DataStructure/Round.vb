Imports System.ComponentModel

Public Class doRound
    Implements INotifyPropertyChanged


    Public Property RoundNumber As Integer
    Public Property Scenario As doScenario
    Public Property Players As New doPlayerCollection
    Public Property Games As New doGameCollection
    Public Property Bye As doPlayer

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
