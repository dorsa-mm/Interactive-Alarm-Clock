Public Class DigitLED
    Inherits Label

    Private _maxDigit As Integer

    ' MaxDigit property to restrict valid digits
    Public Property MaxDigit As Integer
        Get
            Return _maxDigit
        End Get
        Set(value As Integer)
            _maxDigit = value
        End Set
    End Property

    ' Method to set the digit
    Public Sub SetDigit(value As Integer)
        If value <= _maxDigit Then
            Me.Text = value.ToString()
        End If
    End Sub
End Class
