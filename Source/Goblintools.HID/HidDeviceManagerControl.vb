Imports System.ComponentModel

Public Class HidDeviceManagerControl

    Private Sub HidDeviceManagerControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler Main.GobletManager.GobletAdded, AddressOf GobletManager_GobletAdded
        AddHandler Main.GobletManager.GobletUpdated, AddressOf GobletManager_GobletUpdated
        AddHandler Main.GobletManager.GobletRemoved, AddressOf GobletManager_GobletRemoved
    End Sub

    Private Sub Reload()
        Me.lvwDevices.Items.Clear()
        Me.lvwDevices.Groups.Clear()

        For Each goblet As Goblet In Main.GobletManager.Goblets.Values
            If goblet.Features.Count > 0 Then
                Dim group As New ListViewGroup(goblet.Key, goblet.Name)
                Me.lvwDevices.Groups.Add(group)

                For Each key As HidMessageCode In goblet.Features.Keys
                    Dim text As String = key.ToString()
                    Dim value As String = If(goblet.Features(key) IsNot Nothing, goblet.Features(key).ToString(), String.Empty)

                    Dim item As New ListViewItem(text, group)
                    item.SubItems.Add(value)

                    Me.lvwDevices.Items.Add(item)
                Next
            End If
        Next
    End Sub

    Private Sub GobletManager_GobletAdded(sender As Object, e As String)
        If Main.GobletManager.Goblets.ContainsKey(e) Then
            Reload()
        End If
    End Sub

    Private Sub GobletManager_GobletUpdated(sender As Object, e As String)
        If Main.GobletManager.Goblets.ContainsKey(e) Then
            Reload()
            'Dim group As ListViewGroup = GetOrAddGroup(e)

            'group.Header = Main.GobletManager.Goblets(e).Name
        End If
    End Sub

    Private Sub GobletManager_GobletRemoved(sender As Object, e As String)
        If Main.GobletManager.Goblets.ContainsKey(e) Then
            Reload()
            'Dim group As ListViewGroup = GetOrAddGroup(e)

            'Me.lvwDevices.Groups.Remove(group)
        End If
    End Sub

    Private Function GetOrAddGroup(key As String) As ListViewGroup
        Dim group = (From g In Me.lvwDevices.Groups
                     Where g.Name = key
                     Select g).FirstOrDefault

        If group Is Nothing Then
            group = New ListViewGroup(key)

            Me.lvwDevices.Groups.Add(group)
        End If

        Return group
    End Function

    Private Function GetOrAddItem(group As ListViewGroup, key As String) As ListViewItem
        Dim item = (From i As ListViewItem In Me.lvwDevices.Items
                    Where i.Group Is group AndAlso i.Text = key
                    Select i).FirstOrDefault()

        If item Is Nothing Then
            item = New ListViewItem(key)

            Me.lvwDevices.Items.Add(item)
        End If

        Return item
    End Function

End Class
