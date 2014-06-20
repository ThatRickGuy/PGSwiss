Public Class Round

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub cboFormat_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cboFormat.SelectionChanged
        'MessageBox.Show(BaseController.Model.CurrentRound.Players.ToString)
    End Sub
End Class
