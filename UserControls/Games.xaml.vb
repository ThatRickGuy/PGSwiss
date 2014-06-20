Public Class Games

    Private Sub dgGames_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgGames.SelectionChanged
        If Not dgGames.CurrentItem Is Nothing Then CType(BaseController.CurrentController, GamesController).SelectGame(dgGames.CurrentItem)
    End Sub

    Private Sub btnAcceptGame_Click(sender As Object, e As RoutedEventArgs) Handles btnAcceptGame.Click
        CType(BaseController.CurrentController, GamesController).AcceptGame()
        Me.dgGames.SelectedItem = Nothing
        Me.dgGames.Items.Refresh()
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
