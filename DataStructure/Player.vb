Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class doPlayer
    Implements INotifyPropertyChanged


    Public Property PlayerID As Guid

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
            OnPropertyChanged("Name")
        End Set
    End Property

    Private _PPHandle As String
    Public Property PPHandle As String
        Get
            Return _PPHandle
        End Get
        Set(value As String)
            _PPHandle = value
            OnPropertyChanged("PPHandle")
        End Set
    End Property

    Private _Meta As doMeta
    Public Property Meta As doMeta
        Get
            Return _Meta
        End Get
        Set(value As doMeta)
            _Meta = value
            OnPropertyChanged("Meta")
        End Set
    End Property

    Private _Faction As doFaction
    Public Property Faction As doFaction
        Get
            Return _Faction
        End Get
        Set(value As doFaction)
            _Faction = value
            OnPropertyChanged("Faction")
        End Set
    End Property

    Public Property ByeVol As Boolean
    Public Property Drop As Boolean
    Public Property TourneyPoints As Integer
    Public Property StrengthOfSchedule As Integer
    Public Property ControlPoints As Integer
    Public Property ArmyPointsDestroyed As Integer
    Public Property Oppontnents As New List(Of Guid)
    Public Property Rank As Integer

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


    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
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
