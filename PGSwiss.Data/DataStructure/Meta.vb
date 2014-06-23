Imports System.IO
Imports System.Xml.Serialization

Public Class doMetaCollection
    Inherits List(Of String)
    Implements IMetaCollection

    Public Sub load() Implements IMetaCollection.load
        Me.Clear()
        If Not IO.File.Exists("MetaCollection.xml") Then
            Me.AddRange(Generate())
            Save()
        Else
            Using objStreamReader As New StreamReader("MetaCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save() Implements IMetaCollection.Save
        Using objStreamWriter As New StreamWriter("MetaCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub
    Private Function Generate() As IEnumerable(Of String)
        Dim lst As New List(Of String)
        lst.Add("")
        Return lst
    End Function
End Class
