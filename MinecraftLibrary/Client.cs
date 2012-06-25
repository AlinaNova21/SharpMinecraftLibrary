using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;

namespace MinecraftLibrary
{
    public class Client
    {
        const int Protocol = 29; 
        public double x=0;
        public double y = 0;
        public double stance = 0;
        public double z = 0;
        public float yaw = 0;
        public float pitch = 0;
        public bool onGround=false;
        public Action<string, Boolean> output2;
        public string name = "";
        public string pass = "";
        bool running = true;
        bool loggedin = false;
        string sessionid = "";
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
            while (client.Connected)
            {
                Thread.Sleep(750);
                Packet_PlayerPosAndLook p = new Packet_PlayerPosAndLook();
                p.x = x;
                p.y = y;
                p.z = z;
                p.stance =stance;
                p.pitch = pitch;
                p.yaw = yaw;
                sendPacket(p);
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
                    //output(BitConverter.ToString(tmps.ToArray()));
                    tmps.Close();
                    str.Flush();
                    //output("OUT: " + pack.GetType().ToString().Split('_')[1]);
                }
                packet = null;
                str.Read(tmp, 0, 1);
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
        public bool Verify()
        {
            WebClient wc = new WebClient();
            string loginstring = wc.DownloadString(string.Format("https://login.minecraft.net/?user={0}&password={1}&version={2}", name, pass, Protocol));
            Console.WriteLine(loginstring);
            if (!loginstring.Contains(":")) { return false; }
            string[] loginarray = loginstring.Split(':');
            name = loginarray[2];
            sessionid = loginarray[3];

            return true;
        }
        public void output(string data, Boolean show = false)
        {
            output2(data, show);
        }

        public void connect(string address, int port)
        {
            registerPacket(0x00, typeof(Packet_KeepAlive));
            registerPacket(0x01, typeof(Packet_Login));
            registerPacket(0x02, typeof(Packet_Handshake));
            registerPacket(0x03, typeof(Packet_Chat));
            registerPacket(0x04, typeof(Packet_Time));
            registerPacket(0x05, typeof(Packet_EntityEquipment));
            registerPacket(0x06, typeof(Packet_SpawnPosition));
            registerPacket(0x07, typeof(Packet_UseEntity));
            registerPacket(0x08, typeof(Packet_UpdateHealth));
            registerPacket(0x09, typeof(Packet_Respawn));
            registerPacket(0x0A, typeof(Packet_Player));
            registerPacket(0x0B, typeof(Packet_PlayerPos));
            registerPacket(0x0C, typeof(Packet_PlayerLook));
            registerPacket(0x0D, typeof(Packet_PlayerPosAndLook));
            registerPacket(0x0E, typeof(Packet_PlayerDigging));
            registerPacket(0x11, typeof(Packet_UseBed));
            registerPacket(0x12, typeof(Packet_Animation));
            registerPacket(0x13, typeof(Packet_EntityAction));
            registerPacket(0x14, typeof(Packet_NamedEntitySpawn));
            registerPacket(0x15, typeof(Packet_PickupSpawn));
            registerPacket(0x16, typeof(Packet_CollectItem));
            registerPacket(0x17, typeof(Packet_AddObjVehicle));
            registerPacket(0x18, typeof(Packet_MobSpawn));
            registerPacket(0x19, typeof(Packet_EntityPainting));
            registerPacket(0x1A, typeof(Packet_ExpOrb));
            registerPacket(0x1C, typeof(Packet_EntityVel));
            registerPacket(0x1D, typeof(Packet_DestroyEntity));
            registerPacket(0x1E, typeof(Packet_Entity));
            registerPacket(0x1F, typeof(Packet_EntityRelativeMove));
            registerPacket(0x20, typeof(Packet_EntityLook));
            registerPacket(0x21, typeof(Packet_EntityLookAndRelativeMove));
            registerPacket(0x22, typeof(Packet_EntityTeleport));
            registerPacket(0x23, typeof(Packet_EntityHeadLook));
            registerPacket(0x26, typeof(Packet_EntityStatus));
            registerPacket(0x27, typeof(Packet_AttachEntity));
            registerPacket(0x28, typeof(Packet_EntityMetadata));
            registerPacket(0x29, typeof(Packet_EntityEffect));
            registerPacket(0x2A, typeof(Packet_RemoveEntityEffect));
            registerPacket(0x2B, typeof(Packet_Experience));
            registerPacket(0x32, typeof(Packet_PreChunk));
            registerPacket(0x33, typeof(Packet_MapChunk));
            registerPacket(0x34, typeof(Packet_MultiBlockChange));
            registerPacket(0x35, typeof(Packet_BlockChange));
            registerPacket(0x36, typeof(Packet_BlockAction));
            registerPacket(0x3C, typeof(Packet_Explosion));
            registerPacket(0x3D, typeof(Packet_SoundEffect));
            registerPacket(0x46, typeof(Packet_NewOrInvalidState));
            registerPacket(0x47, typeof(Packet_Thunder));
            registerPacket(0x64, typeof(Packet_OpenWnd));
            registerPacket(0x65, typeof(Packet_CloseWnd));
            registerPacket(0x66, typeof(Packet_WndClick));
            registerPacket(0x67, typeof(Packet_SetSlot));
            registerPacket(0x68, typeof(Packet_WndItems));
            registerPacket(0x69, typeof(Packet_UpdateWndProp));
            registerPacket(0x6A, typeof(Packet_Transaction));
            registerPacket(0x6B, typeof(Packet_CreativeInventoryAction));
            registerPacket(0x82, typeof(Packet_UpdateSign));
            registerPacket(0x83, typeof(Packet_ItemData));
            registerPacket(0x84, typeof(Packet_EntityTileUpdate));
            registerPacket(0xC8, typeof(Packet_IncStatistic));
            registerPacket(0xC9, typeof(Packet_PlayerListItem));
            registerPacket(0xCA, typeof(Packet_PlayerAbilities));
            registerPacket(0xFA, typeof(Packet_PluginMessage));
            //registerPacket(0xFE, typeof(Packet_ServerListPing));
            registerPacket(0xFF, typeof(Packet_Kick));
            //PacketMap.Initialize();
            if (File.Exists("Out.bin"))
                File.Delete("Out.bin");
            output("Connecting...");
            packetReceived += new packetReceivedEventHandler(onPacketReceived);
            client.NoDelay = true;
            client.Connect(address, port);
            str = client.GetStream();

            Thread packetReceiverThread = new Thread(new ThreadStart(packetReceiver));
            packetReceiverThread.Name = "packetReceiver";
            //packetReceiverThread.Start();

            Thread packetHandlerThread = new Thread(new ThreadStart(packetHandler));
            packetHandlerThread.Name = "packetHandler";
            packetHandlerThread.Start();

            Packet_Handshake packet = new Packet_Handshake();
            packet.dataString = name + ";" + address + ":" + port;
            packets.Enqueue(packet);            

        }
        public void disconnect()
        {
            sendPacket(new Packet_Kick() { dataString = "Closing" });
            while (packets.Count > 0)
                Thread.Sleep(100);
            client.Close();
        }
        public void sendPacket(Packet pack)
        {
            if(loggedin)
                lock(packets)
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
                    loggedin = true;
                    Thread packetSenderThread = new Thread(new ThreadStart(packetSender));
                    packetSenderThread.Name = "PacketSender";
                    packetSenderThread.Start();
                    break;
                case 2:
                    output("Beginning Login...", true);
                    string serverid = ((Packet_Handshake)e.packet).dataString;
                    packets.Enqueue(new Packet_Login() { username = name, protocol = Protocol, serverid = serverid, sessionid = sessionid });
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
                case 0x0D:
                    Packet_PlayerPosAndLook p = (Packet_PlayerPosAndLook)e.packet;
                    this.x = p.x;
                    this.y = p.y;
                    this.z = p.z;
                    this.stance = p.stance;
                    this.pitch=p.pitch;
                    this.yaw = p.yaw;
                    sendPacket(p);
                   output("Moved!",true);
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
                    Thread.Sleep(10);
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

