Imports System.IO
Imports System.Text

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    End Sub
    Shared Sub wl(ByVal txt As String)
        Console.WriteLine(txt)
        Debug.WriteLine(txt)
    End Sub
    Shared Sub w(ByVal txt As String)
        Console.Write(txt)
        Debug.Write(txt)
    End Sub
    Shared Sub Main(ByVal args() As String)
        wl("Initializing...")
        Dim mc As New MinecraftLibrary.Client
        mc.name = "agsBot"
        mc.output2 = AddressOf wl
        'mc.connect("192.168.59.137", 25565) 'MineOS+
        'mc.connect("127.0.0.1", 25566)
        mc.connect("178.33.81.147", 25565)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
