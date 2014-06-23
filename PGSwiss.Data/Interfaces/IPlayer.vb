Imports System.ComponentModel
Imports PGSwiss.Data

Interface IdoPlayer
    Property ArmyPointsDestroyed As Integer
    Property ByeVol As Boolean
    Property ControlPoints As Integer
    Property Drop As Boolean
    Property Faction As String
    Property Meta As String
    Property Name As String
    Property Oppontnents As List(Of String)
    Property PPHandle As String
    Property Rank As Integer
    Property StrengthOfSchedule As Integer
    Property TourneyPoints As Integer
    Event PropertyChanged As PropertyChangedEventHandler
    Function Clone() As doPlayer
End Interface
