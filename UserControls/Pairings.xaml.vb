Imports System.Text

Public Class Pairings

    Private Sub btnGeneratePairing_Click(sender As Object, e As RoutedEventArgs) Handles btnGeneratePairing.Click
        Dim result = CType(BaseController.CurrentController, PairingsController).GeneratePairings()
        If result = String.Empty Then
            Me.dgPairings.Items.Refresh()
        Else
            MessageBox.Show(result)
        End If
    End Sub

    Private Sub btnPrintPairing_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintPairing.Click
        CType(BaseController.CurrentController, PairingsController).PrintPairingsAlphaBetical()
    End Sub

    Private Sub btnPrintTables_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintTables.Click
        CType(BaseController.CurrentController, PairingsController).PrintPairingsByTableNumber()
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


Public Class ColorToSolidBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.Convert
        Dim brushReturn As SolidColorBrush = Nothing
        If value IsNot Nothing Then brushReturn = New SolidColorBrush(value)
        Return brushReturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.ConvertBack
        Return Nothing 'don't care!
    End Function
End Class