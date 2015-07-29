<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HidDeviceManagerControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lvwDevices = New System.Windows.Forms.ListView()
        Me.NameHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ValueHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'lvwDevices
        '
        Me.lvwDevices.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.NameHeader, Me.ValueHeader})
        Me.lvwDevices.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwDevices.FullRowSelect = True
        Me.lvwDevices.Location = New System.Drawing.Point(0, 0)
        Me.lvwDevices.Name = "lvwDevices"
        Me.lvwDevices.Size = New System.Drawing.Size(300, 300)
        Me.lvwDevices.TabIndex = 0
        Me.lvwDevices.UseCompatibleStateImageBehavior = False
        Me.lvwDevices.View = System.Windows.Forms.View.Details
        '
        'NameHeader
        '
        Me.NameHeader.Text = "Name"
        Me.NameHeader.Width = 120
        '
        'ValueHeader
        '
        Me.ValueHeader.Text = "Value"
        Me.ValueHeader.Width = 120
        '
        'HidDeviceManagerControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lvwDevices)
        Me.Name = "HidDeviceManagerControl"
        Me.Size = New System.Drawing.Size(300, 300)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lvwDevices As System.Windows.Forms.ListView
    Friend WithEvents NameHeader As System.Windows.Forms.ColumnHeader
    Friend WithEvents ValueHeader As System.Windows.Forms.ColumnHeader

End Class
