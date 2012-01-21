Imports System.IO
Imports System.Text

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    End Sub
    Shared Sub writeDebug(ByVal txt As String, Optional ByVal show As Boolean = False)
        debug.WriteLine(txt)
        If show Then
            wl(txt)
        End If
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
        wl("Welcome to SharpMCLibrary")
        Dim mc As New MinecraftLibrary.Client
        w("Please enter a name:")
        mc.name = Console.ReadLine()
        mc.output2 = AddressOf writeDebug
        'mc.registerPacket(&H3, GetType(MinecraftLibrary.Packet_Chat))
        'mc.connect("192.168.59.137", 25566) 'MineOS+
        'mc.connect("192.168.159.129", 25566) 'MineOS+
        'mc.connect("127.0.0.1", 25566)
        mc.connect("178.33.81.147", 25565)
        Dim tmp As String = ""
        While True
            tmp = Console.ReadLine()
            mc.sendPacket(New MinecraftLibrary.Packet_Chat() With {.dataString = tmp})
        End While
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
