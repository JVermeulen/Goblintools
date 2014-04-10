Public Class GrdHeader

    Public Const Code As String = "DSBB"

    ''' <summary>
    ''' The size of this object in bytes.
    ''' </summary>
    Public Const SizeInBytes As Integer = 56

    Public Property SizeX As Int32
    Public Property SizeY As Int32
    Public Property MinX As Double
    Public Property MaxX As Double
    Public Property MinY As Double
    Public Property MaxY As Double
    Public Property MinValue As Double
    Public Property MaxValue As Double

    Public ReadOnly Property NumberOfRecords As Int32
        Get
            Return Me.SizeX * Me.SizeY
        End Get
    End Property

    Public ReadOnly Property DeltaX As Double
        Get
            Return Me.MaxX - Me.MinX
        End Get
    End Property

    Public ReadOnly Property DeltaY As Double
        Get
            Return Me.MaxY - Me.MinY
        End Get
    End Property

    Public ReadOnly Property DeltaValue As Double
        Get
            Return Me.MaxValue - Me.MinValue
        End Get
    End Property

End Class
