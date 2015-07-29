Imports System.Runtime.Serialization

<DataContract()>
Public Class InfoMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.Info

    <DataMember(order:=0)>
    Public Property Name As String = String.Empty

    <DataMember(order:=1)>
    Public Property Features As Byte() = {}

    Public Sub New()
        MyBase.New(ID)
    End Sub

End Class
