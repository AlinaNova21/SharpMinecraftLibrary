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
        const int launcherVersion = 13;
        public double x = 0;
        public double y = 0;
        public double stance = 0;
        public double z = 0;
        public float pitch = 0;
        public float yaw = 0;
        public bool onGround = false;
        public Action<string, Boolean> output2;
        public string name = "";
        public string pass = "";
        bool loggedin = false;
        string sessionid = "";
        NetworkStream str;
        TcpClient client = new TcpClient();
        Queue<Packet> packets = new Queue<Packet>();
        Queue<byte> dataIn = new Queue<byte>();
        Dictionary<PacketType, Type> customPacketType = new Dictionary<PacketType, Type>();

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
                p.stance = stance;
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
                    tmps.WriteTo(str);
                    tmps.Close();
                    str.Flush();
                }
                packet = null;
                str.Read(tmp, 0, 1);
                if (packet == null && customPacketType.ContainsKey((PacketType)tmp[0]))
                {
                    packet = (Packet)customPacketType[(PacketType)tmp[0]].GetConstructor(Type.EmptyTypes).Invoke(null);
                }
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
            }
        }

        public bool Verify()
        {
            WebClient wc = new WebClient();
            string loginstring = wc.DownloadString(string.Format("https://login.minecraft.net/?user={0}&password={1}&version={2}", name, pass, launcherVersion));
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
            registerPacket(PacketType.KeepAlive, typeof(Packet_KeepAlive));
            registerPacket(PacketType.Login, typeof(Packet_Login));
            registerPacket(PacketType.Handshake, typeof(Packet_Handshake));
            registerPacket(PacketType.Chat, typeof(Packet_Chat));
            registerPacket(PacketType.Time, typeof(Packet_Time));
            registerPacket(PacketType.EntityEquipment, typeof(Packet_EntityEquipment));
            registerPacket(PacketType.SpawnPosition, typeof(Packet_SpawnPosition));
            registerPacket(PacketType.UseEntity, typeof(Packet_UseEntity));
            registerPacket(PacketType.UpdateHealth, typeof(Packet_UpdateHealth));
            registerPacket(PacketType.Respawn, typeof(Packet_Respawn));
            registerPacket(PacketType.Player, typeof(Packet_Player));
            registerPacket(PacketType.PlayerPos, typeof(Packet_PlayerPos));
            registerPacket(PacketType.PlayerLook, typeof(Packet_PlayerLook));
            registerPacket(PacketType.PlayerPosAndLook, typeof(Packet_PlayerPosAndLook));
            registerPacket(PacketType.PlayerDigging, typeof(Packet_PlayerDigging));
            registerPacket(PacketType.UseBed, typeof(Packet_UseBed));
            registerPacket(PacketType.Animation, typeof(Packet_Animation));
            registerPacket(PacketType.EntityAction, typeof(Packet_EntityAction));
            registerPacket(PacketType.NamedEntitySpawn, typeof(Packet_NamedEntitySpawn));
            registerPacket(PacketType.PickupSpawn, typeof(Packet_PickupSpawn));
            registerPacket(PacketType.CollectItem, typeof(Packet_CollectItem));
            registerPacket(PacketType.AddObjVehicle, typeof(Packet_AddObjVehicle));
            registerPacket(PacketType.MobSpawn, typeof(Packet_MobSpawn));
            registerPacket(PacketType.EntityPainting, typeof(Packet_EntityPainting));
            registerPacket(PacketType.ExpOrb, typeof(Packet_ExpOrb));
            registerPacket(PacketType.EntityVel, typeof(Packet_EntityVel));
            registerPacket(PacketType.DestroyEntity, typeof(Packet_DestroyEntity));
            registerPacket(PacketType.Entity, typeof(Packet_Entity));
            registerPacket(PacketType.EntityRelativeMove, typeof(Packet_EntityRelativeMove));
            registerPacket(PacketType.EntityLook, typeof(Packet_EntityLook));
            registerPacket(PacketType.EntityLookAndRelativeMove, typeof(Packet_EntityLookAndRelativeMove));
            registerPacket(PacketType.EntityTeleport, typeof(Packet_EntityTeleport));
            registerPacket(PacketType.EntityHeadLook, typeof(Packet_EntityHeadLook));
            registerPacket(PacketType.EntityStatus, typeof(Packet_EntityStatus));
            registerPacket(PacketType.AttachEntity, typeof(Packet_AttachEntity));
            registerPacket(PacketType.EntityMetadata, typeof(Packet_EntityMetadata));
            registerPacket(PacketType.EntityEffect, typeof(Packet_EntityEffect));
            registerPacket(PacketType.RemoveEntityEffect, typeof(Packet_RemoveEntityEffect));
            registerPacket(PacketType.Experience, typeof(Packet_Experience));
            registerPacket(PacketType.PreChunk, typeof(Packet_PreChunk));
            registerPacket(PacketType.MapChunk, typeof(Packet_MapChunk));
            registerPacket(PacketType.MultiBlockChange, typeof(Packet_MultiBlockChange));
            registerPacket(PacketType.BlockChange, typeof(Packet_BlockChange));
            registerPacket(PacketType.BlockAction, typeof(Packet_BlockAction));
            registerPacket(PacketType.Explosion, typeof(Packet_Explosion));
            registerPacket(PacketType.SoundEffect, typeof(Packet_SoundEffect));
            registerPacket(PacketType.NewOrInvalidState, typeof(Packet_NewOrInvalidState));
            registerPacket(PacketType.Thunder, typeof(Packet_Thunder));
            registerPacket(PacketType.OpenWnd, typeof(Packet_OpenWnd));
            registerPacket(PacketType.CloseWnd, typeof(Packet_CloseWnd));
            registerPacket(PacketType.WndClick, typeof(Packet_WndClick));
            registerPacket(PacketType.SetSlot, typeof(Packet_SetSlot));
            registerPacket(PacketType.WndItems, typeof(Packet_WndItems));
            registerPacket(PacketType.UpdateWndProp, typeof(Packet_UpdateWndProp));
            registerPacket(PacketType.Transaction, typeof(Packet_Transaction));
            registerPacket(PacketType.CreativeInventoryAction, typeof(Packet_CreativeInventoryAction));
            registerPacket(PacketType.UpdateSign, typeof(Packet_UpdateSign));
            registerPacket(PacketType.ItemData, typeof(Packet_ItemData));
            registerPacket(PacketType.EntityTileUpdate, typeof(Packet_EntityTileUpdate));
            registerPacket(PacketType.IncStatistic, typeof(Packet_IncStatistic));
            registerPacket(PacketType.PlayerListItem, typeof(Packet_PlayerListItem));
            registerPacket(PacketType.PlayerAbilities, typeof(Packet_PlayerAbilities));
            registerPacket(PacketType.PluginMessage, typeof(Packet_PluginMessage));
            //registerPacket(PacketType.ServerListPing, typeof(Packet_ServerListPing));
            registerPacket(PacketType.Kick, typeof(Packet_Kick));

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
            if (loggedin)
                lock (packets)
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
                    packets.Enqueue(e.packet);
                    break;
                case 1:
                    output("Login success!", true);
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
                            if (cc.ContainsKey(tmp[0]))
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
                    this.pitch = p.pitch;
                    this.yaw = p.yaw;
                    sendPacket(p);
                    output("Moved!", true);
                    break;
                case 255:
                    output("Kicked: " + ((Packet_Kick)e.packet).dataString, true);
                    break;
            }
        }

        public void registerPacket(PacketType id, Type packet)
        {
            if (customPacketType.ContainsKey(id))
            {
                throw new InvalidOperationException("Packet ID already registered");
            }
            else if (!packet.IsSubclassOf(typeof(Packet)))
            {
                throw new InvalidOperationException("Invalid packet class! Must inherit Packet");
            }
            else
            {
                customPacketType.Add(id, packet);
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