Imports System.ComponentModel
Imports PGSwiss.Data

Interface IGame
    Property Condition As String
    Property GameID As Guid
    Property Player1 As doPlayer
    Property Player2 As doPlayer
    Property Reported As Boolean
    Property TableNumber As Integer
    Property Winner As String
    Event PropertyChanged As PropertyChangedEventHandler
End Interface
