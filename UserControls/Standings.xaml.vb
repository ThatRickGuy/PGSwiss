Public Class Standings

    Private Sub btnPrintStandings_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintStandings.Click
        CType(BaseController.CurrentController, StandingsController).PrintStandings(False)
    End Sub

    Private Sub btnUploadStandings_Click(sender As Object, e As RoutedEventArgs) Handles btnUploadStandings.Click
        CType(BaseController.CurrentController, StandingsController).PrintStandings(True)
    End Sub
End Class
