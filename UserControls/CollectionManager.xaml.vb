Imports PGSwiss.Data

Public Class CollectionManager

    Private Sub cboCollection_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cboCollection.SelectionChanged
        Dim Controller = CType(BaseController.CurrentController, CollectionManagerController)
        Dim Holder As New List(Of StringHolder)
        Select Case CType(cboCollection.SelectedItem, ComboBoxItem).Content
            Case Is = "Event Formats"
                Dim h As New List(Of StringAndStringCollectionHolder)
                For Each EventFormat In Controller.EventFormats
                    h.Add(New StringAndStringCollectionHolder(EventFormat))
                Next
                Me.dgCollection.ItemsSource = h
            Case Is = "Game Sizes"
                Dim q = From p In Controller.Sizes Select New StringHolder With {.Value = p.ToString}
                Holder.AddRange(q)
                Me.dgCollection.ItemsSource = Holder
            Case Is = "Factions"
                Dim q = From p In Controller.Factions Select New StringHolder With {.Value = p}
                Holder.AddRange(q)
                Me.dgCollection.ItemsSource = Holder
            Case Is = "Players"
                Me.dgCollection.ItemsSource = Controller.Players
            Case Is = "Metas"
                Dim q = From p In Controller.Metas Select New StringHolder With {.Value = p}
                Holder.AddRange(q)
                Me.dgCollection.ItemsSource = Holder
        End Select
        Me.dgCollection.Items.Refresh()
        Me.dgCollection.CanUserAddRows = True
        Me.dgCollection.IsReadOnly = False
        Me.dgCollection.CanUserDeleteRows = True
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        CType(BaseController.CurrentController, CollectionManagerController).Save()
        BaseController.CurrentController.OpenLandingPage()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        BaseController.CurrentController.OpenLandingPage()
    End Sub

    Public Class StringHolder
        Public Property Value As String
    End Class

    Public Class StringAndStringCollectionHolder
        Public Sub New(EventFormat As doEventFormat)
            Value = EventFormat.Name
            _StringCollection = EventFormat.Scenarios
        End Sub

        Public Property Value As String
        Private _StringCollection As New List(Of String)
        Public Property StringCollection As String
            Get
                Dim sReturn As String = String.Empty
                For Each s In _StringCollection
                    sReturn &= s & ", "
                Next
                If sReturn.EndsWith(", ") Then sReturn = sReturn.Substring(0, sReturn.Length - 2)
                Return sReturn
            End Get
            Set(value As String)
                Dim values = value.Split(",")
                _StringCollection.Clear()
                For Each s In values
                    _StringCollection.Add(s.TrimEnd)
                Next
            End Set
        End Property

        Public Function GetActualCollection() As List(Of String)
            Return _StringCollection
        End Function
    End Class

    Private Sub dgCollection_LostFocus(sender As Object, e As RoutedEventArgs) Handles dgCollection.LostFocus
        Dim Holder As List(Of StringHolder)
        Dim Controller = CType(BaseController.CurrentController, CollectionManagerController)
        Select Case CType(cboCollection.SelectedItem, ComboBoxItem).Content
            Case Is = "Event Formats"
                Dim h As List(Of StringAndStringCollectionHolder) = Me.dgCollection.ItemsSource
                Controller.EventFormats.Clear()
                For Each i In h
                    Controller.EventFormats.Add(New doEventFormat With {.Name = i.Value, .Scenarios = i.GetActualCollection})
                Next
            Case Is = "Game Sizes"
                Holder = Me.dgCollection.ItemsSource
                Controller.Sizes.Clear()
                Controller.Sizes.AddRange(From p In Holder Where IsNumeric(p.Value) Select CInt(p.Value))
            Case Is = "Factions"
                Holder = Me.dgCollection.ItemsSource
                Controller.Factions.Clear()
                Controller.Factions.AddRange(From p In Holder Select p.Value)
            Case Is = "Players"
                'Already bound
            Case Is = "Metas"
                Holder = Me.dgCollection.ItemsSource
                Controller.Metas.Clear()
                Controller.Metas.AddRange(From p In Holder Select p.Value)
        End Select
    End Sub

End Class
