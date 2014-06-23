Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class doWMEvent
    Implements INotifyPropertyChanged
    Implements IWMEvent


    Public Sub New()

    End Sub

    Public Sub New(FileName As String)
        Me._FileName = FileName
        load()
    End Sub

    Private _FileName As String

    Public Property EventID As Guid = Guid.NewGuid Implements IWMEvent.EventID
    Public Property EventFormat As String Implements IWMEvent.EventFormat
    Private _Name As String
    Public Property Name As String Implements IWMEvent.Name
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
            OnPropertyChanged("Name")
        End Set
    End Property
    Public Property EventDate As Date = Today Implements IWMEvent.EventDate
    Public Property Players As New List(Of doPlayer) Implements IWMEvent.Players
    Public Property Rounds As New List(Of doRound) Implements IWMEvent.Rounds

    Public Sub load() Implements IWMEvent.load
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

    Public Sub Save() Implements IWMEvent.Save
        Using objStreamWriter As New StreamWriter(_FileName)
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(objStreamWriter, Me)
        End Using
    End Sub

    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged, IWMEvent.PropertyChanged
End Class