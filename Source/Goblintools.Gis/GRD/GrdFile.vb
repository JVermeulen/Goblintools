Public Class GrdFile

    Public Const Extension As String = ".GRD"

    Public Property Header As GrdHeader
    Public Property Values As Single(,)

    Public ReadOnly Property SizeInBytes As Integer
        Get
            Return GrdHeader.SizeInBytes + Values.Length * 4
        End Get
    End Property

    Public Function GetValue(w As Integer, h As Integer) As Single
        If w < 0 Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter w is too low.")
        If w > Me.Header.Width Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter w is too high.")

        If h < 0 Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter h is too low.")
        If h > Me.Header.Height Then Throw New ArgumentOutOfRangeException("Unable to get value. Parameter h is too high.")

        Return Values(w, h)
    End Function

    Public Function GetValueNearestNeighbor(x As Double, y As Double) As Single
        Dim w As Double = Me.Header.GetW(x)
        Dim h As Double = Me.Header.GetH(y)

        Return Me.GetValue(Math.Round(w), Math.Round(h))
    End Function

    Public Function GetValueBilinear(x As Double, y As Double) As Single
        Dim w As Double = Me.Header.GetW(x)
        Dim h As Double = Me.Header.GetH(y)

        Dim w0 As Integer = Math.Floor(w)
        Dim h0 As Integer = Math.Floor(h)

        Dim width As Double = w - w0
        Dim height As Double = h - h0

        Dim value As Single = (1 - width) * (1 - height) * GetValue(w0, h0) +
                              (1 - width) * (height) * GetValue(w0, h0 + 1) +
                              (width) * (1 - height) * GetValue(w0 + 1, h0) +
                              (width) * (height) * GetValue(w0 + 1, h0 + 1)

        Return value
    End Function

    Public Function GetValueBicubic(x As Double, y As Double) As Single
        Dim w As Double = Me.Header.GetW(x)
        Dim h As Double = Me.Header.GetH(y)

        Throw New NotImplementedException()
    End Function

End Class
