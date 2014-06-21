Imports System.Text

Public Class Pairings

    Private Sub btnGeneratePairing_Click(sender As Object, e As RoutedEventArgs) Handles btnGeneratePairing.Click
        btnGeneratePairing.Content = "Regenerate Pairings"

        CType(BaseController.CurrentController, PairingsController).GeneratePairings()
        Me.dgPairings.Items.Refresh()
    End Sub

    Private Sub btnPrintPairing_Click(sender As Object, e As RoutedEventArgs) Handles btnPrintPairing.Click
        If BaseController.Model.CurrentRound.Games.Count > 0 Then
            Dim sbOutput As New StringBuilder
            sbOutput.Append(My.Resources.HTMLHeader)
            sbOutput.AppendFormat(My.Resources.Header, BaseController.Model.WMEvent.Name, BaseController.Model.CurrentRound.RoundNumber)

            Dim sbLeftBlock As New StringBuilder()
            Dim sbRightBlock As New StringBuilder()

            Dim PlayerGames As New List(Of PlayerGame)
            For Each player In (From p In BaseController.Model.CurrentRound.Players Order By p.Name)
                Dim game = (From p In BaseController.Model.CurrentRound.Games Where p.Player1.PPHandle = player.PPHandle Or (p.Player2 IsNot Nothing AndAlso p.Player2.PPHandle = player.PPHandle)).FirstOrDefault

                Dim pg As PlayerGame = Nothing
                If game.Player2 Is Nothing Then
                    'bye
                    pg.Player = game.Player1.Name
                    pg.Table = "-"
                    pg.Opponent = "Bye"
                ElseIf game.Player1.PPHandle = player.PPHandle Then
                    pg.Player = game.Player1.Name
                    pg.Table = game.TableNumber
                    pg.Opponent = game.Player2.Name
                ElseIf game.Player2.PPHandle = player.PPHandle Then
                    pg.Player = game.Player2.Name
                    pg.Table = game.TableNumber
                    pg.Opponent = game.Player1.Name
                End If
                PlayerGames.Add(pg)
            Next

            For i As Integer = 0 To Math.Ceiling(PlayerGames.Count / 2) - 1
                sbLeftBlock.AppendFormat(My.Resources.Item, New String() {PlayerGames(i).Player, PlayerGames(i).Table, PlayerGames(i).Opponent})
                sbRightBlock.AppendFormat(My.Resources.Item, New String() {PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Player, PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Table, PlayerGames(i + Math.Floor(PlayerGames.Count / 2)).Opponent})
            Next

            sbOutput.AppendFormat(My.Resources.LeftColumn, sbLeftBlock.ToString)
            sbOutput.AppendFormat(My.Resources.RightColumn, sbRightBlock.ToString)
            sbOutput.Append(My.Resources.Footer)


            IO.File.WriteAllText(".\PairingsList.html", sbOutput.ToString)
            Process.Start(".\PairingsList.html")
        End If
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

Public Structure PlayerGame
    Public Player As String
    Public Table As Integer
    Public Opponent As String
End Structure