Imports System.Drawing

Public Class GrdImage

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

                Dim r As Byte
                Dim g As Byte
                Dim b As Byte
                Dim a As Byte

                If record < file.Header.MinValue Or record > file.Header.MaxValue Then
                    a = 0
                Else
                    Dim value As Single = (record - file.Header.MinValue) / (file.Header.DeltaValue)

                    If colored Then
                        If alpha Then
                            If record > 0 Then
                                If multiColor Then
                                    value = (record / file.Header.MaxValue) * maxFactor
                                End If

                                a = value * 255
                                r = 255
                                g = 0
                                b = 0
                            Else
                                If multiColor Then
                                    value = (record / file.Header.MinValue) * minFactor
                                End If

                                a = value * 255
                                r = 0
                                g = 0
                                b = 255
                            End If
                        Else
                            If record > 0 Then
                                If multiColor Then
                                    value = (record / file.Header.MaxValue) * maxFactor
                                End If

                                a = 255
                                r = 255
                                g = 255 - value * 255
                                b = 255 - value * 255
                            Else
                                If multiColor Then
                                    value = (record / file.Header.MinValue) * minFactor
                                End If

                                a = 255
                                r = 255 - value * 255
                                g = 255 - value * 255
                                b = 255
                            End If
                        End If
                    Else
                        If alpha Then
                            If record > 0 Then
                                If multiColor Then
                                    value = (record / file.Header.MaxValue) * maxFactor
                                End If

                                a = value * 255
                                r = 255
                                g = 255
                                b = 255
                            Else
                                If multiColor Then
                                    value = (record / file.Header.MinValue) * minFactor
                                End If

                                a = value * 255
                                r = 0
                                g = 0
                                b = 0
                            End If
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
