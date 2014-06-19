Class MainWindow 
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim WMEvent As New WMEventController
        BaseController.Model.Load()

        UpdateUI()
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        BaseController.CurrentController.MoveNext()
        UpdateUI()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        BaseController.CurrentController.MovePrev()
        UpdateUI()
    End Sub

    Private Sub UpdateUI()
        grdContent.Children.Clear()
        grdContent.Children.Add(BaseController.CurrentController.View)
        Me.btnNext.IsEnabled = BaseController.CurrentController.NextEnabled
        Me.btnPrev.IsEnabled = BaseController.CurrentController.PreviousEnabled
        Me.Header.txtTitle.Text = "PG Swiss" 'BaseController.CurrentController.Title
    End Sub
End Class
