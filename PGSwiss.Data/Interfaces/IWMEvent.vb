Imports System.ComponentModel
Imports PGSwiss.Data

Interface IWMEvent
    Property EventDate As Date
    Property EventFormat As String
    Property EventID As Guid
    Property Name As String
    Property Players As List(Of doPlayer)
    Property Rounds As List(Of doRound)
    Event PropertyChanged As PropertyChangedEventHandler
    Sub load()
    Sub Save()
End Interface
