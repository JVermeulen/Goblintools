Namespace GRD

    ''' <summary>
    ''' The header of a GRD file.
    ''' </summary>
    Public Class GrdHeader

        ''' <summary>
        ''' A code identifying the GRD file.
        ''' </summary>
        Public Const Code As String = "DSBB"

        ''' <summary>
        ''' The size of this object in bytes.
        ''' </summary>
        Public Const SizeInBytes As Integer = 56

        ''' <summary>
        ''' The number of columns.
        ''' </summary>
        Public Property ColumnCount As Int32

        ''' <summary>
        ''' The number of rows.
        ''' </summary>
        Public Property RowCount As Int32

        ''' <summary>
        ''' The lowest x-coordinate.
        ''' </summary>
        Public Property MinX As Double

        ''' <summary>
        ''' The highest x-coordinate.
        ''' </summary>
        Public Property MaxX As Double

        ''' <summary>
        ''' The lowest y-coordinate.
        ''' </summary>
        Public Property MinY As Double

        ''' <summary>
        ''' The highest y-coordinate.
        ''' </summary>
        Public Property MaxY As Double

        ''' <summary>
        ''' The lowest value.
        ''' </summary>
        Public Property MinValue As Double

        ''' <summary>
        ''' The highest value.
        ''' </summary>
        Public Property MaxValue As Double

        ''' <summary>
        ''' The total number of values.
        ''' </summary>
        Public ReadOnly Property ValueCount As Int32
            Get
                Return (Me.ColumnCount * Me.RowCount)
            End Get
        End Property

        ''' <summary>
        ''' The difference between the highest and lowest x-coordinate.
        ''' </summary>
        Public ReadOnly Property DeltaX As Double
            Get
                Return (Me.MaxX - Me.MinX)
            End Get
        End Property

        ''' <summary>
        ''' The difference between the highest and lowest y-coordinate.
        ''' </summary>
        Public ReadOnly Property DeltaY As Double
            Get
                Return (Me.MaxY - Me.MinY)
            End Get
        End Property

        ''' <summary>
        ''' The difference between the highest and lowest value.
        ''' </summary>
        Public ReadOnly Property DeltaValue As Double
            Get
                Return (Me.MaxValue - Me.MinValue)
            End Get
        End Property

        ''' <summary>
        ''' The distance per column.
        ''' </summary>
        Public ReadOnly Property XResolution As Double
            Get
                Return (Me.DeltaX / (Me.ColumnCount - 1))
            End Get
        End Property

        ''' <summary>
        ''' The distance per row.
        ''' </summary>
        Public ReadOnly Property YResolution As Double
            Get
                Return (Me.DeltaY / (Me.RowCount - 1))
            End Get
        End Property

        ''' <summary>
        ''' The index of the given column and row.
        ''' </summary>
        ''' <param name="column">The column of the grid.</param>
        ''' <param name="row">The row of the grid.</param>
        Public Function GetIndex(column As Integer, row As Integer) As Integer
            Return (column * Me.ColumnCount + row)
        End Function

        ''' <summary>
        ''' Gets the column of the given x-coordinate. The value can be between two columns.
        ''' </summary>
        ''' <param name="x">The x-coordinate.</param>
        Public Function GetColumn(x As Integer) As Double
            If x < Me.MinX Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter x is too low.")
            End If
            If x > Me.MaxX Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter x is too high.")
            End If

            Return (x - Me.MinX) / XResolution
        End Function

        ''' <summary>
        ''' Gets the column of the given y-coordinate. The value can be between two row.
        ''' </summary>
        ''' <param name="y">The y-coordinate.</param>
        Public Function GetRow(y As Integer) As Double
            If y < Me.MinY Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter y is too low.")
            End If
            If y > Me.MaxY Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter y is too high.")
            End If

            Return (y - Me.MinY) / YResolution
        End Function

    End Class

End Namespace
