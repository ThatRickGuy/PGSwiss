Imports System.IO
Imports System.Xml.Serialization

Public Class doScenarioCollection
    Inherits List(Of String)

    Public Function load() As String
        Dim sReturn As String = String.Empty
        Me.Clear()
        If Not IO.File.Exists("ScenarioCollection.xml") Then
            Try
                Me.AddRange(From p In Generate() Order By p)
            Catch exc As Exception
                sReturn = exc.Message
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("ScenarioCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim lst As New List(Of String)
                lst.AddRange(x.Deserialize(objStreamReader))
                Me.AddRange(From p In lst Order By p.Split(".")(0).PadLeft(2, "0"c))
            End Using
        End If
        Return sReturn
    End Function

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("ScenarioCollection.xml")
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
        lst.Add("1. Destruction")
        lst.Add("2. Supply and Demand")
        lst.Add("3. Balance of Power")
        lst.Add("4. Process of Elimination")
        lst.Add("5. Close Quarters")
        lst.Add("6. Two Fronts")
        lst.Add("7. Incoming")
        lst.Add("8. Rally Point")
        lst.Add("9. Incursion")
        lst.Add("10. Outflank")
        lst.Add("11. Into the Breach")
        lst.Add("12. Fire Support")
        Return lst
    End Function

End Class