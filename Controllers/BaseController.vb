Partial Public MustInherit Class BaseController
    Public Event ActivationCompleted(sender As Object, e As EventArgs)

    Public Event ForceUIUpdate(sender As Object, e As EventArgs)

    Public Sub StartEvent(FileName As String)
        _Stack.Clear()
        Model.Load(FileName)
        Dim WMEvent As New WMEventController
        _Stack.Add(WMEvent)
        _CurrentController = WMEvent
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub


    Protected Shared _Stack As New List(Of BaseController)
    Protected MustOverride Function CreateNext() As BaseController
    Protected Overridable Sub Activated()
        RaiseEvent ActivationCompleted(Me, Nothing)
    End Sub

    Public Shared Property Model As WMEventViewModel

    Public Property View As UserControl

    Private Shared _CurrentController As BaseController
    Public Shared ReadOnly Property CurrentController() As BaseController
        Get
            If _CurrentController Is Nothing Then _CurrentController = _Stack.First
            Return _CurrentController
        End Get
    End Property

    Public Overridable Function MoveNext() As BaseController
        Model.Save()
        If _Stack.Count = _Stack.IndexOf(Me) + 1 Then
            _Stack.Add(CreateNext)
        End If
        _CurrentController = _Stack.Item(_Stack.IndexOf(Me) + 1)
        _CurrentController.Activated()
        Return _CurrentController
    End Function

    Public Overridable Function MovePrev() As BaseController
        If _Stack.IndexOf(Me) > 0 Then
            _CurrentController = _Stack.Item(_Stack.IndexOf(Me) - 1)
        End If
        _CurrentController.Activated()
        Return _CurrentController
    End Function

    Protected _PreviousEnabled As Boolean = True
    ReadOnly Property PreviousEnabled As Boolean
        Get
            Return _PreviousEnabled
        End Get
    End Property

    Protected _NextEnabled As Boolean = True
    ReadOnly Property NextEnabled As Boolean
        Get
            Return _NextEnabled
        End Get
    End Property
End Class
