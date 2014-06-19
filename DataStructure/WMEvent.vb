Imports System.IO
Imports System.Xml.Serialization

Public Class doWMEvent
    Public Property EventID As Guid = Guid.NewGuid
    Public Property EventFormat As doEventFormat
    Public Property Name As String
    Public Property EventDate As Date = Today
    Public Property Players As New List(Of doPlayer)
    Public Property Rounds As New List(Of doRound)
    
    Public Sub load()
        If File.Exists("EventCollection.xml") Then
            Using objStreamReader As New StreamReader("EventCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim temp As doWMEvent = (x.Deserialize(objStreamReader))

                Me.EventID = temp.EventID
                Me.EventFormat = temp.EventFormat
                Me.Name = temp.Name
                Me.EventDate = temp.EventDate
                Me.Players = temp.Players
                Me.Rounds = temp.Rounds
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("EventCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

End Class