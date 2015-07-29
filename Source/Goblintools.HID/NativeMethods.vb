Imports System.Security.Permissions
Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

Public Class NativeMethods

    Public Shared Function IsX64() As Boolean
        Return Marshal.SizeOf(GetType(IntPtr)) = 8
    End Function

#Region " uer32.dll "

    Friend Const DBT_DEVTYP_DEVICEINTERFACE As Integer = &H5
    Friend Const HID_GUID As String = "4d1e55b2-f16f-11cf-88cb-001111000030"

    <DllImport("User32.dll", SetLastError:=True)>
    Public Shared Function RegisterDeviceNotification(recipient As IntPtr, notificationFilter As IntPtr, flags As Integer) As IntPtr
    End Function

    <DllImport("User32.dll", SetLastError:=True)>
    Public Shared Function UnregisterDeviceNotification(recipient As IntPtr) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Class DEV_BROADCAST_DEVICEINTERFACE
        Public Size As Integer
        Public DeviceType As Integer
        Public Reserved As Integer
        Public ClassGuid As Guid
        Public Name As Char

        Public Sub New()
            Me.Size = Marshal.SizeOf(Me)
            Me.DeviceType = DBT_DEVTYP_DEVICEINTERFACE
            Me.ClassGuid = New Guid(HID_GUID)
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Class DEV_BROADCAST_DEVICEINTERFACE_1
        Public Size As Integer
        Public DeviceType As Integer
        Public Reserved As Integer
        Public ClassGuid As Guid
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=255)>
        Public Name As Char()

        Public Sub New()
            Me.Size = Marshal.SizeOf(Me)
            Me.DeviceType = DBT_DEVTYP_DEVICEINTERFACE
            Me.ClassGuid = New Guid(HID_GUID)
        End Sub

    End Class

#End Region

#Region " kernel32.dll "

    <DllImport("kernel32.dll")> _
    Public Shared Function CreateFile(lpFileName As String, dwDesiredAccess As FileAccess, dwShareMode As FileShare, lpSecurityAttributes As IntPtr, dwCreationDisposition As FileMode, dwFlagsAndAttributes As FileAttributes, hTemplateFile As IntPtr) As SafeFileHandle
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False, SetLastError:=True)>
    <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)>
    Friend Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function GetLastError() As Integer
    End Function

#End Region

#Region " setupapi.dll "

    <Flags>
    Public Enum ClassDevsFlags
        DIGCF_DEFAULT = &H1
        DIGCF_PRESENT = &H2
        DIGCF_ALLCLASSES = &H4
        DIGCF_PROFILE = &H8
        DIGCF_DEVICEINTERFACE = &H10
    End Enum

    <Flags>
    Public Enum SP_DEVICE_INTERFACE_DATA_FLAGS
        SPINT_ACTIVE = &H1
        SPINT_DEFAULT = &H2
        SPINT_REMOVED = &H4
    End Enum

    <DllImport("setupapi.dll", SetLastError:=True)>
    Public Shared Function SetupDiCreateDeviceInfoList(ByRef classGuid As Guid, hwndParent As IntPtr) As IntPtr
    End Function

    <DllImport("setupapi.dll", SetLastError:=True)>
    Public Shared Function SetupDiDestroyDeviceInfoList(deviceInfoSet As IntPtr) As Boolean
    End Function

    <DllImport("setupapi.dll", SetLastError:=True)>
    Public Shared Function SetupDiEnumDeviceInterfaces(deviceInfoSet As IntPtr, deviceInfoData As IntPtr, ByRef interfaceClassGuid As Guid, memberIndex As Integer, ByRef deviceInterfaceData As PSP_DEVICE_INTERFACE_DATA) As Boolean
    End Function

    <DllImport("setupapi.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function SetupDiGetClassDevs(ByRef ClassGuid As Guid, Enumerator As String, hwndParent As IntPtr, Flags As ClassDevsFlags) As IntPtr
    End Function

    <DllImport("setupapi.dll", SetLastError:=True)>
    Public Shared Function SetupDiGetDeviceInterfaceDetail(ByVal DeviceInfoSet As IntPtr,
                                                           ByRef DeviceInterfaceData As PSP_DEVICE_INTERFACE_DATA,
                                                           ByVal DeviceInterfaceDetailData As IntPtr,
                                                           ByVal DeviceInterfaceDetailDataSize As Integer,
                                                           ByRef RequiredSize As Integer,
                                                           ByVal DeviceInfoData As IntPtr) As Boolean
    End Function

    <DllImport("setupapi.dll", SetLastError:=True)>
    Public Shared Function SetupDiGetDeviceInterfaceDetail(ByVal DeviceInfoSet As IntPtr,
                                                           ByRef DeviceInterfaceData As PSP_DEVICE_INTERFACE_DATA,
                                                           ByRef DeviceInterfaceDetailData As PSP_DEVICE_INTERFACE_DETAIL_DATA,
                                                           ByVal DeviceInterfaceDetailDataSize As Integer,
                                                           ByRef RequiredSize As Integer,
                                                           ByVal DeviceInfoData As IntPtr) As Boolean
    End Function

    ' ''' <summary>
    ' ''' Represents a wrapper class for a DeviceInfoSet handle.
    ' ''' </summary>
    '<StructLayout(LayoutKind.Sequential)>
    'Public Class HDEVINFO
    '    Inherits SafeHandleMinusOneIsInvalid

    '    Protected Sub New()
    '        MyBase.New(True)
    '    End Sub

    '    Protected Overrides Function ReleaseHandle() As Boolean
    '        Return SetupDiDestroyDeviceInfoList(Me.handle)
    '    End Function

    'End Class

    Public Structure PSP_DEVICE_INTERFACE_DATA
        Public cbSize As Integer
        Public InterfaceClassGuid As Guid
        Public Flags As SP_DEVICE_INTERFACE_DATA_FLAGS
        Public Reserved As Integer

        Public Sub SetSize()
            Me.cbSize = Marshal.SizeOf(Me)
        End Sub

    End Structure

    Public Structure PSP_DEVICE_INTERFACE_DETAIL_DATA
        Public cbSize As Integer

        <MarshalAs(UnmanagedType.ByValTStr, sizeconst:=256)>
        Public DeviceName As String

        Public Sub SetSize()
            Me.cbSize = If(IsX64, 8, 5)
        End Sub

    End Structure

#End Region

#Region " hid.dll "

    ''' <summary>
    ''' The HidD_GetAttributes routine returns the attributes of a specified top-level collection.
    ''' </summary>
    ''' <param name="hidDeviceObject">Specifies an open handle to a top-level collection.</param>
    ''' <param name="outAttributes">Pointer to a caller-allocated HIDD_ATTRIBUTES structure that returns the attributes of the collection specified by HidDeviceObject.</param>
    ''' <returns>HidD_GetAttributes returns TRUE if succeeds; otherwise, it returns FALSE.</returns>
    <DllImport("hid.dll", EntryPoint:="HidD_GetAttributes")>
    Public Shared Function GetHidAttributes(hidDeviceObject As IntPtr, ByRef outAttributes As HidAttributes) As Boolean
    End Function

    ''' <summary>
    ''' The HidD_GetHidGuid routine returns the device interfaceGUID for HIDClass devices.
    ''' </summary>
    ''' <param name="hidGuid">Pointer to a caller-allocated GUID buffer that the routine uses to return the device interface GUID for HIDClass devices.</param>
    <DllImport("hid.dll", EntryPoint:="HidD_GetHidGuid")>
    Public Shared Sub GetHidGuid(ByRef hidGuid As Guid)
    End Sub

    ''' <summary>
    ''' The HIDD_ATTRIBUTES structure contains vendor information about a HIDClass device.
    ''' </summary>
    ''' <remarks>http://msdn.microsoft.com/en-us/library/windows/hardware/ff538865(v=vs.85).aspx</remarks>
    Public Structure HidAttributes

        ''' <summary>
        ''' Specifies the size, in bytes, of a HIDD_ATTRIBUTES structure.
        ''' </summary>
        Public Size As UInteger

        ''' <summary>
        ''' Specifies a HID device's vendor ID.
        ''' </summary>
        Public VendorID As UShort

        ''' <summary>
        ''' Specifies a HID device's product ID.
        ''' </summary>
        Public ProductID As UShort

        ''' <summary>
        ''' Specifies the manufacturer's revision number for a HIDClass device.
        ''' </summary>
        Public VersionNumber As UShort

    End Structure

#End Region

End Class
