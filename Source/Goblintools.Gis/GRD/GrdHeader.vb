Public Class GrdHeader

    Public Const Code As String = "DSBB"

    ''' <summary>
    ''' The size of this object in bytes.
    ''' </summary>
    Public Const SizeInBytes As Integer = 56

    Public Property Width As Int32
    Public Property Height As Int32
    Public Property MinX As Double
    Public Property MaxX As Double
    Public Property MinY As Double
    Public Property MaxY As Double
    Public Property MinValue As Double
    Public Property MaxValue As Double

    Public ReadOnly Property NumberOfValues As Int32
        Get
            Return (Me.Width * Me.Height)
        End Get
    End Property

    Public ReadOnly Property DeltaX As Double
        Get
            Return (Me.MaxX - Me.MinX)
        End Get
    End Property

    Public ReadOnly Property DeltaY As Double
        Get
            Return (Me.MaxY - Me.MinY)
        End Get
    End Property

    Public ReadOnly Property DeltaValue As Double
        Get
            Return (Me.MaxValue - Me.MinValue)
        End Get
    End Property

    Public ReadOnly Property XResolution As Double
        Get
            Return (Me.DeltaX / (Me.Width - 1))
        End Get
    End Property

    Public ReadOnly Property YResolution As Double
        Get
            Return (Me.DeltaY / (Me.Height - 1))
        End Get
    End Property

    Public Function GetIndex(w As Integer, h As Integer) As Integer
        Return (w * Me.Width + h)
    End Function

    Public Function GetW(x As Integer) As Double
        If x < Me.MinX Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter x is too low.")
        If x > Me.MaxX Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter x is too high.")

        Return (x - Me.MinX) / XResolution
    End Function

    Public Function GetH(y As Integer) As Double
        If y < Me.MinY Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter y is too low.")
        If y > Me.MaxY Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter y is too high.")

        Return (y - Me.MinY) / YResolution
    End Function

End Class
