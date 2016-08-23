Imports System.ComponentModel

Public Class DirtyMonitor
    Implements INotifyPropertyChanged

    Private Shared _IsDirty = False
    Public Shared Property IsDirty
        Get
            Return _IsDirty
        End Get
        Set(value)
            If Not (_IsDirty AndAlso value) Then
                _IsDirty = value
                GetSingleton.OnPropertyChanged("IsDirty")
            End If
        End Set
    End Property

    Private Shared WithEvents _SingletonInstance As New DirtyMonitor
    Public Shared Function GetSingleton() As DirtyMonitor
        If _SingletonInstance Is Nothing Then _SingletonInstance = New DirtyMonitor
        Return _SingletonInstance
    End Function

    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
