Imports PGSwiss.Data
Imports System.Text
Imports System.Net
Imports System.IO
Imports System.ComponentModel

Public Class StandingsController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New ReportingController
    End Function

    Public Sub New()
        Me.View = New Standings
    End Sub

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Dim Standings As New doStandings
        Standings.Standings = Model.CurrentRound.GetPlayers(Model.WMEvent)

        SetEventStandings(Standings)

        Model.CurrentProgress = 90
    End Sub

    Public Sub SetEventStandings(Standings As doStandings)
        Dim es As New EventStandings
        es.Standings = Standings
        es.WMEvent = BaseController.Model.WMEvent

        Me.View.DataContext = es
    End Sub


    Public Sub PrintStandings(Upload As Boolean)
        Dim sbOutput As New StringBuilder()
        sbOutput.Append(My.Resources.Standings)
        sbOutput.Replace("[Event Title]", Model.WMEvent.Name)
        sbOutput.Replace("[Date]", Model.WMEvent.EventDate.Year & "-" & Model.WMEvent.EventDate.Month & "-" & Model.WMEvent.EventDate.Day)
        sbOutput.Replace("[Location]", "")
        sbOutput.Replace("[Format]", Model.WMEvent.EventFormat.Name)
        sbOutput.Replace("[PG]", "")
        sbOutput.Replace("[Version]", My.Application.Info.Version.ToString())
        sbOutput.Replace("[EventDate]", Model.WMEvent.EventDate.Year & "-" & Model.WMEvent.EventDate.Month & "-" & Model.WMEvent.EventDate.Day)
        sbOutput.Replace("[EventTitle]", Model.WMEvent.Name)
        sbOutput.Replace("[FileName]", Model.WMEvent.EventID.ToString & ".html")

        Dim sbRows As New StringBuilder()

        For Each player In (From p In Model.CurrentRoundPlayers Order By p.Rank)
            Dim Drop = String.Empty
            If Player.Drop Then Drop = "Dropped"
            Dim PlayerName = ""
            If player.CCCode <> "" Then
                PlayerName = "<a href=""https://conflictchamber.com/?" & player.CCCode & """><img src=""http://ringdev.com/images/ccbolt.png"" alt=""Conflict Chamber"" style=""Height:22px;Width:13px""><span style=""margin-left:3px;"">" & player.Name & "</span></a>"
            Else
                PlayerName = "<div style=""margin-left:16px;"">" & player.Name & "</div>"
            End If
            sbRows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                                 {player.Rank.ToString, PlayerName, player.Faction, player.Meta, player.TourneyPoints, player.StrengthOfSchedule, player.ControlPoints, player.ArmyPointsDestroyed, Drop})
        Next
        sbOutput.Replace("[Rows]", sbRows.ToString())

        If Model.WMEvent.BestPaintedWinner IsNot Nothing AndAlso Model.WMEvent.BestSportWinner IsNot Nothing Then
            sbOutput.Replace("[AwardRow]", "<TR><TD>" & Model.WMEvent.BestPaintedWinner.Name & "</TD><TD></TD><TD>" & Model.WMEvent.BestSportWinner.Name & "</TD></TR>")
        Else
            sbOutput.Replace("[AwardRow]", "<TR><TD>-</TD><TD></TD><TD>-</TD></TR>")
        End If


        sbRows = New StringBuilder()

        Dim rounds = (From p In BaseController.Model.WMEvent.Rounds Where p.Scenario <> "" Order By p.RoundNumber Select p).ToList()
        For Each r In rounds
            Dim NonByeGames = From p In r.Games Where p.Player2 IsNot Nothing
            'Get all the CP's for Player1 where they have an opponent (ie: no Byes!)
            Dim q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ControlPoints).ToList
            'Get all the CP's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ControlPoints)
            Dim ACP = 0
            If q.Count > 0 Then ACP = q.Average


            'Get all the APD's for Player1 where they have an opponent (ie: no Byes!)
            q = (From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player1.ArmyPointsDestroyed).ToList
            'Get all the APD's for Player2
            q.AddRange(From p In NonByeGames Where p.Player2 IsNot Nothing Select p.Player2.ArmyPointsDestroyed)
            Dim AAPD = 0
            If q.Count > 0 Then AAPD = q.Average

            sbRows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td></tr>",
                      {r.RoundNumber, r.Size, r.Scenario, NonByeGames.Count,
                       (From p In NonByeGames Where p.Condition = "Assassination").Count,
                       (From p In NonByeGames Where p.Condition = "Scenario").Count,
                       (From p In NonByeGames Where p.Condition = "Tie Breakers").Count,
                       (From p In NonByeGames Where p.Condition = "Death Clock").Count,
                       ACP.ToString("#.#"), AAPD.ToString("#.#")})
        Next
        sbOutput.Replace("[StatRows]", sbRows.ToString())

        If Not IO.File.Exists(".\Ringdev.png") Then My.Resources.RingDev.Save(".\Ringdev.png")
        If Not IO.File.Exists(".\pgswiss_small.png") Then My.Resources.pgswiss_small.Save(".\pgswiss_small.png")
        If Not IO.File.Exists(".\pgswiss_icon.png") Then My.Resources.pgswiss_icon.Save(".\pgswiss_icon.png")
        IO.File.WriteAllText(".\" & Model.WMEvent.EventID.ToString & ".html", sbOutput.ToString)

        If Upload Then
            WebAPIHelper.UploadFile(".\" & Model.WMEvent.EventID.ToString & ".html")
            IO.File.Copy(Model.WMEvent.FileName, ".\" & Model.WMEvent.EventID.ToString & ".xml", True)

            WebAPIHelper.UploadFile(".\" & Model.WMEvent.EventID.ToString & ".xml")

            Process.Start("http://ringdev.com/swiss/standings/" & Model.WMEvent.EventID.ToString & ".html")
        Else
            Process.Start(".\" & Model.WMEvent.EventID.ToString & ".html")
        End If

    End Sub

End Class

Public Class EventStandings
    Implements INotifyPropertyChanged

    Private _Standings As doStandings
    Public Property Standings As doStandings
        Get
            Return _Standings
        End Get
        Set(value As doStandings)
            _Standings = value
            OnPropertyChanged("Standings")
        End Set
    End Property

    Private _WMEvent As doWMEvent
    Public Property WMEvent As doWMEvent
        Get
            Return _WMEvent
        End Get
        Set(value As doWMEvent)
            _WMEvent = value
            OnPropertyChanged("WMEvent")
        End Set
    End Property

    Protected Sub OnPropertyChanged(ByVal name As String)
        PGSwiss.Data.DirtyMonitor.IsDirty = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
