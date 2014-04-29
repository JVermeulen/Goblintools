Imports System.IO
Imports System.Text.ASCIIEncoding

''' <summary>
''' A GRD file reader.
''' </summary>
Public Class GrdReader
    Implements IDisposable

    ''' <summary>
    ''' The base binary reader.
    ''' </summary>
    Protected Property Reader As BinaryReader

    ''' <summary>
    ''' The header containing meta information.
    ''' </summary>
    Protected Property Header As GrdHeader

    ''' <summary>
    ''' The constructor of this class.
    ''' </summary>
    ''' <param name="fileName">The filename of the file to read from.</param>
    Public Sub New(fileName As String)
        Me.New(New FileStream(fileName, FileMode.Open, FileAccess.Read))
    End Sub

    ''' <summary>
    ''' The constructor of this class.
    ''' </summary>
    ''' <param name="stream">The stream to read from.</param>
    Public Sub New(stream As Stream)
        Me.Reader = New BinaryReader(stream)
    End Sub

    ''' <summary>
    ''' Reads the given stream as GRD file.
    ''' </summary>
    Public Function ReadFile() As GrdFile
        Dim file As New GrdFile()

        file.Header = ReadHeader()
        file.Values = ReadValues()

        Return file
    End Function

    ''' <summary>
    ''' Read the header of the given stream.
    ''' </summary>
    Public Function ReadHeader() As GrdHeader
        If Me.Header Is Nothing Then
            Me.Header = New GrdHeader()

            Me.Reader.BaseStream.Position = 0

            'Validate length of stream
            If Me.Reader.BaseStream.Length < GrdHeader.SizeInBytes Then
                Throw New ArgumentException("Unable to read header. Stream is too short.")
            End If

            'Validate first 4 bytes code
            If ASCII.GetString(Reader.ReadBytes(4)) <> GrdHeader.Code Then
                Throw New ArgumentException("Unable to reader header. Stream is not a valid GRD file.")
            End If

            Header.ColumnCount = Reader.ReadInt16()
            Header.RowCount = Reader.ReadInt16()
            Header.MinX = Reader.ReadDouble()
            Header.MaxX = Reader.ReadDouble()
            Header.MinY = Reader.ReadDouble()
            Header.MaxY = Reader.ReadDouble()
            Header.MinValue = Reader.ReadDouble()
            Header.MaxValue = Reader.ReadDouble()
        End If

        Return Me.Header
    End Function

    ''' <summary>
    ''' Reads all values from the given stream.
    ''' </summary>
    Public Function ReadValues() As Single(,)
        If Me.Header Is Nothing Then
            Me.Header = ReadHeader()
        End If

        Me.Reader.BaseStream.Position = GrdHeader.SizeInBytes

        Dim values(Me.Header.ColumnCount - 1, Me.Header.RowCount - 1) As Single

        For column As Integer = 0 To Me.Header.ColumnCount - 1
            For row As Integer = 0 To Me.Header.RowCount - 1
                values(column, row) = ReadValue(column, row)
            Next
        Next

        Return values
    End Function

    ''' <summary>
    ''' Reads a single value from the given stream.
    ''' </summary>
    ''' <param name="column">The column index.</param>
    ''' <param name="row">The row index.</param>
    Public Function ReadValue(column As Integer, row As Integer) As Single
        Dim position As Integer = GrdHeader.SizeInBytes + Me.Header.GetIndex(column, row) * 4

        'Validates the length of the given stream.
        If (position + 4 > Me.Reader.BaseStream.Length) Then
            Throw New ArgumentException("Unable to read value. Stream is too short.")
        End If

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
