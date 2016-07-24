Imports System.Windows.Controls.Primitives
Imports System.Windows.Threading
Imports System.ComponentModel
Imports PGSwiss.Data

Public Class WMEvent

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

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



    Private Sub RemovePlayer_Click(sender As Object, e As RoutedEventArgs)
        Dim CurrentItem As doPlayer = Nothing
        If dgPlayers.CurrentItem.GetType Is GetType(doPlayer) Then CurrentItem = CType(dgPlayers.CurrentItem, doPlayer)

        dgPlayers.CommitEdit()
        If Not CurrentItem Is Nothing Then
            BaseController.Model.WMEvent.Players.Remove(CurrentItem)
        End If
        Try
            dgPlayers.Items.Refresh()
        Catch
        End Try
    End Sub

    Private Sub WMEvent_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.txtEventName.Focus()
    End Sub

    Private Sub cboFaction_PreviewKeyDown_1(sender As Object, e As KeyEventArgs)
        Dim cbo As ComboBox = CType(sender, ComboBox)
        Dim FoundIndex As Integer = -1

        If cbo.SelectedIndex > 0 Then
            'something already selected
            For i = cbo.SelectedIndex + 1 To cbo.Items.Count - 1
                If cbo.Items(i).ToString.Substring(0, 1).ToUpper = e.Key.ToString.ToUpper Then
                    FoundIndex = i 'found
                    Exit For
                End If
            Next
            If FoundIndex = -1 Then
                For i = 1 To cbo.SelectedIndex - 1
                    If cbo.Items(i).ToString.Substring(0, 1).ToUpper = e.Key.ToString.ToUpper Then
                        FoundIndex = i 'found
                        Exit For
                    End If
                Next
            End If
        Else
            For i = 1 To cbo.Items.Count - 1
                If cbo.Items(i).ToString.Substring(0, 1).ToUpper = e.Key.ToString.ToUpper Then
                    FoundIndex = i 'found
                    Exit For
                End If
            Next
        End If

        If FoundIndex > 0 Then cbo.SelectedIndex = FoundIndex
        If e.Key <> Key.Tab Then e.Handled = True
    End Sub



End Class

