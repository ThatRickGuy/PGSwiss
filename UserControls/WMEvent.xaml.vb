Imports System.Windows.Controls.Primitives
Imports System.Windows.Threading

Public Class WMEvent
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
    End Sub


    Private Sub cboPPHandle_LostFocus_1(sender As Object, e As RoutedEventArgs)
        'PP Handle combobox lost focus
        Dim AllPlayers = BaseController.Model.Players
        Dim EventPlayers = BaseController.Model.WMEvent.Players
        Dim cbo = CType(sender, ComboBox)
        Dim CurrentItem As doPlayer = Nothing
        If dgPlayers.CurrentItem.GetType Is GetType(doPlayer) Then CurrentItem = CType(dgPlayers.CurrentItem, doPlayer)

        Dim Player = (From p In AllPlayers Where p.PPHandle = cbo.Text).FirstOrDefault
        If Not CurrentItem Is Nothing Then
            If Not Player Is Nothing Then
                If CurrentItem.Name = String.Empty Then
                    'PPHandle found in Allplayers, Player already exists in EventPlayers
                    Dim q = (From p In EventPlayers Where p.PPHandle Is Nothing).FirstOrDefault
                    If Not q Is Nothing Then
                        q.Faction = Player.Faction
                        q.Meta = Player.Meta
                        q.Name = Player.Name
                        q.PlayerID = Player.PlayerID
                    End If

                    dgPlayers.CurrentItem = Player
                    Try
                        dgPlayers.Items.Refresh()
                    Catch
                    End Try
                End If
            ElseIf Not dgPlayers.CurrentItem Is Nothing Then
                'PPHandle not found in Allplayers, Player already exists in EventPlayers
                AllPlayers.Add(CType(dgPlayers.CurrentItem, doPlayer).Clone)
                AllPlayers.Last.PPHandle = cbo.Text
                cbo.SelectedItem = AllPlayers.Last
            End If
        End If

    End Sub

    Private Sub ComboBox_LostFocus_1(sender As Object, e As RoutedEventArgs)
        'Meta combobox lost focus
        Dim Metas = BaseController.Model.Metas
        Dim cbo = CType(sender, ComboBox)
        If (From p In Metas Where p.Name = cbo.Text).Count = 0 Then
            Metas.Add(New doMeta With {.Name = cbo.Text, .MetaID = Guid.NewGuid})
            cbo.SelectedItem = Metas.Last
        End If

        Dim SortedMetas = (From p In Metas Order By p.Name).ToList

        Metas.Clear()
        Metas.AddRange(SortedMetas)
    End Sub


    Private Sub dgPlayers_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles dgPlayers.PreviewKeyDown
        Dim values = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*(),./;'[]<>?:""{}\|-=_+"
        If values.Contains(e.Key.ToString) Then dgPlayers.BeginEdit()

        If e.Key = Key.Tab Then
            If dgPlayers.CurrentColumn.DisplayIndex = dgPlayers.Columns.Count - 1 Then
                Dim icg = dgPlayers.ItemContainerGenerator

                If dgPlayers.SelectedIndex = icg.Items.Count - 2 Then
                    dgPlayers.CommitEdit(DataGridEditingUnit.Row, False)
                End If
            End If
        End If
    End Sub

    Private Sub RemovePlayer_Click(sender As Object, e As RoutedEventArgs)
        Dim CurrentItem As doPlayer = Nothing
        If dgPlayers.CurrentItem.GetType Is GetType(doPlayer) Then CurrentItem = CType(dgPlayers.CurrentItem, doPlayer)

        If Not CurrentItem Is Nothing Then
            BaseController.Model.WMEvent.Players.Remove(CurrentItem)
        End If
        dgPlayers.Items.Refresh()
    End Sub
End Class

