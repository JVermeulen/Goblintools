Imports System.Drawing

Namespace CLR

    ''' <summary>
    ''' http://georezo.net/jparis/MI_Enviro/Colors/colors_and_mapinfo.htm
    ''' </summary>
    Public Class ClrFile

        ''' <summary>
        ''' The default file extension for this file.
        ''' </summary>
        Public Const Extension As String = ".CLR"

        ''' <summary>
        ''' The header containing meta information.
        ''' </summary>
        Public Property Header As ClrHeader

        ''' <summary>
        ''' The values in this file.
        ''' </summary>
        Public Property Values As Int32(,)

        ''' <summary>
        ''' The size of this file in bytes.
        ''' </summary>
        Public ReadOnly Property SizeInBytes As Integer
            Get
                Return ClrHeader.SizeInBytes + Me.Values.Length * 4
            End Get
        End Property

        ''' <summary>
        ''' Gets a single value from the this file.
        ''' </summary>
        ''' <param name="column">The column index.</param>
        ''' <param name="row">The row index.</param>
        Public Function GetValue(column As Integer, row As Integer) As Int32
            'Validate column
            If column < 0 Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter column is too low.")
            End If
            If column > Me.Header.ColumnCount Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter column is too high.")
            End If

            'Validate row
            If row < 0 Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter row is too low.")
            End If
            If row > Me.Header.RowCount Then
                Throw New ArgumentOutOfRangeException("Unable to get value. Parameter row is too high.")
            End If

            Return Me.Values(column, row)
        End Function

        ''' <summary>
        ''' Converts the given .NET color to a MI color.
        ''' </summary>
        ''' <param name="value">A .NET color.</param>
        Public Shared Function ToMapColor(value As Color) As Integer
            Return value.R * 256 * 256 + value.G * 256 + value.B
        End Function

        ''' <summary>
        ''' Converts the given MI color to a .NET color.
        ''' </summary>
        ''' <param name="value">A MI color.</param>
        Public Shared Function FromMapColor(value As Integer) As Color
            Dim blue As Byte = value Mod 256
            Dim green As Byte = (value - blue) / 256 Mod 256
            Dim red As Byte = (value - green - blue) / 256 ^ 2 Mod 256

            Return Color.FromArgb(red, green, blue)
        End Function

    End Class

End Namespace
