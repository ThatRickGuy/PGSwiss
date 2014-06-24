Public Class CollectionManagerController
    Inherits BaseController

    Public Sub New()
        _NextEnabled = False
        _PreviousEnabled = False
        Me.View = New CollectionManager
        Me.View.DataContext = Me
        _Stack.Add(Me)

        _EventFormats.load()
        _Factions.load()
        _Metas.load()
        _Players.load()
        _Sizes.load()
        _Scenarios.load()

    End Sub

    Public Sub Save()
        _EventFormats.Save()
        _Factions.Save()
        _Metas.Save()
        _Players.Save()
        _Sizes.Save()
        _Scenarios.Save()
    End Sub

    Private _EventFormats As New doEventFormatCollection
    Public ReadOnly Property EventFormats As doEventFormatCollection
        Get
            Return _EventFormats
        End Get
    End Property

    Private _Factions As New doFactionCollection
    Public ReadOnly Property Factions As doFactionCollection
        Get
            Return _Factions
        End Get
    End Property

    Private _Metas As New doMetaCollection
    Public ReadOnly Property Metas As doMetaCollection
        Get
            Return _Metas
        End Get
    End Property

    Private _Players As New doPlayerCollection
    Public ReadOnly Property Players As doPlayerCollection
        Get
            Return _Players
        End Get
    End Property

    Private _Scenarios As New doScenarioCollection
    Public ReadOnly Property Scenarios As doScenarioCollection
        Get
            Return _Scenarios
        End Get
    End Property

    Private _Sizes As New doRoundSizeCollection
    Public ReadOnly Property Sizes As doRoundSizeCollection
        Get
            Return _Sizes
        End Get
    End Property

    Protected Overrides Function CreateNext() As BaseController
        Return Nothing
    End Function

    Public Overrides Function Validate() As String
        Return String.Empty
    End Function

    Protected Overrides Sub Activated()
        MyBase.Activated()
        Model.CurrentProgress = 0
    End Sub
End Class
