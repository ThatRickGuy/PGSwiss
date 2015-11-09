﻿Imports PGSwiss.Data
Imports System.Text
Imports System.Net
Imports System.IO

Public Class StandingsController
    Inherits BaseController

    Protected Overrides Function CreateNext() As BaseController
        Return New ReportingController
    End Function

    Public Sub New()
        Me.View = New Standings

        For Each Player In Model.CurrentRound.Players
            Player.StrengthOfSchedule = 0
            For Each Opponent In Player.Opponents
                Dim po = (From p In Model.CurrentRound.Players Where p.PPHandle = Opponent).FirstOrDefault
                If Not po Is Nothing Then Player.StrengthOfSchedule += po.TourneyPoints
            Next
        Next

        Dim Standings As New doStandings
        Standings.Standings.AddRange((From p In Model.CurrentRound.Players Order By p.TourneyPoints Descending, p.StrengthOfSchedule Descending, p.ControlPoints Descending, p.ArmyPointsDestroyed Descending).ToList)
        Dim i As Integer = 1
        For Each s In Standings.Standings
            s.Rank = i
            i += 1
        Next
        Me.View.DataContext = Standings
    End Sub

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 90

        For Each Player In Model.CurrentRound.Players
            Player.StrengthOfSchedule = 0
            For Each Opponent In Player.Opponents
                Dim po = (From p In Model.CurrentRound.Players Where p.PPHandle = Opponent).FirstOrDefault
                If Not po Is Nothing Then Player.StrengthOfSchedule += po.TourneyPoints
            Next
        Next
    End Sub

    Public Sub PrintStandings(Upload As Boolean)
        Dim sbOutput As New StringBuilder
        sbOutput.Append(My.Resources.HTMLHeaderStandings)
        sbOutput.AppendFormat("<h1>{0}</h1><hr><h3>{1}</h3><br>", {Model.WMEvent.Name, Model.WMEvent.EventDate.ToShortDateString})

        sbOutput.Append("<table>")
        sbOutput.AppendFormat("<tr><td><b>{0}</b></td><td><b>{1}</b></td><td><b>{2}</b></td><td><b>{3}</b></td><td><b>{4}</b></td><td><b>{5}</b></td><td><b>{6}</b></td><td><b>{7}</b></td><td><b>{8}</b></td></tr>", {"Rank", "Name", "PPHandle", "Faction", "Meta", "TP", "SOS", "CP", "APD"})

        For Each player In (From p In Model.CurrentRound.Players Order By p.Rank)
            sbOutput.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>", _
                                 {player.Rank.ToString, player.Name, player.PPHandle, player.Faction, player.Meta, player.TourneyPoints, player.StrengthOfSchedule, player.ControlPoints, player.ArmyPointsDestroyed})
        Next
        sbOutput.Append("</table>")

        sbOutput.Append(My.Resources.Footer)

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
