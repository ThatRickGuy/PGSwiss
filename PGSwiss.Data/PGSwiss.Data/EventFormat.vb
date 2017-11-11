Imports System.IO
Imports System.Xml.Serialization

Public Class doEventFormat
    Public Property Name As String
    Public Property Scenarios As New List(Of String)


    Public Overloads Overrides Function Equals(obj As Object) As Boolean

        If obj Is Nothing OrElse Not Me.GetType() Is obj.GetType() Then
            Return False
        End If

        Return Me.Name = CType(obj, doEventFormat).Name
    End Function
End Class

Public Class doEventFormatCollection
    Inherits List(Of doEventFormat)

    Public Sub load()
        Me.Clear()
        If Not IO.File.Exists("EventFormatCollection.xml") Then
            Me.AddRange(From p In Generate() Order By p.Name)
            Save()
        Else
            Using objStreamReader As New StreamReader("EventFormatCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim lst As New List(Of doEventFormat)
                lst.AddRange(x.Deserialize(objStreamReader))

                If (From p In lst Where p.Name = "2017 Masters").Count = 0 Then
                    Dim AllFormats = Generate()
                    For Each EventFormat In AllFormats
                        Dim q = (From p In Me Where p.Name = EventFormat.Name Select p).FirstOrDefault()
                        If q Is Nothing Then Me.Add(EventFormat)
                    Next
                Else
                    Me.AddRange(From p In lst Order By p.Name)
                    Dim q = (From p In Me Where p.Name = "2017 SteamR Roller").FirstOrDefault
                    If q IsNot Nothing Then
                        q.Name = "2017 Steam Roller"
                    End If
                    q = (From p In Me Where p.Name = "2017 SteamR Roller Rumble").FirstOrDefault
                    If q IsNot Nothing Then
                        q.Name = "2017 Steam Roller Rumble"
                    End If
                End If


            End Using
            Save()
        End If
    End Sub

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("EventFormatCollection.xml")
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


    Private Function Generate() As IEnumerable(Of doEventFormat)
        Dim lst As New List(Of doEventFormat)
        Dim ef As doEventFormat

        ef = New doEventFormat
        ef.Name = "2017 Masters"
        ef.Scenarios.Add("1. The Pit")
        ef.Scenarios.Add("2. Standoff")
        ef.Scenarios.Add("3. Spread the Net")
        ef.Scenarios.Add("4. Breakdown")
        ef.Scenarios.Add("5. Outlast")
        ef.Scenarios.Add("6. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2017 Champions"
        ef.Scenarios.Add("1. The Pit")
        ef.Scenarios.Add("2. Standoff")
        ef.Scenarios.Add("3. Spread the Net")
        ef.Scenarios.Add("4. Breakdown")
        ef.Scenarios.Add("5. Outlast")
        ef.Scenarios.Add("6. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2017 Steam Roller"
        ef.Scenarios.Add("1. The Pit")
        ef.Scenarios.Add("2. Standoff")
        ef.Scenarios.Add("3. Spread the Net")
        ef.Scenarios.Add("4. Breakdown")
        ef.Scenarios.Add("5. Outlast")
        ef.Scenarios.Add("6. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2017 Steam Roller Rumble"
        ef.Scenarios.Add("1. Patrol")
        ef.Scenarios.Add("2. Killing Field")
        ef.Scenarios.Add("3. Target of Opportunity")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Who's the Boss"
        ef.Scenarios.Add("1. Entrenched")
        ef.Scenarios.Add("2. Line Breaker")
        ef.Scenarios.Add("3. Take and Hold")
        ef.Scenarios.Add("4. The Pit")
        ef.Scenarios.Add("5. Extraction")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outlast")
        ef.Scenarios.Add("8. Recon")
        ef.Scenarios.Add("9. Other")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Highlander"
        ef.Scenarios.Add("1. Entrenched")
        ef.Scenarios.Add("2. Line Breaker")
        ef.Scenarios.Add("3. Take and Hold")
        ef.Scenarios.Add("4. The Pit")
        ef.Scenarios.Add("5. Extraction")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outlast")
        ef.Scenarios.Add("8. Recon")
        ef.Scenarios.Add("9. Other")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Escalation"
        ef.Scenarios.Add("1. Entrenched")
        ef.Scenarios.Add("2. Line Breaker")
        ef.Scenarios.Add("3. Take and Hold")
        ef.Scenarios.Add("4. The Pit")
        ef.Scenarios.Add("5. Extraction")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outlast")
        ef.Scenarios.Add("8. Recon")
        ef.Scenarios.Add("9. Other")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Other"
        ef.Scenarios.Add("1. Other")
        lst.Add(ef)

        Return lst
    End Function

End Class
