Imports System.ComponentModel

Public Class doGame
    Implements INotifyPropertyChanged
    Implements IGame


    Public Property GameID As Guid Implements IGame.GameID
    Public Property Player1 As doPlayer Implements IGame.Player1
    Public Property Player2 As doPlayer Implements IGame.Player2
    Public Property Winner As String Implements IGame.Winner
    Public Property Condition As String Implements IGame.Condition
    Public Property TableNumber As Integer Implements IGame.TableNumber
    Private _Reported As Boolean = False
    Public Property Reported As Boolean Implements IGame.Reported
        Get
            Return _Reported
        End Get
        Set(value As Boolean)
            _Reported = value
            OnPropertyChanged("Reported")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged, IGame.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Public Class doGameCollection
    Inherits List(Of doGame)
End Class
