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

        Dim es As New EventStandings
        es.Standings = Standings
        es.WMEvent = BaseController.Model.WMEvent

        Me.View.DataContext = es
        Model.CurrentProgress = 90
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
        sbOutput.Replace("[EventDate]", Model.WMEvent.EventDate.ToShortDateString)
        sbOutput.Replace("[EventTitle]", Model.WMEvent.Name)
        sbOutput.Replace("[FileName]", Model.WMEvent.EventID.ToString & ".html")

        Dim sbRows As New StringBuilder()

        For Each player In (From p In Model.CurrentRoundPlayers Order By p.Rank)
            Dim Drop = String.Empty
            If player.Drop Then Drop = "Dropped"
            sbRows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>", _
                                 {player.Rank.ToString, player.Name, player.Faction, player.Meta, player.TourneyPoints, player.StrengthOfSchedule, player.ControlPoints, player.ArmyPointsDestroyed, Drop})
        Next
        sbOutput.Replace("[Rows]", sbRows.ToString())

        If Model.WMEvent.BestPaintedWinner IsNot Nothing AndAlso Model.WMEvent.BestSportWinner IsNot Nothing Then
            sbOutput.Replace("[AwardRow]", "<TR><TD>" & Model.WMEvent.BestPaintedWinner.Name & "</TD><TD></TD><TD>" & Model.WMEvent.BestSportWinner.Name & "</TD></TR>")
        Else
            sbOutput.Replace("[AwardRow]", "<TR><TD>-</TD><TD></TD><TD>-</TD></TR>")
        End If


        sbRows = New StringBuilder()

        For Each r In BaseController.Model.WMEvent.Rounds
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

            sbRows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                      {r.RoundNumber, r.Size, r.Scenario, NonByeGames.Count,
                       (From p In NonByeGames Where p.Condition = "Assassination").Count,
                       (From p In NonByeGames Where p.Condition = "Scenario").Count,
                       (From p In NonByeGames Where p.Condition = "Time").Count,
                       ACP.ToString("#.#"), AAPD.ToString("#.#")})
        Next
        sbOutput.Replace("[StatRows]", sbRows.ToString())

        If Not IO.File.Exists(".\Ringdev.png") Then My.Resources.RingDev.Save(".\Ringdev.png")
        If Not IO.File.Exists(".\pgswiss_small.png") Then My.Resources.pgswiss_small.Save(".\pgswiss_small.png")
        If Not IO.File.Exists(".\pgswiss_icon.png") Then My.Resources.pgswiss_icon.Save(".\pgswiss_icon.png")
        IO.File.WriteAllText(".\" & Model.WMEvent.EventID.ToString & ".html", sbOutput.ToString)

        If Upload Then
            UploadFile(".\" & Model.WMEvent.EventID.ToString & ".html")
            Process.Start("http://ringdev.com/swiss/standings/" & Model.WMEvent.EventID.ToString & ".html")
        Else
            Process.Start(".\" & Model.WMEvent.EventID.ToString & ".html")
        End If

    End Sub

    Private Sub UploadFile(FileName As String)
        Dim FileInfo As New System.IO.FileInfo(FileName)
        Dim FtpWebRequest As System.Net.FtpWebRequest = CType(System.Net.FtpWebRequest.Create(New Uri("ftp://ringdev.com/" & FileInfo.Name)), System.Net.FtpWebRequest)
        FtpWebRequest.Credentials = New System.Net.NetworkCredential("PGSwissUpload", "SendIt1")
        FtpWebRequest.KeepAlive = False
        FtpWebRequest.Timeout = 20000
        FtpWebRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
        FtpWebRequest.UseBinary = True
        FtpWebRequest.ContentLength = FileInfo.Length
        Dim buffLength As Integer = 2048
        Dim buff(buffLength - 1) As Byte
        Using FileStream As System.IO.FileStream = FileInfo.OpenRead()
            Try
                Using Stream As System.IO.Stream = FtpWebRequest.GetRequestStream()
                    Dim contentLen As Integer = FileStream.Read(buff, 0, buffLength)
                    Do While contentLen <> 0
                        Stream.Write(buff, 0, contentLen)
                        contentLen = FileStream.Read(buff, 0, buffLength)
                    Loop
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Upload Error")
            End Try
        End Using
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
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
