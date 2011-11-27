using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Chraft.Net;

namespace MinecraftLibrary
{
    public class Client
    {
        public Action<string> output2 ;
        int state = 0; // 0 not connected // 1 sent handshake // 2 sent login // 3 normal operation
        public string name = "";
        public string pass = "";
        NetworkStream str;
        TcpClient client = new TcpClient();
        Queue<Packet> packets=new Queue<Packet>();
        Queue<byte> dataIn = new Queue<byte>();
        public class packetReceivedEventArgs : EventArgs
        {
            public packetReceivedEventArgs(Packet pack,int ID)
            {
                this.packet = pack;
                this.ID = ID;
            }
            public Packet packet;
            public int ID;
        }
        public delegate void packetReceivedEventHandler(object sender, packetReceivedEventArgs e);
        public event packetReceivedEventHandler packetReceived;
        void packetSender()
        {
            while (client.Connected)
            {
                Thread.Sleep(100);
                while (packets.Count > 0)
                {
                    lock (packets)
                    {
                        lock (str)
                        {
                            byte[] data = packets.Dequeue().write();
                            str.Write(data, 0, data.Length);
                            str.Flush();
                            output("OUT: " + BitConverter.ToString(data));
                        }
                    }
                }
            }
        }
        void packetReceiver()
        {
            while (client.Connected)
            {
                Thread.Sleep(100);
                lock (str)
                {
                    lock (dataIn)
                    {
                        while (str.DataAvailable)
                        {
                            byte[] tmp=new byte[1];
                            str.Read(tmp,0,1);
                            dataIn.Enqueue(tmp[0]);
                        }
                        while (dataIn.Count > 0)
                        {
                            byte tmp = dataIn.Peek();
                            Packet packet=null;
                            switch (tmp)
                            {
                                case 0x00:
                                    packet = new Packet_KeepAlive();
                                    break;
                                case 0x01:
                                    packet = new Packet_Login();
                                    break;
                                case 0x02:
                                    packet = new Packet_Handshake();
                                    break;
                                case 0x03:
                                    packet = new Packet_Chat();
                                    break;
                                case 0x04:
                                    packet = new Packet_Time();
                                    break;
                                case 0x06:
                                    packet = new Packet_SpawnPosition();
                                    break;
                                case 0xC9:
                                    packet = new Packet_PlayerListItem();
                                    break;
                                case 0xFF:
                                    packet = new Packet_Kick();
                                    break;
                            }
                            output("IN: " + BitConverter.ToString(dataIn.ToArray()));
                            if (packet != null)
                            {
                                if (packet.read(dataIn)) {  packetReceived(this, new packetReceivedEventArgs(packet, (int)tmp)); }
                            }
                       }
                    }
                }
            }
        }

        public void output(string data)
        {
            System.Diagnostics.Debug.WriteLine(data);
            output2(data);
        }

        public void connect(string address, int port)
        {
            //PacketMap.Initialize();
            //ChraftTestClient.TestClient client = new ChraftTestClient.TestClient("agsbot", new Random());
            ////client.Start(new IPEndPoint(IPAddress.Parse("192.168.59.137"), 25566));
            //client.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25566));
            output("Connecting...");
            packetReceived += new packetReceivedEventHandler(onPacketReceived);
            client.NoDelay = true;
            client.Connect(address, port);
            str = client.GetStream();

            Thread packetSenderThread = new Thread(new ThreadStart(packetSender));
            packetSenderThread.Name = "PacketSender";
            packetSenderThread.Start();

            Thread packetReceiverThread = new Thread(new ThreadStart(packetReceiver));
            packetReceiverThread.Name = "packetReceiver";
            packetReceiverThread.Start();

            Packet_Handshake packet = new Packet_Handshake();
            packet.dataString = name;
            packets.Enqueue(packet);
            state = 1;
        }

        public void onPacketReceived(object sender,packetReceivedEventArgs e) 
        {
            switch (e.ID)
            {
                case 0:
                    output("Keep Alive");
                    packets.Enqueue(e.packet);
                    break;
                case 1:
                    output("Login success!");
                    break;
                case 2:
                    output("Beginning Login...");
                    packets.Enqueue(new Packet_Login(){username="agsBot",protocol=22});
                    break;
                case 3:
                    output(((Packet_Chat)e.packet).dataString);
                    break;
                case 6:
                    output("SpawnPosition");
                    break;
                case 255:
                    output("Kicked: " + ((Packet_Kick)e.packet).dataString);
                    break;
            }
        }
    }
}

