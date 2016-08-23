Imports PGSwiss.Data

Public Class Round

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub



    Private Sub cboName_LostFocus_1(sender As Object, e As RoutedEventArgs)
        'Name combobox lost focus
        Dim AllPlayers = BaseController.Model.AllPlayers
        Dim EventPlayers = BaseController.Model.WMEvent.Players
        Dim cbo = CType(sender, ComboBox)
        Dim CurrentItem As doPlayer = Nothing
        If dgPlayers.CurrentItem IsNot Nothing AndAlso dgPlayers.CurrentItem.GetType Is GetType(doPlayer) Then CurrentItem = CType(dgPlayers.CurrentItem, doPlayer)

        Dim Player = (From p In AllPlayers Where p.Name = cbo.Text).FirstOrDefault
        If Not CurrentItem Is Nothing Then
            If Not Player Is Nothing Then
                If CurrentItem.Name = String.Empty Then
                    'Name found in Allplayers, Player already exists in EventPlayers
                    dgPlayers.CurrentItem.Faction = Player.Faction
                    dgPlayers.CurrentItem.Meta = Player.Meta
                    dgPlayers.CurrentItem.Name = Player.Name
                    Try
                        dgPlayers.Items.Refresh()
                    Catch exc As Exception
                        'MessageBox.Show(exc.Message)
                    End Try
                End If
            ElseIf Not dgPlayers.CurrentItem Is Nothing Then
                'Name not found in Allplayers, Player already exists in EventPlayers
                'In this case we want it to be a shared reference so that updates to the new player's
                'faction, meta, and name will carry across
                AllPlayers.Add(CType(dgPlayers.CurrentItem, doPlayer))
                AllPlayers.Last.Name = cbo.Text
                cbo.SelectedItem = AllPlayers.Last
            End If
        End If
    End Sub

    Private Sub ComboBox_LostFocus_1(sender As Object, e As RoutedEventArgs)
        'Meta combobox lost focus
        Dim Metas = BaseController.Model.Metas
        Dim cbo = CType(sender, ComboBox)
        If (From p In Metas Where p = cbo.Text).Count = 0 Then
            Metas.Add(cbo.Text)
            cbo.SelectedItem = Metas.Last
        End If

        Dim SortedMetas = (From p In Metas Order By p).ToList

        Metas.Clear()
        Metas.AddRange(SortedMetas)
    End Sub


    Public Sub Refresh()
        dgPlayers.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget()
    End Sub

    Private Sub dgPlayers_CellEditEnding(sender As Object, e As DataGridCellEditEndingEventArgs) Handles dgPlayers.CellEditEnding
        PGSwiss.Data.DirtyMonitor.IsDirty = True
    End Sub

    Private Sub dgPlayers_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles dgPlayers.PreviewKeyDown
        Dim values = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890D1D2D3D4D5D6D7D8D9D0!@#$%^&*(),./;'[]<>?:""{}\|-=_+"

        Dim NumPadKeys = {Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9}

        If values.Contains(e.Key.ToString) OrElse NumPadKeys.Contains(e.Key) Then dgPlayers.BeginEdit()

        If e.Key = Key.Tab Then
            If dgPlayers.CurrentColumn.DisplayIndex = dgPlayers.Columns.Count - 1 Then
                Dim icg = dgPlayers.ItemContainerGenerator

                If dgPlayers.SelectedIndex = icg.Items.Count - 2 Then
                    Try
                        dgPlayers.CommitEdit(DataGridEditingUnit.Row, False)
                    Catch
                    End Try
                End If
            End If
        End If

    End Sub

    Private Sub btnRandomScenario_Click(sender As Object, e As RoutedEventArgs)
        Dim model = WMEventViewModel.GetSingleton()
        Dim ExclusionList As New List(Of String)
        For Each Round In model.WMEvent.Rounds
            ExclusionList.Add(Round.Scenario)
        Next

        Dim Attempts As Integer = 0
        Dim rnd As New Random
        While ExclusionList.Contains(cboFormat.SelectedValue) AndAlso Attempts < 10
            Attempts += 1
            cboFormat.SelectedIndex = rnd.Next(cboFormat.Items.Count - 1)
        End While
    End Sub
End Class
