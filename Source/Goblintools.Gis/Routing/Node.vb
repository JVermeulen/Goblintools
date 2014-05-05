Public Structure Node

    Public Property Cost As UInt32
    Public Property Calculated As Boolean
    Public Property ViaNode As UInt32

    Public Sub New(cost As UInt32, calculated As Boolean, viaNode As UInt32)
        Me.Cost = cost
        Me.Calculated = calculated
        Me.ViaNode = viaNode
    End Sub

End Structure
