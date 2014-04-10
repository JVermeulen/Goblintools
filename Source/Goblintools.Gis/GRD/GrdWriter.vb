Imports System.IO
Imports System.Text.ASCIIEncoding

Public Class GrdWriter

    Public Property Writer As BinaryWriter

    Public Sub New(fileName As String)
        Me.New(New FileStream(fileName, FileMode.Create, FileAccess.Write))
    End Sub

    Public Sub New(stream As Stream)
        Me.Writer = New BinaryWriter(stream)
    End Sub

    Public Sub WriteFile(file As GrdFile)
        WriteHeader(file.Header)
        WriteRecords(file.Records)
    End Sub

    Public Sub WriteHeader(header As GrdHeader)
        Me.Writer.Write(ASCII.GetBytes(GrdHeader.Code))

        Me.Writer.Write(CType(header.SizeX, Int16))
        Me.Writer.Write(CType(header.SizeY, Int16))
        Me.Writer.Write(header.MinX)
        Me.Writer.Write(header.MaxX)
        Me.Writer.Write(header.MinY)
        Me.Writer.Write(header.MaxY)
        Me.Writer.Write(header.MinValue)
        Me.Writer.Write(header.MaxValue)
    End Sub

    Public Sub WriteRecords(records As Single())
        For Each record As Single In records
            Me.Writer.Write(record)
        Next
    End Sub

End Class
