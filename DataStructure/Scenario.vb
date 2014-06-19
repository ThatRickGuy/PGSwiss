Imports System.IO
Imports System.Xml.Serialization

Public Class doScenario
    Public Property ScenarioID As Guid
    Public Property Name As String
End Class

Public Class doScenarioCollection
    Inherits List(Of doScenario)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("ScenarioCollection.xml") Then
            Try
                Me.AddRange(Generate())
            Catch exc As Exception
                MessageBox.Show(exc.Message)
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("ScenarioCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("ScenarioCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

    Private Function Generate() As IEnumerable(Of doScenario)
        Dim lst As New List(Of doScenario)
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "1. Destruction"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "2. Supply and Demand"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "3. Balance of Power"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "4. Process of Elimination"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "5. Close Quarters"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "6. Two Fronts"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "7. Incoming"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "8. Rally Point"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "9. Incursion"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "10. Outflank"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "11. Into the Breach"})
        lst.Add(New doScenario With {.ScenarioID = Guid.NewGuid, .Name = "12. Fire Support"})
        Return lst
    End Function

End Class