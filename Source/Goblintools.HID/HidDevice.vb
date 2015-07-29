Imports Microsoft.Win32.SafeHandles

Public Class HidDevice
    Implements IDisposable

    Friend Property ReadStream As FileStream
    Friend Property WriteStream As FileStream

    Public Property Name As String
    Public Property VendorID As Integer
    Public Property ProductID As Integer
    Public Property InterfaceID As Integer
    Public Property AutoConnect As Boolean
    Public Property IsPlugged As Boolean
    Public Property IsConnected As Boolean

    Public Event Connected()
    Public Event Disconnected()
    Public Event DataSend(data As Byte())
    Public Event DataReceived(data As Byte())
    Public Event MessageReceived As EventHandler(Of HidMessage)

    Public Function IsRelevant() As Boolean
        Return (Me.VendorID = My.Settings.HidVendorID And Me.ProductID = My.Settings.HidProductID And Me.InterfaceID = My.Settings.HidInterfaceID)
    End Function

    Friend Sub New(name As String)
        Me.Name = name

        Dim parts() As String = name.Split("#"c)

        For Each part As String In parts
            If part.StartsWith("VID_") Then
                If part.Length > 7 Then
                    Me.VendorID = Convert.ToInt32(part.Substring(4, 4), 16)
                End If

                If part.Length > 16 Then
                    Me.ProductID = Convert.ToInt32(part.Substring(13, 4), 16)
                End If

                If part.Length > 22 Then
                    Me.InterfaceID = Convert.ToInt32(part.Substring(21, 2), 16)
                End If
            End If
        Next
    End Sub

    Public Sub OnPlugged()
        Me.IsPlugged = True

        If Me.AutoConnect Then
            TryConnect()
        End If
    End Sub

    Public Sub OnUnplugged()
        Me.IsPlugged = False

        Disconnect()

        RaiseEvent Disconnected()
    End Sub

    Public Function TryConnect(autoConnect As Boolean) As Boolean
        Me.AutoConnect = autoConnect

        Return TryConnect()
    End Function

    Public Function TryConnect() As Boolean
        If Me.WriteStream Is Nothing Then
            Dim writeHandle As SafeFileHandle = NativeMethods.CreateFile(Name, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.OpenOrCreate, FileAttributes.Normal, IntPtr.Zero)

            If writeHandle.IsInvalid Then
                If Me.WriteStream IsNot Nothing Then
                    Me.WriteStream.Dispose()
                    Me.WriteStream = Nothing
                End If

                Return False
            Else
                Me.WriteStream = New FileStream(writeHandle, FileAccess.Write, 65)
            End If
        End If

        If Me.ReadStream Is Nothing Then
            Dim readHandle As SafeFileHandle = NativeMethods.CreateFile(Name, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.OpenOrCreate, FileAttributes.Normal, IntPtr.Zero)

            If readHandle.IsInvalid Then
                If Me.ReadStream IsNot Nothing Then
                    Me.ReadStream.Dispose()
                    Me.ReadStream = Nothing
                End If

                Return False
            Else
                Me.ReadStream = New FileStream(readHandle, FileAccess.Read, 65)
            End If
        End If

        Me.IsConnected = True

        RaiseEvent Connected()

        ReadAsync()

        Write(New InfoMessage(), HidMessageCommand.Get)

        Return True
    End Function

    Public Sub Disconnect(autoConnect As Boolean)
        Me.AutoConnect = autoConnect

        Disconnect()
    End Sub

    Public Sub Disconnect()
        Me.IsConnected = False

        If Me.ReadStream IsNot Nothing Then
            Me.ReadStream.Dispose()
            Me.ReadStream = Nothing
        End If

        If Me.WriteStream IsNot Nothing Then
            Me.WriteStream.Dispose()
            Me.WriteStream = Nothing
        End If
    End Sub

    Public Async Sub ReadAsync()
        Try
            Dim data(Main.Serializer.Size) As Byte

            Dim numberOfBytes As Integer = Await Me.ReadStream.ReadAsync(data, 0, data.Length)

            Using input As New MemoryStream(data, 1, data.Length - 1)
                Dim message As HidMessage = Main.Serializer.Deserialize(input.ToArray())

                If message IsNot Nothing Then
                    RaiseEvent MessageReceived(Me, message)
                End If
            End Using

            ReadAsync()
        Catch ex As IOException
            If Me.IsConnected Then
                Disconnect()
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub Write(message As HidMessage, command As HidMessageCommand)
        Write(Main.Serializer.Serialize(message, command))
    End Sub

    Protected Overridable Sub Write(data As Byte())
        Try
            If Me.IsConnected Then
                Me.WriteStream.WriteByte(0)
                Me.WriteStream.Write(data, 0, data.Length)

                Me.WriteStream.Flush()

                RaiseEvent DataSend(data)
            End If
        Catch ex As System.IO.FileNotFoundException
            If Me.IsConnected Then
                Disconnect()
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

#Region " IDisposable "

    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.Disconnect()
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
