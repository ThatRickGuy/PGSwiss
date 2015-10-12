Imports System.Net
Imports System.Net.Http
Imports System.Web.Http

<Authorize>
Public Class ValuesController
    Inherits ApiController

    Public Sub PostPublishEvent(WMEvent As PGSwiss.Data.doWMEvent)
        Dim context As New PGSwiss.WebServices.Entities
        Dim WMEventEntity As New WMEvent
        WMEventEntity.Date = WMEvent.EventDate
        WMEventEntity.Format = WMEvent.EventFormat.Name
        WMEventEntity.Name = WMEvent.Name
        WMEventEntity.WMEventId = WMEvent.EventID

        context.WMEvents.Add(WMEventEntity)

        Dim FinalStandings = WMEvent.Rounds.Last.Players
        For Each p In FinalStandings
            Dim PlayerEntity As New WMEventStanding
            PlayerEntity.WMEventID = WMEvent.EventID
            PlayerEntity.Rank = p.Rank
            PlayerEntity.PPHandle = p.PPHandle
            PlayerEntity.Name = p.Name
            PlayerEntity.TP = p.TourneyPoints
            PlayerEntity.APD = p.ArmyPointsDestroyed
            PlayerEntity.CP = p.ControlPoints
            PlayerEntity.Faction = p.Faction

            context.WMEventStandings.Add(PlayerEntity)
        Next

        context.SaveChanges()
    End Sub

    ' GET api/values
    Public Function GetValues() As IEnumerable(Of String)
        Return New String() {"value1", "value2"}
    End Function

    ' GET api/values/5
    Public Function GetValue(id As Integer) As String
        Return "value"
    End Function

    ' POST api/values
    Public Sub PostValue(<FromBody> value As String)

    End Sub

    ' PUT api/values/5
    Public Sub PutValue(id As Integer, <FromBody> value As String)

    End Sub

    ' DELETE api/values/5
    Public Sub DeleteValue(id As Integer)

    End Sub
End Class
