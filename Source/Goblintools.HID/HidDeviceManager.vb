Imports System.Windows.Forms
Imports System.Runtime.InteropServices

<System.ComponentModel.ToolboxItem(False)>
Public Class HidDeviceManager
    Inherits Control

    Public Property Devices As New Dictionary(Of String, HidDevice)

    Public HidHandle As IntPtr
    Public HidGuid As Guid

    Public Event HidPlugged As EventHandler(Of HidDevice)
    Public Event HidUnplugged As EventHandler(Of HidDevice)

    Public Sub New()
        '
    End Sub

    Public Function GetOrAddDevice(name As String) As HidDevice
        If Not Me.Devices.ContainsKey(name) Then
            Me.Devices.Add(name, New HidDevice(name))
        End If

        Return Me.Devices(name)
    End Function

    Public Sub ReadDevices()
        Dim filter As New NativeMethods.DEV_BROADCAST_DEVICEINTERFACE()
        Dim buffer As IntPtr = Marshal.AllocHGlobal(filter.Size)
        Marshal.StructureToPtr(filter, buffer, True)
        Dim notifierHandle As IntPtr = NativeMethods.RegisterDeviceNotification(Me.Handle, buffer, 0)
        filter = Marshal.PtrToStructure(buffer, filter.GetType())
        Marshal.FreeHGlobal(buffer)

        NativeMethods.GetHidGuid(Me.HidGuid)
        Me.HidHandle = NativeMethods.SetupDiGetClassDevs(Me.HidGuid, Nothing, Nothing, NativeMethods.ClassDevsFlags.DIGCF_DEVICEINTERFACE Or NativeMethods.ClassDevsFlags.DIGCF_PRESENT)

        While True
            Dim data As New NativeMethods.PSP_DEVICE_INTERFACE_DATA()
            data.SetSize()

            If NativeMethods.SetupDiEnumDeviceInterfaces(Me.HidHandle, IntPtr.Zero, Me.HidGuid, Me.Devices.Count, data) Then
                Dim detailData As New NativeMethods.PSP_DEVICE_INTERFACE_DETAIL_DATA()
                detailData.SetSize()

                Dim size As Integer = 0
                NativeMethods.SetupDiGetDeviceInterfaceDetail(Me.HidHandle, data, IntPtr.Zero, 0, size, IntPtr.Zero)
                NativeMethods.SetupDiGetDeviceInterfaceDetail(Me.HidHandle, data, detailData, size, size, IntPtr.Zero)

                Dim name As String = detailData.DeviceName.Trim().ToUpper()

                Dim device As HidDevice = GetOrAddDevice(name)

                device.OnPlugged()

                RaiseEvent HidPlugged(Me, device)
            Else
                Return
            End If
        End While
    End Sub

    Protected Overrides Sub WndProc(ByRef msg As Message)
        MyBase.WndProc(msg)

        If msg.Msg = 537 Then
            Dim dbi As NativeMethods.DEV_BROADCAST_DEVICEINTERFACE_1 = msg.GetLParam(GetType(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE_1))

            Dim name As New String(dbi.Name)
            name = name.Substring(0, name.IndexOf(Chr(0))).ToUpper()

            If msg.WParam = HidDeviceManagementEvent.DeviceArrival Then
                HidPluggedHandler(name)
            ElseIf msg.WParam = HidDeviceManagementEvent.DeviceRemoveComplete Then
                HidUnpluggedHandler(name)
            End If
        End If
    End Sub

    Protected Sub HidPluggedHandler(name As String)
        Dim device As HidDevice = GetOrAddDevice(name)
        device.OnPlugged()

        RaiseEvent HidPlugged(Me, device)
    End Sub

    Protected Sub HidUnpluggedHandler(name As String)
        Dim device As HidDevice = GetOrAddDevice(name)
        device.OnUnplugged()

        RaiseEvent HidUnplugged(Me, device)
    End Sub

End Class
