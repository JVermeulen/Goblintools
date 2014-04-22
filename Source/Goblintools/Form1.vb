Imports System.IO
Imports Goblintools.Gis

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.TextBox1.Text = Path.Combine(My.Application.Info.DirectoryPath, "Resources", "x2c" & GrdFile.Extension)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        CreateImage()
    End Sub

    Private Sub CreateImage()
        If File.Exists(Me.TextBox1.Text) Then
            Dim file As GrdFile

            Using reader As New GrdReader(Me.TextBox1.Text)
                file = reader.ReadFile()
            End Using

            Me.PictureBox1.Image = file.CreateImage(True, True)

            'Path.Combine(My.Application.Info.DirectoryPath, name & "." & Imaging.ImageFormat.Png.ToString()), Imaging.ImageFormat.Png
        End If
    End Sub

End Class
