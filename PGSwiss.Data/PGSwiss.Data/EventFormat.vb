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
                Me.AddRange(From p In lst Order By p.Name)
            End Using
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
        Dim ef As New doEventFormat
        ef.Name = "2015 Steamroller"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
        ef.Scenarios.Add("8. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2015 Hardcore"
        ef.Scenarios.Add("1. Death Match")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2015 Masters"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
        ef.Scenarios.Add("8. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "2015-2016 Iron Gauntlet"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
        ef.Scenarios.Add("8. Recon")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Who's the Boss"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
        ef.Scenarios.Add("8. Recon")
        ef.Scenarios.Add("9. Other")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Highlander"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
        ef.Scenarios.Add("8. Recon")
        ef.Scenarios.Add("9. Other")
        lst.Add(ef)

        ef = New doEventFormat
        ef.Name = "Escalation"
        ef.Scenarios.Add("1. Destruction")
        ef.Scenarios.Add("2. Two Fronts")
        ef.Scenarios.Add("3. Close Quarters")
        ef.Scenarios.Add("4. Fire Support")
        ef.Scenarios.Add("5. Incoming")
        ef.Scenarios.Add("6. Incursion")
        ef.Scenarios.Add("7. Outflank")
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
