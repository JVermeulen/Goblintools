Public Class HidMessageAttribute
    Inherits Attribute

    Public Property Id As Integer

    Public Sub New(id As Integer)
        Me.Id = id
    End Sub

End Class
