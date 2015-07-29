Imports System.Runtime.Serialization

''' <summary>
''' http://www.adafruit.com/products/159
''' </summary>
<DataContract()>
Public Class LedMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.Led

    <DataMember(order:=0)>
    Public Property Alpha As Byte

    <DataMember(order:=1)>
    Public Property Red As Byte

    <DataMember(order:=2)>
    Public Property Green As Byte

    <DataMember(order:=3)>
    Public Property Blue As Byte

    Public Sub New()
        Me.New(255, 0, 0, 0)
    End Sub

    Public Sub New(color As Color)
        Me.New(color.A, color.R, color.G, color.B)
    End Sub

    Public Sub New(red As Byte, green As Byte, blue As Byte)
        Me.New(255, red, green, blue)
    End Sub

    Public Sub New(alpha As Byte, red As Byte, green As Byte, blue As Byte)
        MyBase.New(ID)

        Me.Alpha = alpha
        Me.Red = red
        Me.Green = green
        Me.Blue = blue
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("#{0}{1}{2}{3}", Me.Alpha.ToString("X2"), Me.Red.ToString("X2"), Me.Green.ToString("X2"), Me.Blue.ToString("X2"))
    End Function

End Class
