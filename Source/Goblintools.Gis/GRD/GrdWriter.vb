Imports System.IO
Imports System.Text.ASCIIEncoding

Namespace GRD

    ''' <summary>
    ''' A GRD file writer.
    ''' </summary>
    Public Class GrdWriter

        ''' <summary>
        ''' The base binary writer.
        ''' </summary>
        Public Property Writer As BinaryWriter

        ''' <summary>
        ''' The constructor of this class.
        ''' </summary>
        ''' <param name="fileName">The filename of the file to read from.</param>
        Public Sub New(fileName As String)
            Me.New(New FileStream(fileName, FileMode.Create, FileAccess.Write))
        End Sub

        ''' <summary>
        ''' The constructor of this class.
        ''' </summary>
        ''' <param name="stream">The stream to read from.</param>
        Public Sub New(stream As Stream)
            Me.Writer = New BinaryWriter(stream)
        End Sub

        ''' <summary>
        ''' Writes the given GRD file to the given stream.
        ''' </summary>
        ''' <param name="file">The file to write.</param>
        Public Sub WriteFile(file As GrdFile)
            WriteHeader(file.Header)
            WriteValues(file.Values)
        End Sub

        ''' <summary>
        ''' Writes the given header to the given stream.
        ''' </summary>
        ''' <param name="header">The header to write.</param>
        Public Sub WriteHeader(header As GrdHeader)
            Me.Writer.Write(ASCII.GetBytes(GrdHeader.Code))

            Me.Writer.Write(CType(header.ColumnCount, Int16))
            Me.Writer.Write(CType(header.RowCount, Int16))
            Me.Writer.Write(header.MinX)
            Me.Writer.Write(header.MaxX)
            Me.Writer.Write(header.MinY)
            Me.Writer.Write(header.MaxY)
            Me.Writer.Write(header.MinValue)
            Me.Writer.Write(header.MaxValue)
        End Sub

        ''' <summary>
        ''' Writes the given values to the given stream.
        ''' </summary>
        ''' <param name="values">The values to write.</param>
        Public Sub WriteValues(values As Single(,))
            For column As Integer = 0 To values.GetLength(0) - 1
                For row As Integer = 0 To values.GetLength(1) - 1
                    Me.Writer.Write(values(column, row))
                Next
            Next
        End Sub

    End Class

End Namespace
