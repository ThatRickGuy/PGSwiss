Imports System.IO
Imports System.Xml.Serialization

Public Class doMetaCollection
    Inherits List(Of String)

    Public Sub load()
        Me.Clear()
        If IO.File.Exists("MetaCollection.xml") Then
            Using objStreamReader As New StreamReader("MetaCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("MetaCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub
End Class
