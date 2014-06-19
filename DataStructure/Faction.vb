Imports System.IO
Imports System.Xml.Serialization

Public Class doFaction
    Public Property FactionID As Guid
    Public Property Name As String
End Class



Public Class doFactionCollection
    Inherits List(Of doFaction)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("FactionCollection.xml") Then
            Try
                Me.AddRange(Generate())
            Catch exc As Exception
                MessageBox.Show(exc.Message)
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("FactionCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("FactionCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

    Private Function Generate() As IEnumerable(Of doFaction)
        Dim lst As New List(Of doFaction)
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Khador"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Protectorate of Menoth"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Cygnar"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Cryx"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Mercenaries"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Convergence of Cyriss"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Retribution of Scyrah"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Trollblood"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Circle Orboros"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Legion of Everblight"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Skorne"})
        lst.Add(New doFaction With {.FactionID = Guid.NewGuid, .Name = "Minions"})
        Return lst
    End Function

End Class
