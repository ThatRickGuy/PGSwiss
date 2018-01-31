Imports System.IO
Imports System.Text

Public Class LandingController
    Inherits BaseController


    Protected Overrides Function CreateNext() As BaseController
        Return Nothing
    End Function

    Public Sub New()
        _Stack.Add(Me)
        _NextEnabled = False
        _PreviousEnabled = False
        Me.View = New Landing
        Me.View.DataContext = Me
    End Sub

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 0
    End Sub

    Public ReadOnly Property Version As String
        Get
            Return My.Application.Info.Version.ToString
        End Get
    End Property

    Public Function DeserializeJSON(input As String) As List(Of ConflictChamberJSON)
        Dim cc As New List(Of ConflictChamberJSON)()
        Try
            Dim ms As New MemoryStream(Encoding.Unicode.GetBytes(input))
            Dim serializer As New System.Runtime.Serialization.Json.DataContractJsonSerializer(cc.[GetType]())
            cc = DirectCast(serializer.ReadObject(ms), List(Of ConflictChamberJSON))
            ms.Close()
            ms.Dispose()
        Catch

        End Try

        Return cc
    End Function


End Class