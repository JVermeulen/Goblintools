Public Class GobletManager

    Public Property Goblets As New Dictionary(Of String, Goblet)

    Public Event GobletAdded As EventHandler(Of String)
    Public Event GobletUpdated As EventHandler(Of String)
    Public Event GobletRemoved As EventHandler(Of String)

    Public Sub New()
        AddHandler Main.DeviceManager.HidPlugged, AddressOf HidDeviceManager_HidPlugged
        AddHandler Main.DeviceManager.HidUnplugged, AddressOf HidDeviceManager_HidUnplugged
    End Sub

    Private Sub HidDeviceManager_HidPlugged(sender As Object, e As HidDevice)
        If e.IsRelevant Then
            If Not Me.Goblets.ContainsKey(e.Name) Then
                Me.Goblets.Add(e.Name, New Goblet() With {.Key = e.Name})
            End If

            AddHandler e.MessageReceived, AddressOf HidDevice_MessageReceived

            RaiseEvent GobletAdded(Me, e.Name)

            e.TryConnect(True)
        End If
    End Sub

    Private Sub HidDeviceManager_HidUnplugged(sender As Object, e As HidDevice)
        If e.IsRelevant Then
            If Me.Goblets.ContainsKey(e.Name) Then
                Me.Goblets.Remove(e.Name)
            End If

            RemoveHandler e.MessageReceived, AddressOf HidDevice_MessageReceived

            RaiseEvent GobletRemoved(Me, e.Name)
        End If
    End Sub

    Private Sub HidDevice_MessageReceived(sender As Object, e As HidMessage)
        Dim device As HidDevice = CType(sender, HidDevice)

        If device.IsRelevant Then
            If Me.Goblets.ContainsKey(device.Name) Then
                Dim goblet As Goblet = Me.Goblets(device.Name)

                Select Case e.ID
                    Case HidMessageCode.Info
                        Dim message As InfoMessage = CType(e, InfoMessage)

                        goblet.Name = message.Name

                        For Each feature As HidMessageCode In message.Features
                            If feature > HidMessageCode.Info Then
                                goblet.Features.Add(feature, Nothing)

                                device.Write(Main.Serializer.CreateMessage(feature), HidMessageCommand.Get)
                            End If
                        Next

                        RaiseEvent GobletUpdated(Me, device.Name)
                    Case Else
                        If goblet.Features.ContainsKey(e.ID) Then
                            goblet.Features(e.ID) = e

                            RaiseEvent GobletUpdated(Me, device.Name)
                        End If
                End Select
            End If
        End If
    End Sub

End Class
