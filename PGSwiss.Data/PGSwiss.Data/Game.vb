Imports System.ComponentModel

Public Class doGame
    Implements INotifyPropertyChanged


    Public Property GameID As Guid
    Public Property Player1 As doPlayer
    Public Property Player2 As doPlayer
    Public Property Winner As String
    Public Property Condition As String
    Public Property TableNumber As Integer
    Private _Reported As Boolean = False
    Public Property Reported As Boolean
        Get
            Return _Reported
        End Get
        Set(value As Boolean)
            _Reported = value
            OnPropertyChanged("Reported")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class doGameCollection
    Inherits List(Of doGame)
End Class
