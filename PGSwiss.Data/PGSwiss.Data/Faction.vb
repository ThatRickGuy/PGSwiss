Imports System.IO
Imports System.Xml.Serialization

Public Class doFactionCollection
    Inherits List(Of String)

    Public Function load() As String
        Dim sReturn As String = String.Empty
        Me.Clear()
        If Not IO.File.Exists("FactionCollection.xml") Then
            Try
                Me.AddRange(From p In Generate() Order By p)
            Catch exc As Exception
                sReturn = (exc.Message)
            End Try
            Save()
        Else
            Using objStreamReader As New StreamReader("FactionCollection.xml")
                Dim x As New XmlSerializer(Me.GetType)
                Dim lst As New List(Of String)
                lst.AddRange(x.Deserialize(objStreamReader))
                Me.AddRange(From p In lst Order By p)
            End Using
        End If
        Return sReturn
    End Function

    Private _FailedSave As Boolean = False
    Public Sub Save()
        Try
            Using objStreamWriter As New StreamWriter("FactionCollection.xml")
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

    Private Function Generate() As IEnumerable(Of String)
        Dim lst As New List(Of String)
        lst.Add("")
        lst.Add("Khador")
        lst.Add("Protectorate of Menoth")
        lst.Add("Cygnar")
        lst.Add("Cryx")
        lst.Add("Mercenaries")
        lst.Add("Convergence of Cyriss")
        lst.Add("Retribution of Scyrah")
        lst.Add("Trollblood")
        lst.Add("Circle Orboros")
        lst.Add("Legion of Everblight")
        lst.Add("Skorne")
        lst.Add("Minions")
        Return lst
    End Function

End Class
