Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Text

Public Class doWMEvent
    Implements INotifyPropertyChanged



    Public Sub New()

    End Sub

    Public Sub New(FileName As String)
        Me._FileName = FileName
        load()
    End Sub

    Private _FileName As String

    Public Property EventID As Guid = Guid.NewGuid

    Private _EventFormat As doEventFormat
    Public Property EventFormat As doEventFormat
        Get
            Return _EventFormat
        End Get
        Set(value As doEventFormat)
            _EventFormat = value
            OnPropertyChanged("EventFormat")
        End Set
    End Property

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
    Public Property EventDate As Date = Today
    Public Property Players As New List(Of doPlayer)
    Public Property Rounds As New List(Of doRound)

    Public Sub load()
        If File.Exists(_FileName) Then
            Using objStreamReader As New StreamReader(_FileName)
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

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter(_FileName)
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

    Public Sub FromSerialization(SerializedWMEvent As String)
        Dim x As New XmlSerializer(Me.GetType)
        Dim temp As doWMEvent = (x.Deserialize(New MemoryStream(Encoding.UTF8.GetBytes(SerializedWMEvent))))

        Me.EventID = temp.EventID
        Me.EventFormat = temp.EventFormat
        Me.Name = temp.Name
        Me.EventDate = temp.EventDate
        Me.Players = temp.Players
        Me.Rounds = temp.Rounds
    End Sub

    Public Function ToSerialization() As String
        Dim ms As New MemoryStream()
        Dim x As New XmlSerializer(Me.GetType)
        x.Serialize(ms, Me)
        ms.Position = 0
        Dim sr As New StreamReader(ms)
        Dim x1 = sr.ReadToEnd
        Return x1
    End Function

    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler _
      Implements INotifyPropertyChanged.PropertyChanged

End Class