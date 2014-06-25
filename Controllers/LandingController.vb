﻿Public Class LandingController
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

End Class