Imports System.IO
Imports System.Xml.Serialization

Public Class doRoundSizeCollection
    Inherits List(Of Integer)

    Public Function load() As String
        Dim sReturn As String = String.Empty
        Me.Clear()
        If Not IO.File.Exists("RoundSizeCollection.xml") Then
            Try
                Me.AddRange(From p In Generate() Order By p)
            Catch exc As Exception
                sReturn = (exc.Message)
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("RoundSizeCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim lst As New List(Of Integer)
                lst.AddRange(x.Deserialize(objStreamReader))
                Me.AddRange(From p In lst Order By p)
            End Using
        End If
        Return sReturn
    End Function

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
