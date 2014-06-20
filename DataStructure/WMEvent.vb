﻿Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class doWMEvent
    Implements INotifyPropertyChanged

    Public Property EventID As Guid = Guid.NewGuid
    Public Property EventFormat As String
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

    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class