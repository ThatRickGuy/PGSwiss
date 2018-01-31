Imports Microsoft.Win32
Imports System.Reflection

Public Class Landing

    Private Sub btnCreate_Click(sender As Object, e As RoutedEventArgs) Handles btnCreate.Click
        Dim sfd As New SaveFileDialog
        sfd.AddExtension = True
        sfd.DefaultExt = "XML"
        If sfd.ShowDialog Then
            'go for it
            BaseController.CurrentController.StartEvent(sfd.FileName)
        End If
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As RoutedEventArgs) Handles btnLoad.Click
        Dim ofd As New OpenFileDialog
        ofd.AddExtension = True
        ofd.DefaultExt = "XML"
        ofd.Filter = "XML|*.XML"
        If ofd.ShowDialog Then
            'go for it
            BaseController.CurrentController.StartEvent(ofd.FileName)
        End If
    End Sub

    Private Sub btnManage_Click(sender As Object, e As RoutedEventArgs) Handles btnManage.Click
        BaseController.CurrentController.OpenCollectionManager()
    End Sub

    Private Sub btnImport_Click(sender As Object, e As RoutedEventArgs) Handles btnImport.Click
        Dim ofd As New OpenFileDialog
        ofd.AddExtension = True
        ofd.DefaultExt = "JSON"
        ofd.Filter = "JSON|*.JSON"
        If ofd.ShowDialog Then
            'deserialize
            Dim cc = CType(BaseController.CurrentController, LandingController).DeserializeJSON(IO.File.ReadAllText(ofd.FileName))

            MessageBox.Show("JSON successfully parsed. Please select a file to save the event to.")

            Dim sfd As New SaveFileDialog
            sfd.AddExtension = True
            sfd.DefaultExt = "XML"
            If sfd.ShowDialog Then

                'go for it
                BaseController.CurrentController.StartEvent(sfd.FileName)
                'save the deserialized file
                CType(BaseController.CurrentController, WMEventController).LoadCCToEvent(cc)
            End If


        End If


    End Sub
End Class
