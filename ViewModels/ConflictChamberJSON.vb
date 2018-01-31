Public Class ConflictChamberJSON
    Public Property name As String
    Public Property faction As String
    Public Property cccode As String
    Public Property list1 As WMList
    Public Property list2 As WMList

    Public Class WMList
        Public Property theme As String
        Public Property list As List(Of String)
    End Class
End Class





