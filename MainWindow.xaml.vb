Imports System.Windows.Media.Animation

Class MainWindow
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.DataContext = (BaseController.Model)
        AddHandler BaseController.Landing.ForceUIUpdate, AddressOf UpdateUI
        AddHandler BaseController.CollectionManager.ForceUIUpdate, AddressOf UpdateUI
    End Sub

    Private Sub btnNext_Click(sender As Object, e As RoutedEventArgs) Handles btnNext.Click
        If BaseController.CurrentController.MoveNext() IsNot Nothing Then
            _IsMovingPrev = False
            UpdateUI()
        End If
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As RoutedEventArgs) Handles btnPrev.Click
        If BaseController.CurrentController.MovePrev() IsNot Nothing Then
            _IsMovingPrev = True
            UpdateUI()
        End If
    End Sub

    Private _IsMovingPrev As Boolean = False
    Private _ActivePanel As Grid = grdPanel1
    Private _LastControl As UIElement

    Private Sub UpdateUI()
        'anmSlide = New ThicknessAnimation()
        'anmSlide.Duration = New Duration(TimeSpan.FromSeconds(1))
        'anmSlide.FillBehavior = FillBehavior.Stop

        'Dim NextControl As UIElement = BaseController.CurrentController.View

        'Try
        '    For Each p In grdPanel1.Children.OfType(Of UserControl)()
        '        grdPanel1.Children.Remove(p)
        '    Next
        '    For Each p In grdPanel2.Children.OfType(Of UserControl)()
        '        grdPanel2.Children.Remove(p)
        '    Next
        'Catch exc As Exception
        '    MessageBox.Show(exc.Message)
        'End Try
        'Dim TargetMargin As Thickness
        'If _IsMovingPrev Then
        '    'start on cell #2
        '    grdSlider.Margin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)

        '    grdPanel1.Children.Add(NextControl)
        '    grdPanel2.Children.Add(_LastControl)

        '    anmSlide.From = grdSlider.Margin
        '    TargetMargin = New Thickness(0, 0, 0, 0)
        'Else
        '    'start on cell #1
        '    grdSlider.Margin = New Thickness(0, 0, 0, 0)

        '    grdPanel1.Children.Add(NextControl)
        '    grdPanel2.Children.Add(_LastControl)

        '    anmSlide.From = grdSlider.Margin
        '    TargetMargin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)
        'End If
        'anmSlide.To = TargetMargin

        'grdSlider.BeginAnimation(Grid.MarginProperty, anmNext)
        'grdSlider.Margin = TargetMargin

        '_LastControl = BaseController.CurrentController.View




        'Exit Sub


        If _IsMovingPrev Then
            'make sure panel2 has the control the user is moving from
            If _ActivePanel Is grdPanel1 Then
                Dim content = grdPanel1.Children(grdPanel1.Children.Count - 1)
                grdPanel1.Children.RemoveAt(grdPanel1.Children.Count - 1)
                grdPanel2.Children.RemoveAt(grdPanel2.Children.Count - 1)
                grdPanel2.Children.Add(content)
            End If

            'grdPanel1.Children.RemoveAt(grdPanel1.Children.Count - 1)
            grdPanel1.Children.Add(BaseController.CurrentController.View)
            grdSlider.Margin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)
            _ActivePanel = grdPanel1

            If anmPrev Is Nothing Then
                anmNext = New ThicknessAnimation()
                anmNext.From = grdSlider.Margin
                anmNext.To = New Thickness(0)
                anmNext.Duration = New Duration(TimeSpan.FromSeconds(1))
                anmNext.FillBehavior = FillBehavior.Stop
            End If

            grdSlider.BeginAnimation(Grid.MarginProperty, anmNext)
            grdSlider.Margin = New Thickness(0)
        Else
            'make sure panel1 has the control the user is moving from
            If _ActivePanel Is grdPanel2 Then
                Dim content = grdPanel2.Children(grdPanel2.Children.Count - 1)
                grdPanel1.Children.RemoveAt(grdPanel1.Children.Count - 1)
                grdPanel2.Children.RemoveAt(grdPanel2.Children.Count - 1)
                grdPanel1.Children.Add(content)
            End If
            'If i > 0 Then
            '    ' grdPanel2.Children.RemoveAt(grdPanel2.Children.Count - 1)
            '    i += 1
            'End If
            grdPanel2.Children.Add(BaseController.CurrentController.View)
            grdSlider.Margin = New Thickness(0)
            _ActivePanel = grdPanel2

            If anmNext Is Nothing Then
                anmNext = New ThicknessAnimation()
                anmNext.From = grdSlider.Margin
                anmNext.To = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)
                anmNext.Duration = New Duration(TimeSpan.FromSeconds(1))
                anmNext.FillBehavior = FillBehavior.Stop

                anmRotateNext = New DoubleAnimation
                anmRotateNext.From = 0
                anmRotateNext.To = 360
                anmRotateNext.Duration = New Duration(TimeSpan.FromSeconds(1))
                anmRotateNext.FillBehavior = FillBehavior.Stop

            End If

            grdSlider.BeginAnimation(Grid.MarginProperty, anmNext)
            Dim rtl = New RotateTransform
            grdGearLeft.RenderTransform = rtl
            grdGearLeft.RenderTransformOrigin = New Point(0.5, 0.5)
            rtl.BeginAnimation(RotateTransform.AngleProperty, anmRotateNext)
            Dim rtr = New RotateTransform
            grdGearRight.RenderTransform = rtr
            grdGearRight.RenderTransformOrigin = New Point(0.5, 0.5)
            rtr.BeginAnimation(RotateTransform.AngleProperty, anmRotateNext)

            grdSlider.Margin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)

        End If
        Me.btnNext.IsEnabled = BaseController.CurrentController.NextEnabled
        Me.btnPrev.IsEnabled = BaseController.CurrentController.PreviousEnabled
    End Sub

    Private anmSlide As ThicknessAnimation
    Private anmRotate As DoubleAnimation

    Private i As Integer

    Private anmNext As ThicknessAnimation
    Private anmPrev As ThicknessAnimation
    Private anmRotateNext As DoubleAnimation
    Private anmRotatePrev As DoubleAnimation


    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        UpdateUI()
    End Sub
End Class


Class ScrollBarAnimater
    Inherits UIElement
    Public Shared ReadOnly ScrollOffsetProperty As DependencyProperty = DependencyProperty.Register("ScrollOffset", GetType(Double), GetType(ScrollBarAnimater), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnScrollOffsetChanged)))

    Private sv As ScrollViewer

    Public Property ScrollOffset() As Double
        Get
            Return CDbl(GetValue(ScrollOffsetProperty))
        End Get
        Set(value As Double)
            SetValue(ScrollOffsetProperty, value)
        End Set
    End Property

    Private Shared Sub OnScrollOffsetChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim myObj As ScrollBarAnimater = TryCast(obj, ScrollBarAnimater)

        If myObj IsNot Nothing Then
            myObj.sv.ScrollToHorizontalOffset(myObj.ScrollOffset)
        End If
    End Sub

    Public Sub New(scrollViewer As ScrollViewer, doubleAnimation As DoubleAnimation)
        sv = scrollViewer
        Me.BeginAnimation(ScrollBarAnimater.ScrollOffsetProperty, doubleAnimation)
    End Sub
End Class