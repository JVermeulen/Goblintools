Public Structure Link

    Public Property FromNode As UInt32
    Public Property ToNode As UInt32
    Public Property Cost As UInt32

    Public Sub New(fromNode As UInt32, toNode As UInt32, cost As UInt32)
        Me.FromNode = fromNode
        Me.ToNode = toNode
        Me.Cost = cost
    End Sub

End Structure
