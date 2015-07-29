Imports System.Runtime.Serialization

<DataContract()>
Public Class Rf433TxMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.RF433TX

    Public Sub New()
        MyBase.New(ID)
    End Sub

End Class
