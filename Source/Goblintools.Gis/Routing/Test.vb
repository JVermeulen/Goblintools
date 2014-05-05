Public Class Test

    Public Shared Node(7) As Node
    Public Shared Link(11) As Link

    Public Sub New()
        Node(0) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(1) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(2) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(3) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(4) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(5) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(6) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)
        Node(7) = New Node(UInt32.MaxValue, False, UInt32.MaxValue)

        Link(0) = New Link(0, 1, 16000)
        Link(1) = New Link(0, 2, 9000)
        Link(2) = New Link(0, 3, 35000)
        Link(3) = New Link(1, 4, 25000)
        Link(4) = New Link(1, 3, 12000)
        Link(5) = New Link(2, 3, 15000)
        Link(6) = New Link(3, 4, 14000)
        Link(7) = New Link(4, 6, 8000)
        Link(8) = New Link(3, 6, 19000)
        Link(9) = New Link(3, 5, 17000)
        Link(10) = New Link(5, 6, 14000)
        Link(11) = New Link(2, 5, 22000)
    End Sub

    Public Sub Go()
        Dim startNode As UInt32 = 0
        Dim endNode As UInt32 = 6
        Dim currentNode As UInt32
        Dim targetCost As UInt32

        Node(startNode).Cost = 0
        Node(startNode).ViaNode = 0

        While TryGetNextNode(currentNode)
            Node(currentNode).Calculated = True

            For i As Integer = 0 To Link.Count - 1
                Dim calcNode As UInt32 = currentNode

                If currentNode = Link(i).FromNode Then
                    calcNode = Link(i).ToNode
                ElseIf currentNode = Link(i).ToNode Then
                    calcNode = Link(i).FromNode
                End If

                If calcNode <> currentNode And Not Node(calcNode).Calculated Then
                    targetCost = Node(currentNode).Cost + Link(i).Cost

                    If targetCost < Node(calcNode).Cost Then
                        Node(calcNode).Cost = targetCost
                        Node(calcNode).ViaNode = currentNode
                    End If
                End If
            Next
        End While
    End Sub

    Public Function TryGetNextNode(ByRef nextNode As UInt32) As Boolean
        Dim lowestValue As UInt32 = UInt32.MaxValue

        For i As UInt32 = 0 To Node.Count - 1
            If Not Node(i).Calculated And Node(i).Cost < lowestValue Then
                nextNode = i

                lowestValue = Node(i).Cost
            End If
        Next

        Return lowestValue < UInt32.MaxValue
    End Function

End Class
