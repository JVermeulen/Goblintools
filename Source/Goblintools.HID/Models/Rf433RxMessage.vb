Imports System.Runtime.Serialization

<DataContract()>
Public Class Rf433RxMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.RF433RX

    Public Sub New()
        MyBase.New(ID)
    End Sub

End Class
