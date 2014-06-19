Public Class Pairings

    Private Sub btnGeneratePairing_Click(sender As Object, e As RoutedEventArgs) Handles btnGeneratePairing.Click
        btnGeneratePairing.Content = "Regenerate Pairings"

        CType(BaseController.CurrentController, PairingsController).GeneratePairings()
        Me.dgPairings.Items.Refresh()
    End Sub

    Private Sub btnPrintPairing_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintPairing.Click

    End Sub

    Private Sub btnSwap_Click(sender As Object, e As RoutedEventArgs) Handles btnSwap.Click
        If cboSwapPlayerq1.SelectedItem IsNot Nothing AndAlso cboSwapPlayerq2.SelectedItem IsNot Nothing Then
            CType(BaseController.CurrentController, PairingsController).SwapPlayers(cboSwapPlayerq1.SelectedValue, cboSwapPlayerq2.SelectedValue)
           
            Me.cboSwapPlayerq1.SelectedItem = Nothing
            Me.cboSwapPlayerq2.SelectedItem = Nothing
            Me.dgPairings.Items.Refresh()
        End If
    End Sub
End Class
