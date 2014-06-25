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
        If _ActivePanel Is Nothing Then _ActivePanel = grdPanel2
        anmSlide = New ThicknessAnimation()
        anmSlide.Duration = New Duration(TimeSpan.FromSeconds(0.5))
        anmSlide.FillBehavior = FillBehavior.Stop

        anmRotate = New DoubleAnimation
        anmRotate.Duration = New Duration(TimeSpan.FromSeconds(0.5))
        anmRotate.FillBehavior = FillBehavior.Stop

        Dim NextControl As UIElement = BaseController.CurrentController.View

        grdPanel1Content.Children.Clear()
        grdPanel2Content.Children.Clear()
        
        Dim TargetMargin As Thickness
        If _IsMovingPrev Then
            'start on cell #2
            grdSlider.Margin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)

            grdPanel1Content.Children.Add(NextControl)
            If Not _LastControl Is Nothing AndAlso _LastControl IsNot NextControl Then grdPanel2Content.Children.Add(_LastControl)

            anmSlide.From = grdSlider.Margin
            TargetMargin = New Thickness(0, 0, 0, 0)

            anmRotate.From = 360
            anmRotate.To = 0
        Else
            'start on cell #1
            grdSlider.Margin = New Thickness(0, 0, 0, 0)

            If Not _LastControl Is Nothing Then grdPanel1Content.Children.Add(_LastControl)
            grdPanel2Content.Children.Add(NextControl)

            anmSlide.From = grdSlider.Margin
            TargetMargin = New Thickness(-1 * _ActivePanel.ActualWidth, 0, 0, 0)

            anmRotate.From = 0
            anmRotate.To = 360
        End If
        anmSlide.To = TargetMargin

        Dim rtl = New RotateTransform
        grdGearLeft.RenderTransform = rtl
        grdGearLeft.RenderTransformOrigin = New Point(0.5, 0.5)
        Dim rtr = New RotateTransform
        grdGearRight.RenderTransform = rtr
        grdGearRight.RenderTransformOrigin = New Point(0.5, 0.5)

        rtl.BeginAnimation(RotateTransform.AngleProperty, anmRotate)
        rtr.BeginAnimation(RotateTransform.AngleProperty, anmRotate)

        grdSlider.BeginAnimation(Grid.MarginProperty, anmSlide)
        grdSlider.Margin = TargetMargin

        _LastControl = BaseController.CurrentController.View

        Me.btnNext.IsEnabled = BaseController.CurrentController.NextEnabled
        Me.btnPrev.IsEnabled = BaseController.CurrentController.PreviousEnabled

        'Dim binding = New Binding()
        'binding.Path = New PropertyPath("Title")
        'binding.Source = Me.DataContext
        'BindingOperations.SetBinding(txtTitle, TextBlock.TextProperty, binding)


        BaseController.Model.NotifyButtonVisibilityChange()
    End Sub

    Private anmSlide As ThicknessAnimation
    Private anmRotate As DoubleAnimation

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        UpdateUI()
    End Sub

    Private Sub grdPanel2_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles grdPanel2.SizeChanged
        If _ActivePanel Is grdPanel2 Then
            grdSlider.Margin = New Thickness(-1 * grdPanel2.ActualWidth)
        End If
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