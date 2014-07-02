Public Class Games

    Private Sub dgGames_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgGames.SelectionChanged
        If Not dgGames.CurrentItem Is Nothing Then
            CType(BaseController.CurrentController, GamesController).SelectGame(dgGames.CurrentItem)
        End If
    End Sub

    Private Sub btnAcceptGame_Click(sender As Object, e As RoutedEventArgs) Handles btnAcceptGame.Click
        Dim sAccepted = CType(BaseController.CurrentController, GamesController).AcceptGame()
        If sAccepted = String.Empty Then
            Me.dgGames.SelectedItem = Nothing
            Me.dgGames.Items.Refresh()
        Else
            MessageBox.Show("Please resolve the issue(s) listed below prior to accepting a game:" & ControlChars.CrLf & ControlChars.CrLf & sAccepted)
        End If
    End Sub

    Private Sub PointsBoxes_GotFocus(sender As Object, e As RoutedEventArgs) Handles txtPlayer1APD.GotFocus, txtPlayer1CP.GotFocus, txtPlayer2APD.GotFocus, txtPlayer2CP.GotFocus
        CType(sender, TextBox).SelectAll()
    End Sub

    Private Sub cboCondition_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cboCondition.SelectionChanged
        If BaseController.Model.CurrentGame IsNot Nothing AndAlso BaseController.Model.CurrentGame.Winner <> String.Empty AndAlso BaseController.Model.CurrentGame.Winner <> "Draw" Then
            If CType(e.AddedItems(0), ComboBoxItem).Content = "Scenario" Then CType(BaseController.CurrentController, GamesController).SetWinnerByScenario(BaseController.Model.CurrentGame.Winner)
        End If
    End Sub
End Class

Public Class BoolToColorConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim cReturn As SolidColorBrush = New SolidColorBrush(Colors.White)
        If CType(value, Boolean) = True Then cReturn = New SolidColorBrush(Colors.LightGreen)
        Return cReturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim bReturn As Boolean = True
        If CType(value, SolidColorBrush).Color = Colors.White Then bReturn = False
        Return bReturn
    End Function
End Class
