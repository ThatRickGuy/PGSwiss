Partial Public MustInherit Class BaseController
    Public Event ActivationCompleted(sender As Object, e As EventArgs)

    Public Event ForceUIUpdate(sender As Object, e As EventArgs)

    Public Sub StartEvent(FileName As String)
        _Stack.Clear()
        Model.Load(FileName)
        Dim WMEvent As New WMEventController
        _CurrentController = WMEvent
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Public Sub OpenCollectionManager()
        _Stack.Clear()
        _CurrentController = CollectionManager
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Public Sub OpenLandingPage()
        _Stack.Clear()
        _CurrentController = Landing
        RaiseEvent ForceUIUpdate(Me, Nothing)
    End Sub

    Protected Shared _Stack As New List(Of BaseController)

    Private Shared _Landing As LandingController
    Public Shared ReadOnly Property Landing As LandingController
        Get
            If _Landing Is Nothing Then _Landing = New LandingController
            Return _Landing
        End Get
    End Property

    Private Shared _CollectionManager As CollectionManagerController
    Public Shared ReadOnly Property CollectionManager As CollectionManagerController
        Get
            If _CollectionManager Is Nothing Then _CollectionManager = New CollectionManagerController
            Return _CollectionManager
        End Get
    End Property

    Public MustOverride Function Validate() As String

    Protected MustOverride Function CreateNext() As BaseController
    Protected Overridable Sub Activated()
        RaiseEvent ActivationCompleted(Me, Nothing)
    End Sub

    Public Shared Property Model As WMEventViewModel = WMEventViewModel.GetSingleton

    Public Property View As UserControl

    Private Shared _CurrentController As BaseController
    Public Shared ReadOnly Property CurrentController() As BaseController
        Get
            If _CurrentController Is Nothing Then _CurrentController = _Stack.First
            Return _CurrentController
        End Get
    End Property

    Public Overridable Function MoveNext() As BaseController
        If Me._NextEnabled Then
            Dim sValidated = Validate()
            If sValidated = String.Empty Then
                Model.Save()
                If _Stack.Count = _Stack.IndexOf(Me) + 1 Then
                    Dim NextControl = CreateNext()
                    If Not NextControl Is Nothing Then _Stack.Add(NextControl)
                End If
                If _Stack.IndexOf(Me) + 1 < _Stack.Count Then
                    _CurrentController = _Stack.Item(_Stack.IndexOf(Me) + 1)
                    _CurrentController.Activated()
                Else
                    _CurrentController = Me
                End If
                Return _CurrentController
            Else
                MessageBox.Show("Please correct the following issue(s) before continuing:" & ControlChars.CrLf & ControlChars.CrLf & sValidated)
            End If
        End If
    End Function

    Public Overridable Function MovePrev() As BaseController
        If Me._PreviousEnabled Then
            If _Stack.IndexOf(Me) > 0 Then
                _CurrentController = _Stack.Item(_Stack.IndexOf(Me) - 1)
            End If
            _CurrentController.Activated()
            Return _CurrentController
        End If
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
