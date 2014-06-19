Imports System.IO
Imports System.Xml.Serialization

Public Class doEventFormat
    Public Property EventFormatID As Guid
    Public Property Name As String
End Class


Public Class doEventFormatCollection
    Inherits List(Of doEventFormat)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("EventFormatCollection.xml") Then
            Me.AddRange(Generate())
            Save()
        Else
            Using objStreamReader As New StreamReader("EventFormatCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("EventFormatCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

    Private Function Generate() As IEnumerable(Of doEventFormat)
        Dim lst As New List(Of doEventFormat)
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "SR2014 50pt"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "SR2014 35pt"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "SR2014 25pt"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "SR2014 Masters"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "SR2014 Hard-Core"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "Iron Gauntlet Season 2"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "Who's the Boss"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "Highlander"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "Escalation"})
        lst.Add(New doEventFormat With {.EventFormatID = Guid.NewGuid(), .Name = "Other"})
        Return lst
    End Function

End Class
