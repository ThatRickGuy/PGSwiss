Imports System.Net
Imports System.Web.Http

Public Class EventController
    Inherits ApiController

    Public Function GetEvents(FromDate As Date) As IEnumerable(Of EventLite)
        Using context As New PGSwiss.WebServices.Entities
            Return (From p In context.WMEvents Where p.Date >= FromDate Order By p.Date Descending, p.Name Select New EventLite With {.EventDate = p.Date, .Format = p.Format, .Name = p.Name, .ID = p.WMEventId}).ToList
        End Using
    End Function

    Public Function GetEvent(EventID As Guid) As WMEvent
        Using context As New PGSwiss.WebServices.Entities
            Return (From p In context.WMEvents Where p.WMEventId = EventID).FirstOrDefault
        End Using
    End Function

    Public Sub PostEvent(SerializedWMEvent As String)
        Using context As New PGSwiss.WebServices.Entities

            Dim WMEvent As New PGSwiss.Data.doWMEvent()
            WMEvent.FromSerialization(SerializedWMEvent)

            Dim WMEventEntity As WMEvent = (From p In context.WMEvents Where p.WMEventId = WMEvent.EventID).FirstOrDefault
            If WMEventEntity Is Nothing Then WMEventEntity = New WMEvent

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
        End Using
    End Sub
End Class

Public Class EventLite
    Public EventDate As Date
    Public Format As String
    Public Name As String
    Public ID As Guid
End Class