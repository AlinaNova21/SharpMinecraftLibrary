Imports System.IO
Imports System.Text
Imports MinecraftLibrary

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    End Sub
    Shared Sub writeDebug(ByVal txt As String, Optional ByVal show As Boolean = False)
        Debug.WriteLine(txt)
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
    Shared Sub respawn(ByVal sender As Object, ByVal e As MinecraftLibrary.Client.packetReceivedEventArgs)

        If e.ID = 8 Then
            Dim h As Packet_UpdateHealth = e.packet
            Console.WriteLine("Health Update: {0} {1}", h.health, h.food)
            If h.health <= 0 Then
                Dim r As New MinecraftLibrary.Packet_Respawn()
                r.dim = 0
                r.difficulty = &H1
                r.creative = &H0
                r.levelType = "default"
                r.worldHeight = 256
                mc.sendPacket(r)
            End If
        End If
    End Sub
    Public Shared mc As New MinecraftLibrary.Client
    Shared Sub Main(ByVal args() As String)
        AddHandler mc.packetReceived, AddressOf respawn
        wl("Welcome to SharpMCLibrary")
        w("Please enter a name:")
        mc.name = Console.ReadLine()
        mc.output2 = AddressOf writeDebug
        'mc.registerPacket(&H3, GetType(MinecraftLibrary.Packet_Chat))
        'mc.connect("192.168.59.137", 25566) 'MineOS+
        'mc.connect("192.168.159.129", 25566) 'MineOS+
        'mc.connect("127.0.0.1", 25564) 'SMPROXY
        'mc.connect("178.33.81.147", 25565)
        mc.connect("37.59.228.108", 25565)
        'mc.packetReceivedEventHandler+
        Dim tmp As String = ""
        While Not tmp = ":exit" And Not tmp = ":quit"
            tmp = Console.ReadLine()
            If tmp.StartsWith(":") Then
                If tmp = ":respawn" Then
                    Dim r As New MinecraftLibrary.Packet_Respawn()
                    r.dim = 0
                    r.difficulty = &H1
                    r.creative = &H0
                    r.levelType = "default"
                    r.worldHeight = 256
                    mc.sendPacket(r)
                End If
            Else
                mc.sendPacket(New MinecraftLibrary.Packet_Chat() With {.dataString = tmp})
            End If
        End While
        mc.disconnect()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
