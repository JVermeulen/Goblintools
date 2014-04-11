Imports System.Drawing

''' <summary>
''' An image representation of a GRD file.
''' </summary>
Public Class GrdImage

    ''' <summary>
    ''' Creates an image from the values in GRD file.
    ''' </summary>
    ''' <param name="file">The source data.</param>
    ''' <param name="colored">When true, uses red and blue for positive and negative values.</param>
    ''' <param name="alpha">Use transparency.</param>
    Public Shared Function CreateImage(file As GrdFile, colored As Boolean, alpha As Boolean) As Image
        Dim result As New Bitmap(file.Header.SizeX, file.Header.SizeY)

        Dim multiColor As Boolean = file.Header.MinValue < 0 And file.Header.MaxValue > 0

        Dim minFactor As Double
        Dim maxFactor As Double

        If multiColor Then
            minFactor = Math.Abs(file.Header.MinValue / file.Header.DeltaValue)
            maxFactor = Math.Abs(file.Header.MaxValue / file.Header.DeltaValue)

            If minFactor > maxFactor Then
                minFactor = 1
                maxFactor = maxFactor / minFactor
            Else
                minFactor = minFactor / maxFactor
                maxFactor = 1
            End If
        End If

        For x As Integer = 0 To file.Header.SizeX - 1
            For y As Integer = 0 To file.Header.SizeY - 1
                Dim i As Integer = x * file.Header.SizeX + y

                Dim record As Single = file.Records(i)

                Dim value As Single
                Dim a As Byte
                Dim r As Byte
                Dim g As Byte
                Dim b As Byte

                If record < file.Header.MinValue Or record > file.Header.MaxValue Then
                    a = 0
                Else
                    If colored Then
                        If record > 0 Then
                            If multiColor Then
                                value = (record / file.Header.MaxValue) * maxFactor
                            Else
                                value = (record - file.Header.MinValue) / (file.Header.DeltaValue)
                            End If

                            If alpha Then
                                a = value * 255
                                r = 255
                                g = 0
                                b = 0
                            Else
                                a = 255
                                r = 255
                                g = 255 - value * 255
                                b = 255 - value * 255
                            End If
                        Else
                            If multiColor Then
                                value = (record / file.Header.MinValue) * minFactor
                            Else
                                value = (record - file.Header.MinValue) / (file.Header.DeltaValue)
                            End If

                            If alpha Then
                                a = value * 255
                                r = 0
                                g = 0
                                b = 255
                            Else
                                a = 255
                                r = 255 - value * 255
                                g = 255 - value * 255
                                b = 255
                            End If
                        End If
                    Else
                        value = (record - file.Header.MinValue) / (file.Header.DeltaValue)

                        If alpha Then
                            a = 255 - value * 255
                            r = 0
                            g = 0
                            b = 0
                        Else
                            a = 255
                            r = value * 255
                            g = value * 255
                            b = value * 255
                        End If
                    End If
                End If

                result.SetPixel(x, y, Color.FromArgb(a, r, g, b))
            Next
        Next

        Return result
    End Function

End Class
