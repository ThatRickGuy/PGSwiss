﻿Imports Microsoft.Win32

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
End Class