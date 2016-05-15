Imports System.Text
Imports System.Drawing.Drawing2D

Public Class Pairings

    Private Sub btnGeneratePairing_Click(sender As Object, e As RoutedEventArgs) Handles btnGeneratePairing.Click
        If MessageBox.Show("Regenerating pairings will clear and repair all games, click OK to continue.", "Regenerate Pairings", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then
            Dim result = CType(BaseController.CurrentController, PairingsController).GeneratePairings()
            If result = String.Empty Then
                Me.dgPairings.Items.Refresh()
            Else
                MessageBox.Show(result)
            End If
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

<Flags> _
Public Enum GameCondition
    SameMeta = 1
    ReusedTable = 2
    AlreadyPlayed = 4
    Pairdown = 8
End Enum


Public Class IntToBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.Convert


        Dim brushReturn As SolidColorBrush = Nothing
        Dim bgColor As Color = Colors.Transparent
        If value IsNot Nothing AndAlso value > 0 Then
            If (value And GameCondition.SameMeta) = GameCondition.SameMeta Then
                bgColor = Colors.Yellow
            End If
            If (value And GameCondition.ReusedTable) = GameCondition.ReusedTable Then
                bgColor = Colors.Orange
            End If
            If (value And GameCondition.Pairdown) = GameCondition.Pairdown Then
                bgColor = Colors.LightGray
            End If
            If (value And GameCondition.AlreadyPlayed) = GameCondition.AlreadyPlayed Then
                bgColor = Colors.Red
            End If
        End If
        brushReturn = New SolidColorBrush(bgColor)
        Return brushReturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.ConvertBack
        Return Nothing 'don't care!
    End Function
End Class

