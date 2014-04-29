Namespace CLR

    ''' <summary>
    ''' The header of a MapInfo CLR file containing a palette of colors.
    ''' </summary>
    Public Class ClrHeader

        ''' <summary>
        ''' A code identifying the CLR file. In bytes this is {205, 202, 0, 1}.
        ''' </summary>
        Public Const Code As Int32 = 16829133

        ''' <summary>
        ''' The size of this object in bytes.
        ''' </summary>
        Public Const SizeInBytes As Integer = 4

        ''' <summary>
        ''' The number of columns.
        ''' </summary>
        Public Property ColumnCount As Int32 = 16

        ''' <summary>
        ''' The number of rows.
        ''' </summary>
        Public Property RowCount As Int32 = 16

        ''' <summary>
        ''' The index of the given column and row.
        ''' </summary>
        ''' <param name="column">The column of the grid.</param>
        ''' <param name="row">The row of the grid.</param>
        Public Function GetIndex(column As Integer, row As Integer) As Integer
            Return (column * Me.ColumnCount + row)
        End Function

    End Class

End Namespace
