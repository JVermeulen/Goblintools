Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Main.DeviceManager.ReadDevices()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim r As New Random()
        Dim buffer(2) As Byte
        r.NextBytes(buffer)

        Main.DeviceManager.Devices("\\?\HID#VID_16C0&PID_0486&MI_00#9&2174D85D&0&0000#{4D1E55B2-F16F-11CF-88CB-001111000030}").Write(New LedMessage(buffer(0), buffer(1), buffer(2)), HidMessageCommand.Set)
        Main.DeviceManager.Devices("\\?\HID#VID_16C0&PID_0486&MI_00#9&211BEAA6&0&0000#{4D1E55B2-F16F-11CF-88CB-001111000030}").Write(New LedMessage(buffer(0), buffer(1), buffer(2)), HidMessageCommand.Set)

    End Sub

End Class
