Imports Microsoft.Win32
Imports System.Reflection

Public Class Games

    Private Sub dgGames_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgGames.SelectionChanged
        If Not dgGames.CurrentItem Is Nothing Then
            CType(BaseController.CurrentController, GamesController).SelectGame(dgGames.CurrentItem)
        End If

        If BaseController.Model.CurrentGame.Player1 IsNot Nothing AndAlso BaseController.Model.CurrentRound.Games.DropNextRound.Contains(BaseController.Model.CurrentGame.Player1.Name) Then
            Me.chkPlayer1Drop.IsChecked = True
        Else
            Me.chkPlayer1Drop.IsChecked = False
        End If
        If BaseController.Model.CurrentGame.Player2 IsNot Nothing AndAlso BaseController.Model.CurrentRound.Games.DropNextRound.Contains(BaseController.Model.CurrentGame.Player2.Name) Then
            Me.chkPlayer2Drop.IsChecked = True
        Else
            Me.chkPlayer2Drop.IsChecked = False
        End If
    End Sub

    Private Sub btnAcceptGame_Click(sender As Object, e As RoutedEventArgs) Handles btnAcceptGame.Click
        Dim sAccepted = CType(BaseController.CurrentController, GamesController).AcceptGame()
        If sAccepted = String.Empty Then
            Me.dgGames.SelectedItem = Nothing
            Me.dgGames.Items.Refresh()
            PGSwiss.Data.DirtyMonitor.IsDirty = True
        Else
            MessageBox.Show("Please resolve the issue(s) listed below prior to accepting a game:" & ControlChars.CrLf & ControlChars.CrLf & sAccepted)
        End If
    End Sub

    Private Sub PointsBoxes_GotFocus(sender As Object, e As RoutedEventArgs) Handles txtPlayer1CP.GotFocus, txtPlayer1APD.GotFocus, txtPlayer2CP.GotFocus, txtPlayer2APD.GotFocus
        CType(sender, TextBox).SelectAll()
    End Sub

    Private Sub cboCondition_SelectionChangeCommitted(sender As Object, e As SelectionChangedEventArgs) Handles cboCondition.SelectionChanged
        If BaseController.Model.CurrentGame IsNot Nothing AndAlso
           BaseController.Model.CurrentGame.Winner <> String.Empty AndAlso
           BaseController.Model.CurrentGame.Winner <> "Draw" AndAlso
           BaseController.CurrentController.GetType Is GetType(GamesController) AndAlso
           BaseController.Model.CurrentGame.Player1.ControlPoints = 0 AndAlso
           BaseController.Model.CurrentGame.Player2.ControlPoints = 0 Then
            If CType(e.AddedItems(0), ComboBoxItem).Content = "Scenario" Then CType(BaseController.CurrentController, GamesController).SetWinnerByScenario(BaseController.Model.CurrentGame.Winner)
            If CType(e.AddedItems(0), ComboBoxItem).Content = "Concession" Then CType(BaseController.CurrentController, GamesController).SetWinnerByConcession(BaseController.Model.CurrentGame.Winner)
            If CType(e.AddedItems(0), ComboBoxItem).Content = "Disqualification" Then CType(BaseController.CurrentController, GamesController).SetWinnerByConcession(BaseController.Model.CurrentGame.Winner)
        End If
    End Sub

    Public Sub ForceUpdate()
        Me.dgGames.Items.Refresh()
    End Sub

    Private Sub chkPlayer2Drop_Checked(sender As Object, e As RoutedEventArgs)
        Dim PlayerName = BaseController.Model.CurrentGame.Player2.Name

        If chkPlayer2Drop.IsChecked Then BaseController.Model.CurrentRound.Games.DropNextRound.Add(PlayerName)
        If Not chkPlayer2Drop.IsChecked Then BaseController.Model.CurrentRound.Games.DropNextRound.Remove(PlayerName)
    End Sub

    Private Sub chkPlayer1Drop_Checked(sender As Object, e As RoutedEventArgs)
        Dim PlayerName = BaseController.Model.CurrentGame.Player1.Name

        If chkPlayer1Drop.IsChecked Then BaseController.Model.CurrentRound.Games.DropNextRound.Add(PlayerName)
        If Not chkPlayer1Drop.IsChecked Then BaseController.Model.CurrentRound.Games.DropNextRound.Remove(PlayerName)
    End Sub

    Public Property DropNextRoundCollection As New List(Of String)

End Class


Public Class BoolToColorConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim cReturn As SolidColorBrush = New SolidColorBrush(Colors.White)
        If values(0) = True AndAlso (values(1) And 8) = 8 Then
            'reported pairdown
            cReturn = New SolidColorBrush(Colors.LimeGreen)
        ElseIf values(0) = True Then
            'reported
            cReturn = New SolidColorBrush(Colors.LightGreen)
        ElseIf (values(1) And 8) = 8 Then
            'unreported pairdown
            cReturn = New SolidColorBrush(Colors.LightGray)
        Else
            'unreported
        End If
        Return cReturn
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'don't care
        Return Nothing
    End Function


End Class



