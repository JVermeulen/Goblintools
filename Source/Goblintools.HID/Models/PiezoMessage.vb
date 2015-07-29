Imports System.Runtime.Serialization

<DataContract()>
Public Class PiezoMessage
    Inherits HidMessage

    Public Shadows Const ID As HidMessageCode = HidMessageCode.Piezo

    Public Sub New()
        MyBase.New(ID)
    End Sub

End Class
