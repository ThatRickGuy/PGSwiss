Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Collections.ObjectModel

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

    Public Property PGSwissVersion As String

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

    Private _BestPaintedWinner As doPlayer
    Public Property BestPaintedWinner As doPlayer
        Get
            Return _BestPaintedWinner
        End Get
        Set(value As doPlayer)
            _BestPaintedWinner = value
            OnPropertyChanged("BestPaintedWinner")
        End Set
    End Property

    Private _BestSportWinner As doPlayer
    Public Property BestSportWinner As doPlayer
        Get
            Return _BestSportWinner
        End Get
        Set(value As doPlayer)
            _BestSportWinner = value
            OnPropertyChanged("BestSportWinner")
        End Set
    End Property


    Private WithEvents _Players As New ObservableCollection(Of doPlayer)
    Public Property Players As ObservableCollection(Of doPlayer)
        Get
            Return _Players
        End Get
        Set(value As ObservableCollection(Of doPlayer))
            _Players = value
        End Set
    End Property
    Public Property Rounds As New List(Of doRound)

    Public Sub x() Handles _Players.CollectionChanged
        OnPropertyChanged("Players")
    End Sub


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
                If (Me.Rounds.FirstOrDefault IsNot Nothing) Then
                    If (Me.Rounds.FirstOrDefault().Games IsNot Nothing) Then
                        If (Me.Rounds.FirstOrDefault().Games.FirstOrDefault() IsNot Nothing) Then
                            Dim firstGame = Me.Rounds.FirstOrDefault().Games.FirstOrDefault()
                            If firstGame.Winner <> firstGame.Player1.Name AndAlso firstGame.Winner <> firstGame.Player2.Name AndAlso firstGame.Winner <> String.Empty Then
                                'this is likely a pre 1.10 file being loaded by 1.10 or later
                                For Each round In Me.Rounds
                                    For Each game In round.Games
                                        If game.Winner IsNot Nothing Then
                                            If game.Winner.Contains(game.Player1.Name) Then
                                                game.Winner = game.Player1.Name
                                            ElseIf game.Winner.Contains(game.Player2.Name) Then
                                                game.Winner = game.Player2.Name
                                            End If
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    End If
                End If
                Me.BestPaintedWinner = temp.BestPaintedWinner
                Me.BestSportWinner = temp.BestSportWinner
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

    Protected Sub OnPropertyChanged(ByVal name As String)
        DirtyMonitor.IsDirty = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler _
      Implements INotifyPropertyChanged.PropertyChanged

End Class