Imports System.IO
Imports System.Text.ASCIIEncoding

Public Class GrdReader
    Implements IDisposable

    Protected Property Reader As BinaryReader
    Protected Property Header As GrdHeader

    Public Sub New(fileName As String)
        Me.New(New FileStream(fileName, FileMode.Open, FileAccess.Read))
    End Sub

    Public Sub New(stream As Stream)
        Me.Reader = New BinaryReader(stream)
    End Sub

    Public Function ReadFile() As GrdFile
        Dim file As New GrdFile()

        file.Header = ReadHeader()
        file.Values = ReadValues()

        Return file
    End Function

    Public Function ReadHeader() As GrdHeader
        If Me.Header Is Nothing Then
            Me.Header = New GrdHeader()

            Me.Reader.BaseStream.Position = 0

            If Me.Reader.BaseStream.Length < GrdHeader.SizeInBytes Then
                Throw New ArgumentException("Unable to read header. Stream is too short.")
            End If

            Dim code As String = ASCII.GetString(Reader.ReadBytes(4))

            If code <> GrdHeader.Code Then
                Throw New ArgumentException("Unable to reader header. Stream is not a valid GRD file.")
            End If

            Header.Width = Reader.ReadInt16()
            Header.Height = Reader.ReadInt16()
            Header.MinX = Reader.ReadDouble()
            Header.MaxX = Reader.ReadDouble()
            Header.MinY = Reader.ReadDouble()
            Header.MaxY = Reader.ReadDouble()
            Header.MinValue = Reader.ReadDouble()
            Header.MaxValue = Reader.ReadDouble()
        End If

        Return Me.Header
    End Function

    Public Function ReadValues() As Single(,)
        If Me.Header Is Nothing Then
            Me.Header = ReadHeader()
        End If

        Me.Reader.BaseStream.Position = GrdHeader.SizeInBytes

        Dim values(Me.Header.Width - 1, Me.Header.Height - 1) As Single

        For w As Integer = 0 To Me.Header.Width - 1
            For h As Integer = 0 To Me.Header.Height - 1
                values(w, h) = ReadValue(w, h)
            Next
        Next

        Return values
    End Function

    Public Function ReadValue(w As Integer, h As Integer) As Single
        Dim position As Integer = GrdHeader.SizeInBytes + Me.Header.GetIndex(w, h) * 4
        
        If (position + 4 > Me.Reader.BaseStream.Length) Then Throw New ArgumentException("Unable to read value. Stream is too short.")

        Me.Reader.BaseStream.Position = position

        Return Reader.ReadSingle()
    End Function

#Region " IDisposable "

    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If Me.Reader IsNot Nothing Then Me.Reader.Dispose()
            End If
        End If

        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)

        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
