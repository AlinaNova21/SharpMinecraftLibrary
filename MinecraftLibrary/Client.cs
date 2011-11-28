using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Chraft.Net;
using System.IO;

namespace MinecraftLibrary
{
    public class Client
    {
        const int Protocol = 22;
        public Action<string> output2 ;
        int state = 0; // 0 not connected // 1 sent handshake // 2 sent login // 3 normal operation
        public string name = "";
        public string pass = "";
        int keepAliveCnt = 0;
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
            Stream str = client.GetStream();
            while (client.Connected)
            {
                Packet pack=null;
                bool p = false;
                lock (packets)
                {
                    p = packets.Count > 0;
                    if (p) pack = packets.Dequeue();
                }
                if (p) lock (str)
                {
                    MemoryStream tmp = new MemoryStream();
                    pack.write(tmp);
                    //str.Write(data, 0, data.Length);
                    tmp.WriteTo(str);
                    output(BitConverter.ToString(tmp.ToArray()));
                    tmp.Close();
                    str.Flush();
                    output("OUT: " + pack.GetType().ToString().Split('_')[1]);
                }

            }
        }
        void packetReceiver()
        {
            byte[] tmp = new byte[1];
            while (client.Connected)
            {
                lock (dataIn)
                {
                    lock (str)
                    {
                        while (str.DataAvailable)
                        {
                            str.Read(tmp, 0, 1);
                            dataIn.Enqueue(tmp[0]);
                        }
                    }
                }
            }
        }
        void packetHandler()
        {
            byte[] tmp = new byte[1];
            Packet packet;
            Queue<byte> tmpQueue;
            Stream str = client.GetStream();
            while (client.Connected)
            {
                packet = null;
                str.Read(tmp, 0, 1);
                switch (tmp[0])
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
                    case 0x05:
                        packet = new Packet_EntityEquipment();
                        break;
                    case 0x06:
                        packet = new Packet_SpawnPosition();
                        break;
                    case 0x08:
                        packet = new Packet_UpdateHealth();
                        break;
                    case 0x09:
                        packet = new Packet_Respawn();
                        break;
                    case 0x0D:
                        packet = new Packet_PlayerPosAndLook();
                        break;
                    case 0x11:
                        packet = new Packet_UseBed();
                        break;
                    case 0x12:
                        packet = new Packet_Animation();
                        break;
                    case 0x14:
                        packet = new Packet_NamedEntitySpawn();
                        break;
                    case 0x15:
                        packet = new Packet_PickupSpawn();
                        break;
                    case 0x16:
                        packet = new Packet_CollectItem();
                        break;
                    case 0x17:
                        packet = new Packet_AddObjVehicle();
                        break;
                    case 0x18:
                        packet = new Packet_MobSpawn();
                        break;
                    case 0x1A:
                        packet = new Packet_ExpOrb();
                        break;
                    case 0x1C:
                        packet = new Packet_EntityVel();
                        break;
                    case 0x1D:
                        packet = new Packet_DestroyEntity();
                        break;
                    case 0x1E:
                        packet = new Packet_Entity();
                        break;
                    case 0x1F:
                        packet = new Packet_EntityRelativeMove();
                        break;
                    case 0x19:
                        packet = new Packet_EntityPainting();
                        break;
                    case 0x20:
                        packet = new Packet_EntityLook();
                        break;
                    case 0x21:
                        packet = new Packet_EntityLookAndRelativeMove();
                        break;
                    case 0x22:
                        packet = new Packet_EntityTeleport();
                        break;
                    case 0x26:
                        packet = new Packet_EntityStatus();
                        break;
                    case 0x27:
                        packet = new Packet_AttachEntity();
                        break;
                    case 0x29:
                        packet = new Packet_EntityEffect();
                        break;
                    case 0x2A:
                        packet = new Packet_RemoveEntityEffect();
                        break;
                    case 0x32:
                        packet = new Packet_PreChunk();
                        break;
                    case 0x33:
                        packet = new Packet_MapChunk();
                        break;
                    case 0x34:
                        packet = new Packet_MultiBlockChange();
                        break;
                    case 0x35:
                        packet = new Packet_BlockChange();
                        break;
                    case 0x36:
                        packet = new Packet_BlockAction();
                        break;
                    case 0x3D:
                        packet = new Packet_SoundEffect();
                        break;
                    case 0x46:
                        packet = new Packet_NewOrInvalidState();
                        break;
                    case 0x47:
                        packet = new Packet_Thunder();
                        break;
                    case 0x65:
                        packet = new Packet_CloseWnd();
                        break;
                    case 0x67:
                        packet = new Packet_SetSlot();
                        break;
                    case 0x68:
                        packet = new Packet_WndItems();
                        break;
                    case 0x69:
                        packet = new Packet_UpdateWndProp();
                        break;
                    case 0x82:
                        packet = new Packet_UpdateSign();
                        break;
                    case 0xC8:
                        packet = new Packet_IncStatistic();
                        break;
                    case 0xC9:
                        packet = new Packet_PlayerListItem();
                        break;
                    case 0xFF:
                        packet = new Packet_Kick();
                        break;
                }
                //output("IN: " +  Encoding.Unicode.GetString(dataIn.ToArray()));
                if (packet != null)
                {
                    output("IN: " + packet.GetType().ToString().Split('_')[1]);
                    packet.read(str);
                    packetReceived(this, new packetReceivedEventArgs(packet, (int)tmp[0]));
                }
            }
        }

        public void output(string data)
        {
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
            //packetReceiverThread.Start();

            Thread packetHandlerThread = new Thread(new ThreadStart(packetHandler));
            packetHandlerThread.Name = "packetHandler";
            packetHandlerThread.Start();

            Packet_Handshake packet = new Packet_Handshake();
            packet.dataString = name;
            packets.Enqueue(packet);
            state = 1;
        }

        public void keepAlive(object state)
        {
            packets.Enqueue(new Packet_KeepAlive() {ID=0});
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
                    //Timer tmp=new Timer(keepAlive, null, 100, 100);
                    packets.Enqueue(new Packet_Chat() {dataString="/login cardman" });
                    break;
                case 2:
                    output("Beginning Login...");
                    packets.Enqueue(new Packet_Login(){username="agsBot",protocol=Protocol});
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
            //Console.WriteLine(BitConverter.ToString(new byte[]{(byte)e.ID},0));
        }
    }
}

