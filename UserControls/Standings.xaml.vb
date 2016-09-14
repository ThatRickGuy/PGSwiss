Public Class Standings

    Private Sub btnPrintStandings_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintStandings.Click
        CType(BaseController.CurrentController, StandingsController).PrintStandings(False)
    End Sub

    Private Sub btnUploadStandings_Click(sender As Object, e As RoutedEventArgs) Handles btnUploadStandings.Click
        CType(BaseController.CurrentController, StandingsController).PrintStandings(True)
    End Sub

    'Public ReadOnly Property Players As List(Of PGSwiss.Data.doPlayer)
    '    Get
    '        Return BaseViewModel
    '    End Get
    'End Property


    Private Sub cboBestPaintWinner_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cboBestPaintWinner.SelectionChanged, cboBestSportWinner.SelectionChanged
        PGSwiss.Data.DirtyMonitor.IsDirty = True
    End Sub
End Class
