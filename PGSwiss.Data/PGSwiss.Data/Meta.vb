Imports System.IO
Imports System.Xml.Serialization

Public Class doMetaCollection
    Inherits List(Of String)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("MetaCollection.xml") Then
            Me.AddRange(From p In Generate() Order By p)
            Save()
        Else
            Using objStreamReader As New StreamReader("MetaCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim lst As New List(Of String)
                lst.AddRange(x.Deserialize(objStreamReader))
                Me.AddRange(From p In lst Order By p)
            End Using
        End If
    End Sub

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("MetaCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                x.Serialize(objStreamWriter, Me)
            End Using

            _FailedSave = False
        Catch exc As System.IO.IOException
            If _FailedSave = False Then
                _FailedSave = True
                Save()
            End If
        End Try
    End Sub


    Private Function Generate() As IEnumerable(Of String)
        Dim lst As New List(Of String)
        lst.Add("")
        Return lst
    End Function
End Class
