using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;
using CraftBotLib.Networking;
using CraftBotLib.Networking.ASN1;

namespace MinecraftLibrary
{
    public class Client
    {
        const int Protocol = 39; //1.3.1!
        const int LauncherVersion = 13;
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
        Stream NetStream;
        TcpClient client = new TcpClient();
        Queue<Packet> packets = new Queue<Packet>();
        Queue<byte> dataIn = new Queue<byte>();
        Dictionary<PacketType, Type> customPackets = new Dictionary<PacketType, Type>();

        public class packetReceivedEventArgs : EventArgs
        {
            public packetReceivedEventArgs(Packet pack, int ID)
            {
                this.packet = pack;
                this.Type = (PacketType)ID;
            }
            public Packet packet;
            public PacketType Type;
        }

        public delegate void packetReceivedEventHandler(object sender, packetReceivedEventArgs e);
        public event packetReceivedEventHandler packetReceived;
        void packetSender()
        {
            while (client.Connected)
            {
                Thread.Sleep(750);
                Packet_PlayerPosAndLook p = new Packet_PlayerPosAndLook();
                p.X = x;
                p.Y = y;
                p.Z = z;
                p.Stance = stance;
                p.Pitch = pitch;
                p.Yaw = yaw;
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
            bool debug = true;
            byte[] tmp = new byte[1];
            Packet packet;
            NetStream = new blockingStream(client.GetStream(), debug);
            Stream str = NetStream;
            while (client.Connected)
            {
                str = NetStream;
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
                    pack.Stream = tmps;
                    pack.Write();
                    tmps.WriteTo(str);
                    if (debug)
                    {
                        tmps.Seek(0, SeekOrigin.Begin);
                        tmps.Read(tmp, 0, 1);
                        output("C->S "+BitConverter.ToString(tmp, 0, 1), true);
                    }
                    tmps.Close();
                    str.Flush();
                }
                packet = null;
                tmp = new byte[1];
                str.Read(tmp, 0, 1);
                if (debug)
                    output("S->C "+BitConverter.ToString(tmp,0,1),true);
                if (packet == null && customPackets.ContainsKey((PacketType)tmp[0]))
                {
                    packet = (Packet)customPackets[(PacketType)tmp[0]].GetConstructor(Type.EmptyTypes).Invoke(null);
                }
                if (packet != null)
                {
                    if (debug)
                        output("Packet: " + packet.GetType().ToString().Split('_')[1]);
                    packet.Stream = str;
                    packet.Read();
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
            string loginstring = wc.DownloadString(string.Format("https://login.minecraft.net/?user={0}&password={1}&version={2}", name, pass, LauncherVersion));
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
            registerPacket(PacketType.Flying, typeof(Packet_Flying));
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
            registerPacket(PacketType.ClientStatus, typeof(Packet_ClientStatus));
            registerPacket(PacketType.PluginMessage, typeof(Packet_PluginMessage));
            registerPacket(PacketType.EncryptionResponse, typeof(Packet_EncryptionResponse));
            registerPacket(PacketType.EncryptionRequest, typeof(Packet_EncryptionRequest));
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
            packet.ProtocolVersion = Protocol;
            packet.Username = name;
            packet.Host = address;
            packet.Port = port;

            packets.Enqueue(packet);
        }

        public void disconnect()
        {
            sendPacket(new Packet_Kick() { Reason = "Closing" });
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
        struct Server
        {
            public string ServerID;
            public byte[] PublicKey;
            public byte[] PrivateKey;
            public byte[] Token;
        }
        Server server;
        public void onPacketReceived(object sender, packetReceivedEventArgs e)
        {
            switch (e.Type)
            {
                case PacketType.KeepAlive:
                    packets.Enqueue(e.packet);
                    break;
                case PacketType.Login:
                    output("Login success!", true);
                    loggedin = true;
                    Thread packetSenderThread = new Thread(new ThreadStart(packetSender));
                    packetSenderThread.Name = "PacketSender";
                    packetSenderThread.Start();
                    break;
                case PacketType.ClientStatus:
                    output("-"+((Packet_ClientStatus)e.packet).Payload.ToString());
                    break;
                case PacketType.EncryptionRequest:
                    output("Negotiating Encryption...", true);
                    Packet_EncryptionRequest enc = (Packet_EncryptionRequest)e.packet;
                    server = new Server();
                    server.ServerID = enc.ServerID;
                    server.PublicKey = enc.Key;
                    server.PrivateKey = GenerateKey();
                    server.Token = enc.Token;
                    
                    packets.Enqueue(new Packet_EncryptionResponse() {
                        Key=RSAEncrypt(server.PrivateKey,server.PublicKey),
                        Token = RSAEncrypt(server.Token, server.PublicKey)
                    });
                    break;
                case PacketType.EncryptionResponse:
                    output("Encryption ready!", true);
                    //NetStream = new AesStream(NetStream, server.PrivateKey);
                    //NetStream = new blockingStream(new AesStream(client.GetStream(), server.PrivateKey),false);
                    NetStream = new AesStream(client.GetStream(), server.PrivateKey);
                    packets.Enqueue(new Packet_ClientStatus() { Payload = 0 });
                    //packets.Enqueue(new Packet_Login() { Username = name, ProtocolVersion = Protocol, ServerID = server.ServerID, SessionID = sessionid });
                    break;
                case PacketType.Chat:
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

                    string msg = ((Packet_Chat)e.packet).Message;
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
                case PacketType.PlayerPosAndLook:
                    Packet_PlayerPosAndLook p = (Packet_PlayerPosAndLook)e.packet;
                    this.x = p.X;
                    this.y = p.Y;
                    this.z = p.Z;
                    this.stance = p.Stance;
                    this.pitch = p.Pitch;
                    this.yaw = p.Yaw;
                    sendPacket(p);
                    output("Moved!", true);
                    break;
                case PacketType.Kick:
                    output("Kicked: " + ((Packet_Kick)e.packet).Reason, true);
                    break;
            }
        }

        public void registerPacket(PacketType id, Type packet)
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

        public static byte[] GenerateKey()
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.GenerateKey();

            return rijndaelCipher.Key;
        }

        public static byte[] RSAEncrypt(byte[] data, byte[] key)
        {
            AsnKeyParser keyParser = new AsnKeyParser(key);
            RSAParameters publicKey = keyParser.ParseRSAPublicKey();

            CspParameters csp = new CspParameters();

            csp.ProviderType = 1;

            csp.KeyNumber = 1;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
            rsa.PersistKeyInCsp = false;
            rsa.ImportParameters(publicKey);
            byte[] enc = rsa.Encrypt(data, false);
            rsa.Clear();
            return enc;
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
            if(!_str.CanSeek)
                throw new NotSupportedException();
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