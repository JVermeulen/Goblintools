Imports System.Drawing

Namespace GRD

    ''' <summary>
    ''' Surfer 6 grid file containing data in a geographical grid.
    ''' </summary>
    Public Class GrdFile

        ''' <summary>
        ''' The default file extension for this file.
        ''' </summary>
        Public Const Extension As String = ".GRD"

        ''' <summary>
        ''' The header containing meta information.
        ''' </summary>
        Public Property Header As GrdHeader

        ''' <summary>
        ''' The values in this file.
        ''' </summary>
        Public Property Values As Single(,)

        ''' <summary>
        ''' The size of this file in bytes.
        ''' </summary>
        Public ReadOnly Property SizeInBytes As Integer
            Get
                Return GrdHeader.SizeInBytes + Me.Values.Length * 4
            End Get
        End Property

        ''' <summary>
        ''' Gets a single value from the this file.
        ''' </summary>
        ''' <param name="column">The column index.</param>
        ''' <param name="row">The row index.</param>
        Public Function GetValue(column As Integer, row As Integer) As Single
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
        ''' Gets the value of the nearest point.
        ''' </summary>
        ''' <param name="x">The x-coordinate.</param>
        ''' <param name="y">The y-coordinate.</param>
        Public Function GetValueNearestNeighbor(x As Double, y As Double) As Single
            Dim column As Double = Me.Header.GetColumn(x)
            Dim row As Double = Me.Header.GetRow(y)

            Return Me.GetValue(Math.Round(column), Math.Round(row))
        End Function

        ''' <summary>
        ''' Gets the calculated value.
        ''' </summary>
        ''' <param name="x">The x-coordinate.</param>
        ''' <param name="y">The y-coordinate.</param>
        Public Function GetValueBilinear(x As Double, y As Double) As Single
            Dim column As Double = Me.Header.GetColumn(x)
            Dim row As Double = Me.Header.GetRow(y)

            Dim c As Integer = Math.Floor(column)
            Dim r As Integer = Math.Floor(row)

            Dim width As Double = column - c
            Dim height As Double = row - r

            Dim value As Single = (1 - width) * (1 - height) * GetValue(c, r) +
                                  (1 - width) * (height) * GetValue(c, r + 1) +
                                  (width) * (1 - height) * GetValue(c + 1, r) +
                                  (width) * (height) * GetValue(c + 1, r + 1)

            Return value
        End Function

        ''' <summary>
        ''' Gets the calculated value.
        ''' </summary>
        ''' <param name="x">The x-coordinate.</param>
        ''' <param name="y">The y-coordinate.</param>
        Public Function GetValueBicubic(x As Double, y As Double) As Single
            Dim column As Double = Me.Header.GetColumn(x)
            Dim row As Double = Me.Header.GetRow(y)

            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Creates an image from the values in this file.
        ''' </summary>
        ''' <param name="useColor">When true, uses red and blue for positive and negative values.</param>
        ''' <param name="useAlpha">Use transparency.</param>
        Public Function CreateImage(useColor As Boolean, useAlpha As Boolean) As Image
            Dim result As New Bitmap(Me.Header.ColumnCount, Me.Header.RowCount)

            'When both negative and positive numbers are available, two color will be shown.
            Dim multiColor As Boolean = Me.Header.MinValue < 0 And Me.Header.MaxValue > 0

            Dim negativeFactor As Double
            Dim positiveFactor As Double

            If multiColor Then
                negativeFactor = Math.Abs(Me.Header.MinValue / Me.Header.DeltaValue)
                positiveFactor = Math.Abs(Me.Header.MaxValue / Me.Header.DeltaValue)

                If negativeFactor > positiveFactor Then
                    negativeFactor = 1
                    positiveFactor = positiveFactor / negativeFactor
                Else
                    negativeFactor = negativeFactor / positiveFactor
                    positiveFactor = 1
                End If
            End If

            For column As Integer = 0 To Me.Header.ColumnCount - 1
                For index As Integer = 0 To Me.Header.RowCount - 1
                    Dim value As Single = Me.Values(column, index)

                    Dim factor As Single
                    Dim alpha As Byte
                    Dim red As Byte
                    Dim green As Byte
                    Dim blue As Byte

                    If value < Me.Header.MinValue Or value > Me.Header.MaxValue Then
                        alpha = 0
                    Else
                        If useColor Then
                            If value > 0 Then
                                If multiColor Then
                                    factor = (value / Me.Header.MaxValue) * positiveFactor
                                Else
                                    factor = (value - Me.Header.MinValue) / (Me.Header.DeltaValue)
                                End If

                                If useAlpha Then
                                    alpha = factor * 255
                                    red = 255
                                    green = 0
                                    blue = 0
                                Else
                                    alpha = 255
                                    red = 255
                                    green = 255 - factor * 255
                                    blue = 255 - factor * 255
                                End If
                            Else
                                If multiColor Then
                                    factor = (value / Me.Header.MinValue) * negativeFactor
                                Else
                                    factor = (value - Me.Header.MinValue) / (Me.Header.DeltaValue)
                                End If

                                If useAlpha Then
                                    alpha = factor * 255
                                    red = 0
                                    green = 0
                                    blue = 255
                                Else
                                    alpha = 255
                                    red = 255 - factor * 255
                                    green = 255 - factor * 255
                                    blue = 255
                                End If
                            End If
                        Else
                            factor = (value - Me.Header.MinValue) / (Me.Header.DeltaValue)

                            If useAlpha Then
                                alpha = 255 - factor * 255
                                red = 0
                                green = 0
                                blue = 0
                            Else
                                alpha = 255
                                red = factor * 255
                                green = factor * 255
                                blue = factor * 255
                            End If
                        End If
                    End If

                    result.SetPixel(column, index, Color.FromArgb(alpha, red, green, blue))
                Next
            Next

            Return result
        End Function

    End Class

End Namespace
