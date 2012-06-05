using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace MinecraftLibrary
{
    public class Client
    {
        const int Protocol = 29;
        public Action<string, Boolean> output2;
        public string name = "";
        public string pass = "";
        NetworkStream str;
        TcpClient client = new TcpClient();
        Queue<Packet> packets = new Queue<Packet>();
        Queue<byte> dataIn = new Queue<byte>();
        Dictionary<byte, Type> customPackets = new Dictionary<byte, Type>();
        public class packetReceivedEventArgs : EventArgs
        {
            public packetReceivedEventArgs(Packet pack, int ID)
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
                Packet pack = null;
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
            bool debug = false;
            byte[] tmp = new byte[1];
            Packet packet;
            Stream str = new blockingStream(client.GetStream(), debug);
            while (client.Connected)
            {
                Packet pack = null;
                bool p = false;
                lock (packets)
                {
                    p = packets.Count > 0;
                    if (p) pack = packets.Dequeue();
                }
                if (p)
                {
                    MemoryStream tmps = new MemoryStream();
                    pack.write(tmps);
                    //str.Write(data, 0, data.Length)
                    tmps.WriteTo(str);
                    output(BitConverter.ToString(tmps.ToArray()));
                    tmps.Close();
                    str.Flush();
                    //output("OUT: " + pack.GetType().ToString().Split('_')[1]);
                }
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
                    case 0x0A:
                        packet = new Packet_Player();
                        break;
                    case 0x0B:
                        packet = new Packet_PlayerPos();
                        break;
                    case 0x0C:
                        packet = new Packet_PlayerLook();
                        break;
                    case 0x0D:
                        packet = new Packet_PlayerPosAndLook();
                        break;
                    case 0x0E:
                        packet = new Packet_PlayerDigging();
                        break;
                    case 0x0F:
                        packet = new Packet_PlayerBlockPlacement();
                        break;
                    case 0x10:
                        packet = new Packet_HoldingChange();
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
                    case 0x23:
                        packet = new Packet_EntityHeadLook();
                        break;
                    case 0x26:
                        packet = new Packet_EntityStatus();
                        break;
                    case 0x27:
                        packet = new Packet_AttachEntity();
                        break;
                    case 0x28:
                        packet = new Packet_EntityMetadata();
                        break;
                    case 0x29:
                        packet = new Packet_EntityEffect();
                        break;
                    case 0x2A:
                        packet = new Packet_RemoveEntityEffect();
                        break;
                    case 0x2B:
                        packet = new Packet_Experience();
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
                    case 0xCA:
                        packet = new Packet_PlayerAbilities();
                        break;
                    case 0xFA:
                        packet = new Packet_PluginMessage();
                        break;
                    case 0xFF:
                        packet = new Packet_Kick();
                        break;
                }
                if (packet == null && customPackets.ContainsKey(tmp[0]))
                {
                    packet = (Packet)customPackets[tmp[0]].GetConstructor(Type.EmptyTypes).Invoke(null);
                }

                //output("IN: " +  BitConverter.ToString(tmp));
                if (packet != null)
                {
                    if (debug)
                        output("Packet: " + packet.GetType().ToString().Split('_')[1]);
                    packet.read(str);
                    packetReceived(this, new packetReceivedEventArgs(packet, (int)tmp[0]));
                }
                else
                {
                    throw new InvalidDataException("Unhandled Packet! " + BitConverter.ToString(tmp, 0));
                }
                //Thread.Sleep(10);
            }
        }

        public void output(string data, Boolean show = false)
        {
            output2(data, show);
        }

        public void connect(string address, int port)
        {
            //PacketMap.Initialize();
            //ChraftTestClient.TestClient client = new ChraftTestClient.TestClient("agsbot", new Random());
            ////client.Start(new IPEndPoint(IPAddress.Parse("192.168.59.137"), 25566));
            //client.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25566));
            if (File.Exists("Out.bin"))
                File.Delete("Out.bin");
            output("Connecting...");
            packetReceived += new packetReceivedEventHandler(onPacketReceived);
            client.NoDelay = true;
            client.Connect(address, port);
            str = client.GetStream();

            Thread packetSenderThread = new Thread(new ThreadStart(packetSender));
            packetSenderThread.Name = "PacketSender";
            //packetSenderThread.Start();

            Thread packetReceiverThread = new Thread(new ThreadStart(packetReceiver));
            packetReceiverThread.Name = "packetReceiver";
            //packetReceiverThread.Start();

            Thread packetHandlerThread = new Thread(new ThreadStart(packetHandler));
            packetHandlerThread.Name = "packetHandler";
            packetHandlerThread.Start();

            Packet_Handshake packet = new Packet_Handshake();
            packet.dataString = name;// +";" + address + ":" + port;
            packets.Enqueue(packet);
        }
        public void sendPacket(Packet pack)
        {
            packets.Enqueue(pack);
        }
        public void keepAlive(object state)
        {
            packets.Enqueue(new Packet_KeepAlive() { ID = 0 });
        }
        public void onPacketReceived(object sender, packetReceivedEventArgs e)
        {
            switch (e.ID)
            {
                case 0:
                    //output("Keep Alive");
                    packets.Enqueue(e.packet);
                    break;
                case 1:
                    output("Login success!", true);
                    //Timer tmp=new Timer(keepAlive, null, 100, 100);
                    // packets.Enqueue(new Packet_Chat() {dataString="/login *PassordRemoved*" });
                    break;
                case 2:
                    output("Beginning Login...", true);
                    packets.Enqueue(new Packet_Login() { username = name, protocol = Protocol });
                    break;
                case 3:
                    Dictionary<char, ConsoleColor> cc = new Dictionary<char, ConsoleColor>();
                    cc.Add('0', ConsoleColor.Black);
                    cc.Add('1', ConsoleColor.DarkBlue);
                    cc.Add('2', ConsoleColor.DarkGreen);
                    cc.Add('3', ConsoleColor.DarkCyan);
                    cc.Add('4', ConsoleColor.DarkRed);
                    cc.Add('5', ConsoleColor.DarkMagenta);
                    cc.Add('6', ConsoleColor.DarkYellow);
                    cc.Add('7', ConsoleColor.Gray);
                    cc.Add('8', ConsoleColor.DarkGray);
                    cc.Add('9', ConsoleColor.Blue);
                    cc.Add('a', ConsoleColor.Green);
                    cc.Add('b', ConsoleColor.Cyan);
                    cc.Add('c', ConsoleColor.Red);
                    cc.Add('d', ConsoleColor.Magenta);
                    cc.Add('e', ConsoleColor.Yellow);
                    cc.Add('f', ConsoleColor.White);

                    string msg = ((Packet_Chat)e.packet).dataString;
                    output(msg);
                    StreamReader sr = new StreamReader(new MemoryStream(Encoding.Default.GetBytes(msg)));
                    char[] tmp = new char[1];
                    while (!sr.EndOfStream)
                    {
                        sr.Read(tmp, 0, 1);
                        if (tmp[0] == (char)65533)
                        {
                            sr.Read(tmp, 0, 1);
                            if(cc.ContainsKey(tmp[0]))
                                Console.ForegroundColor = cc[tmp[0]];
                            else
                                Console.Write(tmp[0]);
                        }
                        else
                        {
                            Console.Write(tmp[0]);
                        }
                    }
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 255:
                    //Console.WriteLine("Kicked: " + ((Packet_Kick)e.packet).dataString);
                    output("Kicked: " + ((Packet_Kick)e.packet).dataString, true);
                    break;
            }
            //Console.WriteLine(BitConverter.ToString(new byte[]{(byte)e.ID},0));
        }

        public void registerPacket(byte id, Type packet)
        {
            if (customPackets.ContainsKey(id))
            {
                throw new InvalidOperationException("Packet ID already registered");
            }
            else if (!packet.IsSubclassOf(typeof(Packet)))
            {
                throw new InvalidOperationException("Invalid packet class! Must inherit Packet");
            }
            else
            {
                customPackets.Add(id, packet);
            }
        }
    }

    public class blockingStream : Stream
    {
        bool _debug;
        NetworkStream _str;
        public blockingStream(NetworkStream str, bool debug = false)
        {
            _str = str;
            _debug = debug;
        }
        public override bool CanRead
        {
            get { return _str.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _str.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _str.CanWrite; }
        }

        public override void Flush()
        {
            _str.Flush();
        }

        public override long Length
        {
            get { return _str.Length; }
        }

        public override long Position
        {
            get
            {
                return _str.Position;
            }
            set
            {
                _str.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //FileStream fs = new FileStream("Out.bin", FileMode.Append);
            for (int i = 0; i < count; i++)
            {
                while (!_str.DataAvailable)
                {
                    Thread.Sleep(100);
                }
                _str.Read(buffer, offset + i, 1);
                //fs.Write(buffer,offset+i,1);
            }
            if (_debug)
                System.Diagnostics.Debug.WriteLine("R:" + offset + ',' + count + ' ' + BitConverter.ToString(buffer));
            //fs.Close();
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _str.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _str.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_debug)
                System.Diagnostics.Debug.WriteLine("W:" + offset + ',' + count + ' ' + BitConverter.ToString(buffer));
            _str.Write(buffer, offset, count);
        }
    }
}

