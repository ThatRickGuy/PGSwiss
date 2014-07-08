Imports System.Windows.Threading
Imports System.ComponentModel

Public Class Clock
    Implements INotifyPropertyChanged

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tmrUpate.Interval = TimeSpan.FromSeconds(1)
        Me.DataContext = Me
        Me.grdPause.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private _SetDuration As TimeSpan
    Public Property SetDuration As TimeSpan
        Get
            Return _SetDuration
        End Get
        Set(value As TimeSpan)
            _SetDuration = value
            Duration = value
            OnPropertyChanged("Duration")
        End Set
    End Property
    Public Property Duration As TimeSpan
    Private TimerIsRunning As Boolean = False
    Private IsResetting As Boolean = False
    Private WithEvents tmrUpate As New DispatcherTimer

    Private MouseDownTime As DateTime
    Private Sub recButton_MouseDown(sender As Object, e As Windows.Input.MouseButtonEventArgs) Handles recButton.MouseDown
        MouseDownTime = Now
    End Sub

    Private Sub recButton_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles recButton.MouseUp
        If (Now - MouseDownTime).TotalSeconds > 5 Then
            tmrUpate.Stop()
            TimerIsRunning = False
            IsResetting = True
            Me.Duration = Me.SetDuration
            OnPropertyChanged("Duration")
            IsResetting = False
            txtTimerHours.IsReadOnly = False
            txtTimerMinutes.IsReadOnly = False
            txtTimerSeconds.IsReadOnly = False
            txtTimerHours.IsTabStop = True
            txtTimerMinutes.IsTabStop = True
            txtTimerSeconds.IsTabStop = True
            Me.grdPause.Visibility = Windows.Visibility.Collapsed
            Me.grdPlay.Visibility = Windows.Visibility.Visible
        Else
            If TimerIsRunning Then
                'stop
                tmrUpate.Stop()
                txtTimerHours.IsReadOnly = False
                txtTimerMinutes.IsReadOnly = False
                txtTimerSeconds.IsReadOnly = False
                txtTimerHours.IsTabStop = True
                txtTimerMinutes.IsTabStop = True
                txtTimerSeconds.IsTabStop = True
                Me.grdPause.Visibility = Windows.Visibility.Collapsed
                Me.grdPlay.Visibility = Windows.Visibility.Visible
            Else
                'resume
                tmrUpate.Start()
                txtTimerHours.IsReadOnly = True
                txtTimerMinutes.IsReadOnly = True
                txtTimerSeconds.IsReadOnly = True
                txtTimerHours.IsTabStop = False
                txtTimerMinutes.IsTabStop = False
                txtTimerSeconds.IsTabStop = False
                Me.grdPause.Visibility = Windows.Visibility.Visible
                Me.grdPlay.Visibility = Windows.Visibility.Collapsed
            End If
            TimerIsRunning = Not TimerIsRunning
        End If
    End Sub

    Private Sub txtTimer_GotFocus(sender As Object, e As RoutedEventArgs) Handles txtTimerHours.GotFocus, txtTimerMinutes.GotFocus, txtTimerSeconds.GotFocus
        CType(sender, TextBox).SelectAll()
    End Sub

    Private Sub txtTimerHours_PreviewMouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles txtTimerHours.PreviewMouseLeftButtonUp
        CType(sender, TextBox).SelectAll()
    End Sub

    Private Sub txtTimerHours_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles txtTimerHours.PreviewTextInput
        If IsNumeric(e.Text) OrElse e.Text = String.Empty Then
            e.Handled = True
        Else
            e.Handled = False
        End If
    End Sub

    Private Sub txtTimer_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtTimerHours.TextChanged, txtTimerMinutes.TextChanged, txtTimerSeconds.TextChanged
        If Not TimerIsRunning AndAlso Not IsResetting Then

            Dim Seconds As Integer = 0
            Dim Minutes As Integer = 0
            Dim Hours As Integer = 0

            Integer.TryParse(txtTimerHours.Text, Hours)
            Integer.TryParse(txtTimerMinutes.Text, Minutes)
            Integer.TryParse(txtTimerSeconds.Text, Seconds)

            Me.SetDuration = New TimeSpan(Hours, Minutes, Seconds)
            Me.Duration = Me.SetDuration
        End If
    End Sub

    Private Sub tmrUpate_Tick(sender As Object, e As EventArgs) Handles tmrUpate.Tick
        Me.Duration -= TimeSpan.FromSeconds(1)
        If Me.Duration = TimeSpan.FromSeconds(0) Then
            tmrUpate.Stop()
            Me.Duration = Me.SetDuration
            txtTimerHours.IsReadOnly = False
            txtTimerMinutes.IsReadOnly = False
            txtTimerSeconds.IsReadOnly = False
            txtTimerHours.IsTabStop = True
            txtTimerMinutes.IsTabStop = True
            txtTimerSeconds.IsTabStop = True

            Me.grdPause.Visibility = Windows.Visibility.Collapsed
            Me.grdPlay.Visibility = Windows.Visibility.Visible
            TimerIsRunning = False
            Beep()
            System.Threading.Thread.Sleep(300)
            Beep()
            System.Threading.Thread.Sleep(300)
            Beep()

        End If
        OnPropertyChanged("Duration")
    End Sub

    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


    Private Sub btnAddHours_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnAddHours.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerHours.Text = CInt(txtTimerHours.Text) + 1
    End Sub

    Private Sub btnAddMinutes_Click(sender As Object, e As RoutedEventArgs) Handles btnAddMinutes.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerMinutes.Text = CInt(txtTimerMinutes.Text) + 1
    End Sub

    Private Sub btnAddSeconds_Click(sender As Object, e As RoutedEventArgs) Handles btnAddSeconds.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerSeconds.Text = CInt(txtTimerSeconds.Text) + 1
    End Sub

    Private Sub btnSubtractHours_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnSubtractHours.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerHours.Text = CInt(txtTimerHours.Text) - 1
    End Sub

    Private Sub btnSubtractMinutes_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnSubtractMinutes.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerMinutes.Text = CInt(txtTimerMinutes.Text) - 1
    End Sub

    Private Sub btnSubtractSeconds_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnSubtractSeconds.Click
        If Not TimerIsRunning AndAlso Not IsResetting Then txtTimerSeconds.Text = CInt(txtTimerSeconds.Text) - 1
    End Sub
End Class


Public Class TimeSpanToIntervalConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.Convert
        Dim iReturn As Nullable(Of Integer)
        Select Case parameter
            Case Is = "Hours"
                iReturn = CType(value, TimeSpan).Hours
            Case Is = "Minutes"
                iReturn = CType(value, TimeSpan).Minutes
            Case Is = "Seconds"
                iReturn = CType(value, TimeSpan).Seconds
        End Select
        Return iReturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements Windows.Data.IValueConverter.ConvertBack
        Return Nothing 'don't care!
    End Function
End Class

