Imports System.IO
Imports System.Xml.Serialization

Public Class doPlayer
    Public Property PlayerID As Guid
    Public Property Name As String
    Public Property PPHandle As String
    Public Property Meta As doMeta
    Public Property Faction As doFaction
    Public Property ByeVol As Boolean
    Public Property Drop As Boolean
    Public Property TourneyPoints As Integer
    Public Property StrengthOfSchedule As Integer
    Public Property ControlPoints As Integer
    Public Property ArmyPointsDestroyed As Integer
    Public Property Oppontnents As New List(Of Guid)

    Public Sub New()
        PlayerID = Guid.NewGuid
    End Sub

    Public Function Clone() As doPlayer
        Dim dopReturn As New doPlayer
        dopReturn.Faction = Me.Faction
        dopReturn.Meta = Me.Meta
        dopReturn.Name = Me.Name
        dopReturn.PlayerID = Me.PlayerID
        dopReturn.PPHandle = Me.PPHandle
        Return dopReturn
    End Function
End Class

Public Class doPlayerCollection
    Inherits List(Of doPlayer)

    Public Sub load()
        Me.Clear()
        If IO.File.Exists("PlayerCollection.xml") Then
            Using objStreamReader As New StreamReader("PlayerCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Me.AddRange(x.Deserialize(objStreamReader))
            End Using
        End If
    End Sub

    Public Sub Save()
        Using objStreamWriter As New StreamWriter("PlayerCollection.xml")
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub
End Class
