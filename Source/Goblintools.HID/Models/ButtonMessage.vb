Imports System.Runtime.Serialization

<DataContract()>
Public Class ButtonMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.Button

    <DataMember(order:=0)>
    Public Property Pressed As Boolean

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(pressed As Boolean)
        MyBase.New(ID)

        Me.Pressed = pressed
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Pressed.ToString()
    End Function

End Class
