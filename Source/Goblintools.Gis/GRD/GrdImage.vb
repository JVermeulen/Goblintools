Imports System.Drawing

''' <summary>
''' An graphical representation of a GRD file.
''' </summary>
Public Class GrdImage

    ''' <summary>
    ''' Creates an image from the values in GRD file.
    ''' </summary>
    ''' <param name="file">The source data.</param>
    ''' <param name="useColor">When true, uses red and blue for positive and negative values.</param>
    ''' <param name="useAlpha">Use transparency.</param>
    Public Shared Function CreateImage(file As GrdFile, useColor As Boolean, useAlpha As Boolean) As Image
        Dim result As New Bitmap(file.Header.Width, file.Header.Height)

        Dim multiColor As Boolean = file.Header.MinValue < 0 And file.Header.MaxValue > 0

        Dim negativeFactor As Double
        Dim positiveFactor As Double

        If multiColor Then
            negativeFactor = Math.Abs(file.Header.MinValue / file.Header.DeltaValue)
            positiveFactor = Math.Abs(file.Header.MaxValue / file.Header.DeltaValue)

            If negativeFactor > positiveFactor Then
                negativeFactor = 1
                positiveFactor = positiveFactor / negativeFactor
            Else
                negativeFactor = negativeFactor / positiveFactor
                positiveFactor = 1
            End If
        End If

        For w As Integer = 0 To file.Header.Width - 1
            For h As Integer = 0 To file.Header.Height - 1
                Dim value As Single = file.Values(w, h)

                Dim factor As Single
                Dim alpha As Byte
                Dim red As Byte
                Dim green As Byte
                Dim blue As Byte

                If value < file.Header.MinValue Or value > file.Header.MaxValue Then
                    alpha = 0
                Else
                    If useColor Then
                        If value > 0 Then
                            If multiColor Then
                                factor = (value / file.Header.MaxValue) * positiveFactor
                            Else
                                factor = (value - file.Header.MinValue) / (file.Header.DeltaValue)
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
                                factor = (value / file.Header.MinValue) * negativeFactor
                            Else
                                factor = (value - file.Header.MinValue) / (file.Header.DeltaValue)
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
                        factor = (value - file.Header.MinValue) / (file.Header.DeltaValue)

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

                result.SetPixel(w, h, Color.FromArgb(alpha, red, green, blue))
            Next
        Next

        Return result
    End Function

End Class
