Public Class ReportingController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return Nothing
    End Function

    Public Sub New()
        _NextEnabled = False
        Me.View = New Reporting
        Me._Title = "Event Reporting"
    End Sub
End Class
