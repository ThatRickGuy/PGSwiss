Public Class ProgressBar
    Public Shared ReadOnly ValueProperty As DependencyProperty =
    DependencyProperty.Register("Value",
                                GetType(Integer),
                                GetType(ProgressBar))

    Public Property Value() As Integer
        Get
            Return CBool(GetValue(ValueProperty))
        End Get
        Set(ByVal value As Integer)
            SetValue(ValueProperty, value)
        End Set
    End Property
End Class
