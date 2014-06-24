Imports System.IO
Imports System.Xml.Serialization

Public Class doRoundSizeCollection
    Inherits List(Of Integer)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("RoundSizeCollection.xml") Then
            Try
                Me.AddRange(Generate())
            Catch exc As Exception
                MessageBox.Show(exc.Message)
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("RoundSizeCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("RoundSizeCollection.xml")
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

    Private Function Generate() As IEnumerable(Of Integer)
        Dim lst As New List(Of Integer)
        lst.Add(15)
        lst.Add(25)
        lst.Add(35)
        lst.Add(42)
        lst.Add(50)
        lst.Add(75)
        lst.Add(100)
        lst.Add(150)
        Return lst
    End Function

End Class
