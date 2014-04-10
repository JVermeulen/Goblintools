Public Class GrdFile

    Public Const Extension As String = ".GRD"

    Public Property Header As GrdHeader
    Public Property Records As Single()

    Public ReadOnly Property SizeInBytes As Integer
        Get
            Return GrdHeader.SizeInBytes + Records.Length * 4
        End Get
    End Property

End Class
