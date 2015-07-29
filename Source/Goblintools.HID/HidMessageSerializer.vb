Imports System.IO
Imports System.Runtime.Serialization
Imports System.Reflection
Imports System.Text

Public Class HidMessageSerializer(Of T)

    Public Property Size As Byte = 64
    Public Property Encoding As ASCIIEncoding = ASCIIEncoding.ASCII
    Public Property FeatureTypes As New Dictionary(Of Int16, Type)

    Public Sub New()
        Try
            Dim types = From t As Type In GetType(T).Assembly().GetTypes()
                        Where t.BaseType = GetType(T)
                        Select t

            For Each t As Type In types
                Dim id As Int16 = GetID(t)

                If id > -1 Then
                    Me.FeatureTypes.Add(id, t)
                End If

                'Dim id As HidMessageAttribute = t.GetCustomAttribute(GetType(HidMessageAttribute))

                'Me.FeatureTypes.Add(id.Id, t)
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Function CreateMessage(id As Integer) As T
        Dim messageType = Me.FeatureTypes(id)

        If messageType IsNot Nothing Then
            Return Activator.CreateInstance(messageType)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetID(t As Type) As Int16
        Dim f As FieldInfo = t.GetField("ID")

        If f IsNot Nothing Then
            Return f.GetValue(Nothing)
        Else
            Return -1
        End If
    End Function

    Public Function Serialize(message As T, command As Byte) As Byte()
        Dim t As Type = message.GetType()
        Dim id As Int16 = GetID(t)

        If id > -1 Then
            Dim props = From p As PropertyInfo In t.GetProperties()
                        Let a = p.GetCustomAttribute(GetType(DataMemberAttribute))
                        Where a IsNot Nothing
                        Order By CType(a, DataMemberAttribute).Order
                        Select p

            Dim buffer(Me.Size - 1) As Byte

            Using output As New MemoryStream(buffer)
                Using writer As New BinaryWriter(output, Me.Encoding)
                    writer.Write(CShort(id))
                    writer.Write(CByte(command))
                    writer.Write(CByte(0))

                    For Each p As PropertyInfo In props
                        If p.PropertyType.IsArray Then
                            Dim array As Array = p.GetValue(message)

                            writer.Write(CByte(array.Length))
                            For Each item In array
                                writer.Write(item)
                            Next
                        ElseIf p.PropertyType = GetType(Color) Then
                            Dim value As Color = p.GetValue(message)

                            writer.Write(value.A)
                            writer.Write(value.R)
                            writer.Write(value.G)
                            writer.Write(value.B)
                        Else
                            writer.Write(p.GetValue(message))
                        End If
                    Next
                End Using

                Return output.ToArray()
            End Using
        Else
            Return Nothing
        End If
    End Function

    Public Function Deserialize(data As Byte()) As T
        If data Is Nothing OrElse data.Length < 5 Then
            Return Nothing
        End If

        Dim message As T = Nothing

        Using input As New MemoryStream(data)
            Using reader As New BinaryReader(input, Me.Encoding)
                Dim code As Integer = reader.ReadInt16()
                Dim command As Byte = reader.ReadByte()
                Dim feature As Byte = reader.ReadByte()

                Dim messageType = Me.FeatureTypes(code)

                If messageType IsNot Nothing Then
                    message = Activator.CreateInstance(messageType)

                    Dim props = From p As PropertyInfo In messageType.GetProperties()
                                Let a = p.GetCustomAttribute(GetType(DataMemberAttribute))
                                Where a IsNot Nothing
                                Order By CType(a, DataMemberAttribute).Order
                                Select p

                    For Each p As PropertyInfo In props
                        Select Case p.PropertyType
                            Case GetType(Boolean)
                                p.SetValue(message, reader.ReadBoolean())
                            Case GetType(Byte)
                                p.SetValue(message, reader.ReadByte())
                            Case GetType(Byte())
                                p.SetValue(message, reader.ReadBytes(reader.ReadByte()))
                            Case GetType(Char)
                                p.SetValue(message, reader.ReadChar())
                            Case GetType(Decimal)
                                p.SetValue(message, reader.ReadDecimal())
                            Case GetType(Double)
                                p.SetValue(message, reader.ReadDouble())
                            Case GetType(Int16)
                                p.SetValue(message, reader.ReadInt16())
                            Case GetType(Int32)
                                p.SetValue(message, reader.ReadInt32())
                            Case GetType(Int64)
                                p.SetValue(message, reader.ReadInt64())
                            Case GetType(SByte)
                                p.SetValue(message, reader.ReadSByte())
                            Case GetType(Single)
                                p.SetValue(message, reader.ReadSingle())
                            Case GetType(String)
                                p.SetValue(message, reader.ReadString())
                            Case GetType(UInt16)
                                p.SetValue(message, reader.ReadUInt16())
                            Case GetType(UInt32)
                                p.SetValue(message, reader.ReadUInt32())
                            Case GetType(UInt64)
                                p.SetValue(message, reader.ReadUInt64())
                            Case GetType(Color)
                                p.SetValue(message, Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()))
                            Case Else
                                Throw New NotSupportedException()
                        End Select
                    Next
                End If

            End Using
        End Using

        Return message
    End Function

    Public Sub FlushReader(ByRef input As Stream)
        While input.Position < Size
            input.ReadByte()
        End While

        input.Flush()
    End Sub

    Public Sub FlushWriter(ByRef output As Stream)
        While output.Position < Size
            output.WriteByte(0)
        End While

        output.Flush()
    End Sub

End Class
