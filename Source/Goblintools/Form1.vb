Imports System.IO
Imports Goblintools.Gis

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConvertGrdToPng("x2c")

        Me.Close()
    End Sub

    Public Sub ConvertGrdToPng(name As String)
        Dim file As GrdFile

        Using reader As New GrdReader(Path.Combine(My.Application.Info.DirectoryPath, "Resources", name & GrdFile.Extension))
            file = reader.ReadFile()
        End Using

        Dim value As Single = file.GetValueBilinear(155000, 463000)

        GrdImage.CreateImage(file, True, True).Save(Path.Combine(My.Application.Info.DirectoryPath, name & "." & Imaging.ImageFormat.Png.ToString()), Imaging.ImageFormat.Png)
    End Sub

End Class
