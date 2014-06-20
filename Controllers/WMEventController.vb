Public Class WMEventController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New RoundController
    End Function

    Public Sub New()
        _Stack.Add(Me)
        _PreviousEnabled = False
        Me.View = New WMEvent
        Me.View.DataContext = Me
        Model = New WMEventViewModel

    End Sub

    
End Class
