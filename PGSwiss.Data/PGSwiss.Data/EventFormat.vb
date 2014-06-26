Imports System.IO
Imports System.Xml.Serialization


Public Class doEventFormatCollection
    Inherits List(Of String)

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

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("EventFormatCollection.xml")
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
        lst.Add("SR2014 50pt")
        lst.Add("SR2014 35pt")
        lst.Add("SR2014 25pt")
        lst.Add("SR2014 Masters")
        lst.Add("SR2014 Hard-Core")
        lst.Add("Iron Gauntlet Season 2")
        lst.Add("Who's the Boss")
        lst.Add("Highlander")
        lst.Add("Escalation")
        lst.Add("Other")
        Return lst
    End Function

End Class
