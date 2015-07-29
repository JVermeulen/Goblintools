Public Enum HidDeviceManagementEvent
    QueryChangeConfig = &H17
    ConfigChanged = &H18
    ConfigChangeCanceled = &H19
    DeviceArrival = &H8000
    DeviceQueryRemove = &H8001
    DeviceQueryRemoveFailed = &H8002
    DeviceRemovePending = &H8003
    DeviceRemoveComplete = &H8004
    DeviceTypeSpecific = &H8005
    CustomEvent = &H8006
    DevNodesChanged = &H8007
    UserDefined = &HFFFF
End Enum
