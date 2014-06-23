Imports System.ComponentModel
Imports PGSwiss.Data

Interface IRound
    Property Bye As doPlayer
    Property Games As doGameCollection
    Property IsLastRound As Boolean
    Property Players As doPlayerCollection
    Property RoundNumber As Integer
    Property Scenario As String
    Property Size As Integer
    Event PropertyChanged As PropertyChangedEventHandler
End Interface
