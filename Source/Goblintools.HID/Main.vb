Module Main

    Public WithEvents DeviceManager As New HidDeviceManager()
    Public WithEvents GobletManager As New GobletManager()
    Public Property Serializer As New HidMessageSerializer(Of HidMessage)

End Module
