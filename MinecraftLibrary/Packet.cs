using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zlib;
using System.Net;
using Craft.Net.Server;

namespace MinecraftLibrary
{
    public enum PacketType
    {
        KeepAlive = 0x00,
        Login = 0x01,
        Handshake = 0x02,
        Chat = 0x03,
        Time = 0x04,
        EntityEquipment = 0x05,
        SpawnPosition = 0x06,
        UseEntity = 0x07,
        UpdateHealth = 0x08,
        Respawn = 0x09,
        Flying = 0x0A,
        PlayerPos = 0x0B,
        PlayerLook = 0x0C,
        PlayerPosAndLook = 0x0D,
        PlayerDigging = 0x0E,
        PlayerBlockPlacement = 0x0F,
        HeldItemChange = 0x10,
        UseBed = 0x11,
        Animation = 0x12,
        EntityAction = 0x13,
        NamedEntitySpawn = 0x14,
        PickupSpawn = 0x15,
        CollectItem = 0x16,
        AddObjVehicle = 0x17,
        MobSpawn = 0x18,
        EntityPainting = 0x19,
        ExpOrb = 0x1A,
        SteerVehicle = 0x1B,
        EntityVel = 0x1C,
        DestroyEntity = 0x1D,
        Entity = 0x1E,
        EntityRelativeMove = 0x1F,
        EntityLook = 0x20,
        EntityLookAndRelativeMove = 0x21,
        EntityTeleport = 0x22,
        EntityHeadLook = 0x23,
        EntityStatus = 0x26,
        AttachEntity = 0x27,
        EntityMetadata = 0x28,
        EntityEffect = 0x29,
        RemoveEntityEffect = 0x2A,
        Experience = 0x2B,
        EntityProperties = 0x2C,
        MapChunk = 0x33,
        MultiBlockChange = 0x34,
        BlockChange = 0x35,
        BlockAction = 0x36,
        BlockBreakAnimation = 0x37,
        MapChunkBulk = 0x38,
        Explosion = 0x3C,
        SoundEffect = 0x3D,
        NamedSoundEffect = 0x3E,
        Particle = 0x3F,
        GameStateChange = 0x46,
        Thunder = 0x47,
        OpenWnd = 0x64,
        CloseWnd = 0x65,
        WndClick = 0x66,
        SetSlot = 0x67,
        WndItems = 0x68,
        UpdateWndProp = 0x69,
        Transaction = 0x6A,
        CreativeInventoryAction = 0x6B,
        EnchantItem = 0x6C,
        UpdateSign = 0x82,
        ItemData = 0x83,
        EntityTileUpdate = 0x84,
        TileEditorOpen = 0x85,
        IncStatistic = 0xC8,
        PlayerListItem = 0xC9,
        PlayerAbilities = 0xCA,
        TabComplete = 0xCB,
        LocaleandViewDistance = 0xCC,
        ClientStatus = 0xCD,
        ScoreboardObjective = 0xCE,
        UpdateScore = 0xCF,
        DisplayScoreboard = 0xD0,
        Teams = 0xD1,
        PluginMessage = 0xFA,
        EncryptionResponse = 0xFC,
        EncryptionRequest = 0xFD,
        ServerListPing = 0xFE,
        Kick = 0xFF
    }
    public enum DigStatus
    {
        StartedDigging = 0,
        CancelDigging = 1,
        FinishedDigging = 2,
        DropItemStack = 3,
        DropItem = 4,
        ShootArrowOrFinishEating = 5
    }
    public enum Face
    {
        Bottom = 0,
        Top = 1,
        East = 2,
        West = 3,
        North = 4,
        South = 5
    }
    public enum Animation
    {
        No_animation = 0,
        SwingArm = 1,
        DamageAnimation = 2,
        LeaveBed = 3,
        EatFood = 5,
        UnknownAnimation = 102,
        Crouch = 104,
        Uncrouch = 105
    }
    public enum MobAction
    {
        Crouch = 1,
        Uncrouch = 2,
        LeaveBed = 3,
        StartSprinting = 4,
        StopSprinting = 5,
    }
    public enum EntityStatus
    {
        EntityHurt = 2,
        EntityDead = 3,
        WolfTaming = 6,
        WolfTamed = 7,
        WolfShakingWaterOffItself = 8,
        EatingAcceptedByServer = 9,
        SheepEatingGrass = 10
    }
    public enum Direction
    {
        SouthEast = 0,
        South = 1,
        SouthWest = 2,
        East = 3,
        UpOrMiddle = 4,
        West = 5,
        NorthEast = 6,
        North = 7,
        NorthWest = 8
    }
    public enum ChangeGameStateReason
    {
        InvalidBed = 0,
        BeginRaining = 1,
        EndRaining = 2,
        ChangeGameMode = 3,
        EnterCredits = 4
    }

    public abstract class Packet
    {
        public abstract void Write();
        public abstract void Read();
        protected byte[] reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
        }
        protected byte[] ReadSTUB(int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            System.Diagnostics.Debug.WriteLine("STUBBED!");
            return tmp;
        }
        public byte[] ReadByteArray(int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            return tmp;
        }
        protected void WriteByteArray(byte[] data, int len)
        {
            str.Write(data, 0, len);
        }
        protected bool ReadBool()
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return BitConverter.ToBoolean(tmp, 0);
        }
        protected void WriteBool(bool data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected byte ReadByte()
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return tmp[0];
        }
        protected void WriteByte(byte data)
        {
            str.WriteByte(data);
        }
        protected SByte ReadSByte()
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return (SByte)tmp[0];
        }
        protected void WriteSByte(SByte data)
        {
            str.WriteByte((byte)data);
        }
        protected string ReadString()
        {
            short len = Short;
            byte[] tmp = new byte[(len * 2)];
            str.Read(tmp, 0, len * 2);
            return UnicodeEncoding.BigEndianUnicode.GetString(tmp, 0, tmp.Length);
        }
        protected void WriteString(string data)
        {
            Short = ((short)data.Length);
            byte[] name = ASCIIEncoding.BigEndianUnicode.GetBytes(data);
            str.Write(name, 0, name.Length);
        }
        protected short ReadShort()
        {
            byte[] tmp = new byte[2];
            str.Read(tmp, 0, 2);
            return BitConverter.ToInt16(reverse(tmp), 0);
        }
        protected void WriteShort(short data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected int ReadInt()
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            tmp = reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        protected void WriteInt(int data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected long ReadLong()
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToInt64(reverse(tmp), 0);
        }
        protected void WriteLong(long data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected float ReadFloat()
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            return BitConverter.ToSingle(reverse(tmp), 0);
        }
        protected void WriteFloat(float data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected double ReadDouble()
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToDouble(reverse(tmp), 0);
        }
        protected void WriteDouble(double data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected void ReadSlotData()
        {
            /* short[] enchantable = {
                0x103, //#Flint and steel
                0x105, //#Bow
                0x15A, //#Fishing rod
                0x167, //#Shears
 
                //#TOOLS
                //#sword, shovel, pickaxe, axe, hoe
                0x10C, 0x10D, 0x10E, 0x10F, //0x122, //#WOOD
                0x110, 0x111, 0x112, 0x113, //0x123, //#STONE
                0x10B, 0x100, 0x101, 0x102, //0x124, //#IRON
                0x114, 0x115, 0x116, 0x117, //0x125, //#DIAMOND
                0x11B, 0x11C, 0x11D, 0x11E, //0x126, //#GOLD
 
                //#ARMOUR
                //#helmet, chestplate, leggings, boots
                0x12A, 0x12B, 0x12C, 0x12D, //#LEATHER
                0x12E, 0x12F, 0x130, 0x131, //#CHAIN
                0x132, 0x133, 0x134, 0x135, //#IRON
                0x136, 0x137, 0x138, 0x139, //#DIAMOND
                0x13A, 0x13B, 0x13C, 0x14D  //#GOLD
            }; 
            short itemID = Short;
            if (itemID != -1)
            {
                sbyte cnt = SByte;
                short Damage = Short;
                //if (enchantable.Contains(itemID))
                //{
                    short tmp = Short;
                    if (tmp != -1)
                    {
                        ReadSTUB( tmp);
                    }
                //}
             }
            */
            Slot slot = new Slot();
            Slot.ReadSlot(str);

        }
        protected void ReadMetaData()
        {
            byte xx;
            xx = Byte;
            var n = 0f;
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        n = SByte;
                        break;
                    case 1:
                        n = Short;
                        break;
                    case 2:
                        n = Int;
                        break;
                    case 3:
                        n = Float;
                        break;
                    case 4:
                        var nn = String;
                        break;
                    case 5:
                        ReadSlotData();
                        break;
                    case 6:
                        n = Int;
                        n = Int;
                        n = Int;
                        break;
                }
                xx = Byte;
            }
        }

        protected Stream str;
        public Stream Stream { get { return str; } set { str = value; } }
        public char Char
        {
            get { return (char)ReadByte(); }
            set { WriteByte((byte)value); }
        }
        public byte Byte
        {
            get { return ReadByte(); }
            set { WriteByte(value); }
        }
        public SByte SByte
        {
            get { return (SByte)Byte; }
            set { Byte = (byte)value; }
        }
        public byte[] Bytes
        {
            set { WriteByteArray(value, value.Length); }
        }
        public short Short
        {
            get { return ReadShort(); }
            set { WriteShort(value); }
        }
        public int Int
        {
            get { return ReadInt(); }
            set { WriteInt(value); }
        }
        public long Long
        {
            get { return ReadLong(); }
            set { WriteLong(value); }
        }
        public bool Bool
        {
            get { return ReadBool(); }
            set { WriteBool(value); }
        }
        public float Float
        {
            get { return ReadFloat(); }
            set { WriteFloat(value); }
        }
        public double Double
        {
            get { return ReadDouble(); }
            set { WriteDouble(value); }
        }
        public string String
        {
            get { return ReadString(); }
            set { WriteString(value); }
        }
    }
    //0x00
    public class Packet_KeepAlive : Packet
    {
        public int ID { get; set; }
        public override void Write()
        {
            Byte = (0x00);
            Int = (ID);
        }
        public override void Read()
        {
            ID = Int;
        }
    }
    //0x01
    public class Packet_Login : Packet
    {
        public string SessionID { get; set; }
        public string ServerID { get; set; }

        public int ProtocolVersion { get; set; }
        public int EID { get; set; }
        public string Username { get; set; }
        public string LevelType { get; set; }
        public int Gamemode { get; set; }
        public int Dimension { get; set; }
        public byte Difficulty { get; set; }
        //Currently not used
        private byte WorldHeight { get; set; }
        public byte MaxPlayers { get; set; }
        public override void Write()
        {
            //TODO: Move this to somewhere else, packets should only parse messages
            if (ServerID != "-")
            {
                WebClient wc = new WebClient();
                string answer = wc.DownloadString(string.Format("http://session.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}", Username, SessionID, ServerID));
                if (answer != "OK") { Console.WriteLine("Answer: " + answer); throw new Exception("invalid answer D:<"); }
            }

            Byte = (0x01);
            Int = (ProtocolVersion);
            String = (Username);
            String = ("");
            Int = (0);
            Int = (0);
            Byte = (0x00);
            Byte = (0x00);
            Byte = (0x00);
        }
        public override void Read()
        {
            EID = Int;
            //Username = String; 
            LevelType = String;
            Gamemode = SByte;
            Dimension = SByte;
            Difficulty = Byte;
            WorldHeight = Byte;
            MaxPlayers = Byte;
        }
    }
    //0x02
    public class Packet_Handshake : Packet
    {
        public byte ProtocolVersion { get; set; }
        public string Username { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public override void Write()
        {
            //string UsernameAndHost = string.Format("{0};{1}", Username, Host);
            Byte = (0x02);
            Byte = (ProtocolVersion);
            String = (Username);
            String = (Host);
            Int = (Port);
        }
        public override void Read()
        {
            //ServerID = String;
        }
    }
    //0x03
    public class Packet_Chat : Packet
    {
        const int MaxPacketSize = 119;
        public string Message { get; set; }
        public override void Write()
        {
            Byte = (0x03);
            if (Message.Length > MaxPacketSize)
                Message = Message.Substring(0, MaxPacketSize);
            String = (Message);
        }
        public override void Read()
        {
            Message = String;
        }
    }
    //0x04
    public class Packet_Time : Packet
    {
        public long Time { get; set; }
        public long TimeofDay { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Time = Long;
            TimeofDay = Long;
        }
    }
    //0x05
    public class Packet_EntityEquipment : Packet
    {
        public int EID { get; set; }
        public short Slot { get; set; }
        public short ItemID { get; set; }
        public short ItemDamage { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            Slot = Short;
            ReadSlotData();
            //ItemID = Short;
            //ItemDamage = Short;
        }
    }
    //0x06
    public class Packet_SpawnPosition : Packet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Y = Int;
            Z = Int;
        }
    }
    //0x07
    public class Packet_UseEntity : Packet
    {
        public int User { get; set; }
        public int Target { get; set; }
        public byte IsLeftClick { get; set; }
        public override void Write()
        {
            Byte = 0x07;
            Int = (User);
            Int = (Target);
            Byte = (IsLeftClick);
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x08
    public class Packet_UpdateHealth : Packet
    {
        public float Health { get; set; }
        public short Food { get; set; }
        public float Saturation { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Health = Float;
            Food = Short;
            Saturation = Float;
        }
    }
    //0x09
    public class Packet_Respawn : Packet
    {
        public int Dimension { get; set; }
        public sbyte Difficulty { get; set; }
        public sbyte Gamemode { get; set; }
        public short WorldHeight { get; set; }
        public string LevelType { get; set; }
        public override void Write()
        {
            Byte = 0x09;
            Int = (Dimension);
            SByte = (Difficulty);
            SByte = (Gamemode);
            Short = (WorldHeight);
            String = (LevelType);
        }
        public override void Read()
        {
            Dimension = Int;
            Difficulty = SByte;
            Gamemode = SByte;
            WorldHeight = Short;
            LevelType = String;
        }
    }
    //The base for 0x0A to 0x0D
    public abstract class Packet_Player : Packet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Stance { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
        public bool Moving { get; set; }
        public bool Rotating { get; set; }
    }
    //0x0A
    public class Packet_Flying : Packet_Player
    {
        public override void Write()
        {
            Byte = 0x0A;
            Bool = (OnGround);
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x0B
    public class Packet_PlayerPos : Packet_Player
    {
        public override void Write()
        {
            Byte = 0x0B;
            Double = (X);
            Double = (Y);
            Double = (Stance);
            Double = (Z);
            Bool = (OnGround);
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x0C
    public class Packet_PlayerLook : Packet_Player
    {
        public override void Write()
        {
            Byte = 0x0C;
            Float = (Yaw);
            Float = (Pitch);
            Bool = (OnGround);
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x0D
    public class Packet_PlayerPosAndLook : Packet_Player
    {
        public override void Write()
        {
            Byte = (0x0D);
            Double = (X);
            Double = (Y);
            Double = (Stance);
            Double = (Z);
            Float = (Yaw);
            Float = (Pitch);
            Bool = (OnGround);
        }

        public override void Read()
        {
            X = Double;
            Stance = Double;
            Y = Double;
            Z = Double;
            Yaw = Float;
            Pitch = Float;
            OnGround = Bool;
        }
    }
    //0x0E
    public class Packet_PlayerDigging : Packet
    {
        public DigStatus DigStatus { get; set; }
        public int X { get; set; }
        public byte Y { get; set; }
        public int Z { get; set; }
        public Face Face { get; set; }

        public override void Write()
        {
            Byte = (0x0E);
            SByte = ((sbyte)DigStatus);
            Int = (X);
            Byte = (Y);
            Int = (Z);
            Byte = ((byte)Face);
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x0F
    public class Packet_PlayerBlockPlacement : Packet
    {
        public int X { get; set; }
        public sbyte Y { get; set; }
        public int Z { get; set; }
        public Face Direction { get; set; }
        public int Held { get; set; }
        public byte curX { get; set; }
        public byte curY { get; set; }
        public byte curZ { get; set; }

        public override void Write()
        {
            Byte = (0x0F);
            Int = (X);
            SByte = (Y);
            Int = (Z);
            SByte = ((sbyte)Direction);
            Int = (Held); //TODO: Fix this! Should be Slot!
            Byte = curX;
            Byte = curY;
            Byte = curZ;
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x10
    public class Packet_HoldingChange : Packet
    {
        public short SlotID { get; set; }
        public override void Write()
        {
            Byte = (0x0F);
            Short = (SlotID);

        }
        public override void Read()
        {
            SlotID = Short;
        }
    }
    //0x11
    public class Packet_UseBed : Packet
    {
        public int EID { get; set; }
        public sbyte InBed { get; set; }
        public int X { get; set; }
        public sbyte Y { get; set; }
        public int Z { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            EID = Int;
            InBed = SByte;
            X = Int;
            Y = SByte;
            Z = Int;
        }
    }
    //0x12
    public class Packet_Animation : Packet
    {
        public int
            EID { get; set; }
        public Animation animation { get; set; }

        public override void Write()
        {
            Int = (EID);
            SByte = ((sbyte)animation);
        }

        public override void Read()
        {
            EID = Int;
            animation = (Animation)SByte;
        }
    }
    //0x13
    public class Packet_EntityAction : Packet
    {
        public int EID { get; set; }
        public MobAction mobAction { get; set; }
        public int JumpBoost { get; set; }

        public override void Write()
        {
            Int = (EID);
            SByte = ((sbyte)mobAction);
            Int = JumpBoost;
        }

        public override void Read()
        {
            EID = Int;
            mobAction = (MobAction)SByte;
            JumpBoost = Int;
        }
    }
    //0x14
    public class Packet_NamedEntitySpawn : Packet
    {
        public int EID { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Byte Rotation { get; set; }
        public Byte Pitch { get; set; }
        public short Item { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            EID = Int;
            Name = String;
            X = Int;
            Y = Int;
            Z = Int;
            Rotation = Byte;
            Pitch = Byte;
            Item = Short;
            byte xx; //TODO: Check Metadata
            xx = Byte;
            var n = 0f;
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        n = SByte;
                        break;
                    case 1:
                        n = Short;
                        break;
                    case 2:
                        n = Int;
                        break;
                    case 3:
                        n = Float;
                        break;
                    case 4:
                        var nn = String;
                        break;
                    case 5:
                        n = Short;
                        n = SByte;
                        n = Short;
                        break;
                    case 6:
                        n = Int;
                        n = Int;
                        n = Int;
                        break;
                }
                xx = Byte;
            }
        }
    }
    //0x15 //TODO: No longer needed
    public class Packet_PickupSpawn : Packet
    {
        public int EID { get; set; }
        public short Item { get; set; }
        public byte Count { get; set; }
        public short Damage { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public byte Rotation { get; set; }
        public byte Pitch { get; set; }
        public byte Roll { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            EID = Int;
            Item = Short;
            Count = Byte;
            Damage = Short;
            X = Int;
            Y = Int;
            Z = Int;
            Rotation = Byte;
            Pitch = Byte;
            Roll = Byte;
        }
    }
    //0x16
    public class Packet_CollectItem : Packet
    {
        public int ItemEID { get; set; }
        public int PlayerEID { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            ItemEID = Int;
            PlayerEID = Int;
        }
    }
    //0x17
    public class Packet_AddObjVehicle : Packet
    {
        public int EID { get; set; }
        public byte type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public sbyte Pitch { get; set; }
        public sbyte Yaw { get; set; }
        public int FbEID { get; set; }
        public short SpeedX { get; set; }
        public short SpeedY { get; set; }
        public short SpeedZ { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            EID = Int;
            type = Byte;
            X = Int;
            Y = Int;
            Z = Int;
            Pitch = SByte;
            Yaw = SByte;
            FbEID = Int;
            if (FbEID > 0)
            {
                SpeedX = Short;
                SpeedY = Short;
                SpeedZ = Short;
            }
        }
    }
    //0x18
    public class Packet_MobSpawn : Packet
    {
        public int EID { get; set; }
        public sbyte type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }
        public byte HeadYaw { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            type = SByte;
            X = Int;
            Y = Int;
            Z = Int;
            Pitch = Byte;
            HeadYaw = Byte;
            Yaw = Byte;
            short t;
            t = Short;
            t = Short;
            t = Short;
            ReadMetaData();
        }
    }
    //0x19
    public class Packet_EntityPainting : Packet
    {
        public int EID { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Dir { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            Name = String;
            X = Int;
            Y = Int;
            Z = Int;
            Dir = Int;
        }
    }
    //0x1A
    public class Packet_ExpOrb : Packet
    {
        public int EID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public short Count { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            X = Int;
            Y = Int;
            Z = Int;
            Count = Short;
        }
    }
    //0x1B
    public class Packet_SteerVehicle : Packet
    {
        public float Sideways { get; set; }
        public float Forward { get; set; }
        public bool Jump { get; set; }
        public bool Unmount { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Sideways = Float;
            Forward = Float;
            Jump = Bool;
            Unmount = Bool;
        }
    }
    //0x1C
    public class Packet_EntityVel : Packet
    {
        public int EID { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            VelocityX = Short;
            VelocityY = Short;
            VelocityZ = Short;
        }
    }
    //0x1D
    public class Packet_DestroyEntity : Packet
    {
        public int[] EIDs { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            byte cnt = Byte;
            EIDs = new int[cnt];
            for (int i = 0; i < cnt; i++)
                EIDs[i] = Int;
        }
    }
    //0x1E
    public class Packet_Entity : Packet
    {
        public int EID { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
        }
    }
    //0x1F
    public class Packet_EntityRelativeMove : Packet
    {
        public int EID { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            X = Byte;
            Y = Byte;
            Z = Byte;
        }
    }
    //0x20
    public class Packet_EntityLook : Packet
    {
        public int EID { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            Yaw = Byte;
            Pitch = Byte;
        }
    }
    //0x21
    public class Packet_EntityLookAndRelativeMove : Packet
    {
        public int EID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            X = Byte;
            Y = Byte;
            Z = Byte;
            Yaw = Byte;
            Pitch = Byte;
        }
    }
    //0x22
    public class Packet_EntityTeleport : Packet
    {
        public int EID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            X = Int;
            Y = Int;
            Z = Int;
            Yaw = Byte;
            Pitch = Byte;
        }
    }
    //0x23
    public class Packet_EntityHeadLook : Packet
    {
        public int EID { get; set; }
        public int HeadYaw { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            HeadYaw = Byte;
        }
    }
    //0x26
    public class Packet_EntityStatus : Packet
    {
        public int EID { get; set; }
        public EntityStatus Status { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            Status = (EntityStatus)Byte; //TODO: EntityStatus Enum
        }
    }
    //0x27
    public class Packet_AttachEntity : Packet
    {
        public int EID { get; set; }
        public int VehicleEID { get; set; }
        public byte Leash { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            VehicleEID = Int;
            Leash = Byte;
        }
    }
    //0x28
    public class Packet_EntityMetadata : Packet
    {
        // TODO: Add more variables (0x28 EntityMetadata)
        public int EID { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            ReadMetaData();
        }
    }
    //0x29
    public class Packet_EntityEffect : Packet
    {
        public int EID { get; set; }
        public byte EffectID { get; set; }
        public byte Amplifier { get; set; }
        public short Duration { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            EffectID = Byte;
            Amplifier = Byte;
            Duration = Short;
        }
    }
    //0x2A
    public class Packet_RemoveEntityEffect : Packet
    {
        public int EID { get; set; }
        public byte EffectID { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            EffectID = Byte;
        }
    }
    //0x2B
    public class Packet_Experience : Packet
    {
        public float XPBar { get; set; }
        public short Level { get; set; }
        public short TotalXP { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            XPBar = Float;
            Level = Short;
            TotalXP = Short;
        }
    }
    //0x2C
    public class Packet_EntityProperties : Packet
    {
        public int EID { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            EID = Int;
            int cnt = Int;
            object tmp;
            for (int i = 0; i < cnt; i++)
            {
                tmp = String;
                tmp = Double;
                short cnt2 = Short;
                for (int ii = 0; ii < cnt2; ii++)
                {
                    tmp = Long;
                    tmp = Long;
                    tmp = Double;
                    tmp = Byte;
                }
            }
        }
    }
    //0x32
    /*  public class Packet_PreChunk : Packet
      {
          public int X { get; set; }
          public int Z { get; set; }
          public bool Mode { get; set; }
          public override void Write()
          {
              throw new NotImplementedException();
          }
          public override void Read()
          {
              X = Int;
              Z = Int;
              Mode = Bool;
          }
      }
      */
    //0x33  //!
    public class Packet_MapChunk : Packet
    {
        public int X { get; set; }
        public int Z { get; set; }
        public bool GroundUC { get; set; }
        public short PrimaryBM { get; set; }
        public short AddBM { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
        private byte[] RawData { get; set; }
        public byte[] ChunkData
        {
            get
            {
                if (RawData == null)
                    RawData = ZlibStream.UncompressBuffer(Data);

                return RawData;
            }
            set
            {
                RawData = value;
            }
        }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            //Some variables missing
            X = Int;
            Z = Int;
            GroundUC = Bool;
            PrimaryBM = Short;
            AddBM = Short;
            Size = Int;
            //var n = Int;
            Data = ReadByteArray(Size); //new byte[Size];
            //str.Read(Data, 0, Size);
        }
    }
    //0x34 //!
    public class Packet_MultiBlockChange : Packet
    {
        public int X { get; set; }
        public int Z { get; set; }
        public short Count { get; set; }
        public int[] Rawdata { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Z = Int;
            int count = Short;
            Rawdata = new int[count];
            int ds = Int;
            for (int i = 0; i < count; i++)
                Rawdata[i] = Int;
        }
    }
    //0x35
    public class Packet_BlockChange : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public short Type { get; set; }
        public byte Metadata { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Y = Byte;
            Z = Int;
            Type = Short;
            Metadata = Byte;
        }
    }
    //0x36
    public class Packet_BlockAction : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public short BlockID { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Y = Short;
            Z = Int;
            Byte1 = Byte;
            Byte2 = Byte;
            BlockID = Short;
        }
    }
    //0x37
    public class Packet_BlockBreakingAnimation : Packet
    {
        public int EID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public sbyte Stage { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            X = Int;
            Y = Int;
            Z = Int;
            Stage = SByte;
        }
    }
    //0x38
    public class Packet_MapChunkBulk : Packet
    {
        public short Count { get; set; }
        public int Length { get; set; }
        public bool SkyLightSent { get; set; }
        public byte Metadata { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Count = Short;
            Length = Int;
            SkyLightSent = Bool;
            //Metadata = Byte;
            ReadByteArray((Length) + (12 * Count));
        }
    }
    //0x3C
    public class Packet_Explosion : Packet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Radius { get; set; }
        public int RecordCount { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }
        
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            // TODO: Add Records (0x3C Explosion)
            X = Double;
            Y = Double;
            Z = Double;
            Radius = Float;
            RecordCount = Int;
            var n = 0;
            for (int i = 0; i < RecordCount; i++)
            {
                n = Byte;
                n = Byte;
                n = Byte;
            }
            PlayerMotionX = Float;
            PlayerMotionY = Float;
            PlayerMotionZ = Float;
        }
    }
    //0x3D
    public class Packet_SoundEffect : Packet
    {
        public int EffectID { get; set; }
        public int X { get; set; }
        public byte Y { get; set; }
        public int Z { get; set; }
        public int Data { get; set; }
        public bool NoVolumeDecrease { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EffectID = Int;
            X = Int;
            Y = Byte;
            Z = Int;
            Data = Int;
            NoVolumeDecrease = Bool; 
        }
    }
    //0x3E
    public class Packet_NamedSoundEffect : Packet
    {
        public string SoundName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public byte Pitch { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            SoundName = String;
            X = Int;
            Y = Int;
            Z = Int;
            Volume = Float;
            Pitch = Byte;
        }
    }
    //0x3F
    public class Packet_Particle : Packet
    {
        public string ParticleName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public float Speed { get; set; }
        public int Count { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            ParticleName = String;
            X = Float;
            Y = Float;
            Z = Float;
            OffsetX = Float;
            OffsetY = Float;
            OffsetZ = Float;
            Speed = Float;
            Count = Int;
        }
    }
    //0x46
    public class Packet_GameStateChange : Packet
    {
        public ChangeGameStateReason Reason { get; set; }
        public byte Gamemode { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Reason = (ChangeGameStateReason)Byte; //TODO: Update Enum
            Gamemode = Byte;
        }
    }
    //0x47
    public class Packet_Thunder : Packet
    {
        public int EID { get; set; }
        public byte isLightningBolt { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            EID = Int;
            isLightningBolt = Byte;
            X = Int;
            Y = Int;
            Z = Int;
        }
    }
    //0x64
    public class Packet_OpenWnd : Packet
    {
        public byte WndID { get; set; }
        public byte InvType { get; set; }
        public string WndTitle { get; set; }
        public byte SlotCount { get; set; }
        public bool UseWndTitle { get; set; }
        public int EID { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            WndID = Byte;
            InvType = Byte;
            WndTitle = String;
            SlotCount = Byte;
            UseWndTitle = Bool;
            EID = Int;
        }
    }
    //0x65
    public class Packet_CloseWnd : Packet
    {
        public byte WndID { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            WndID = Byte;
        }
    }
    //0x66
    public class Packet_WndClick : Packet
    {
        public sbyte WndID { get; set; }
        public short Slot { get; set; }
        public sbyte RightClick { get; set; }
        public short ActionNumber { get; set; }
        public bool IsShiftPressed { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
            SByte = (WndID);
            Short = (Slot);
            SByte = (RightClick);
            Short = (ActionNumber);
            Bool = (IsShiftPressed); //TODO: Implement! THis is different in latest protocol!
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x67
    public class Packet_SetSlot : Packet
    {
        sbyte WndID { get; set; }
        short Slot { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            // TODO: Add slotData variable (0x67 SetSlot)
            WndID = SByte;
            Slot = Short;
            ReadSlotData();
        }
    }
    //0x68
    public class Packet_WndItems : Packet
    {
        byte WndID { get; set; }
        short Count { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            // TODO: Add slotdata array (0x68 WndItems)
            WndID = Byte;
            Count = Short;
            for (int i = 0; i < Count; i++)
            {
                ReadSlotData();
            }
        }
    }
    //0x69
    public class Packet_UpdateWndProp : Packet
    {
        byte WndID { get; set; }
        short Property { get; set; }
        short Value { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            WndID = Byte;
            Property = Short;
            Value = Short;
        }
    }
    //0x6A
    public class Packet_Transaction : Packet
    {
        public sbyte WndID { get; set; }
        public short ActionNumber { get; set; }
        public bool Accepted { get; set; }
        public override void Write()
        {
            SByte = (WndID);
            Short = (ActionNumber);
            Bool = (Accepted);
        }
        public override void Read()
        {
            WndID = SByte;
            ActionNumber = Short;
            Accepted = Bool;
        }
    }
    //0x6B
    public class Packet_CreativeInventoryAction : Packet
    {
        public short Slot { get; set; }
        //TODO: Implement Write 0x6B CreativeInventoryAction (Needs Slot data object to be implemented first)
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Slot = Short;
            ReadSlotData();
        }
    }
    //0x6C
    public class Packet_EnchantItem : Packet
    {
        public sbyte WndID { get; set; }
        public sbyte Enchantment { get; set; }
        public override void Write()
        {
            SByte = (WndID);
            SByte = (Enchantment);
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0x82
    public class Packet_UpdateSign : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Y = Short;
            Z = Int;
            Text1 = String;
            Text2 = String;
            Text3 = String;
            Text4 = String;
        }
    }
    //0x83
    public class Packet_ItemData : Packet
    {
        public short Type { get; set; }
        public short ID { get; set; }
        public short TextLength { get; set; }
        public byte[] Text { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Type = Short;
            ID = Short;
            TextLength = Short;
            Text = ReadByteArray(TextLength);
        }
    }
    //0x84
    public class Packet_EntityTileUpdate : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public byte Action { get; set; }
        public int DataLength { get { return NBTData.Length; } set { NBTData = new byte[value]; } }
        public byte[] NBTData { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            X = Int;
            Y = Short;
            Z = Int;
            Action = Byte;
            DataLength = Short;
            NBTData = ReadByteArray(DataLength);
        }
    }
    //0x85
    public class Packet_TileEditorOpen : Packet
    {
        public sbyte TileEID { get; set; }
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            TileEID = SByte;
            X = Int;
            Y = Short;
            Z = Int;
        }
    }
    //0xC8
    public class Packet_IncStatistic : Packet
    {
        public int StatisticID { get; set; }
        public int Amount { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            StatisticID = Int;
            Amount = Int;
        }
    }
    //0xC9
    public class Packet_PlayerListItem : Packet
    {
        public string Name { get; set; }
        public bool Online { get; set; }
        public short Ping { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Name = String;
            Online = Bool;
            Ping = Short;
        }
    }
    //0xCA
    public class Packet_PlayerAbilities : Packet
    {
        public byte Flags { get; set; }
        public float FlySpeed { get; set; }
        public float WalkSpeed { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Flags = Byte;
            FlySpeed = Float;
            WalkSpeed = Float;
        }
    }
    //0xCB
    public class Packet_TabComplete : Packet
    {
        public string Text { get; set; }
        public override void Write()
        {
            String = (Text);
        }
        public override void Read()
        {
            Text = String;
        }
    }
    //0xCC
    public class Packet_LocaleandViewDistance : Packet
    {
        public string Locale { get; set; }
        public byte ViewDistance { get; set; }
        public byte ChatFlags { get; set; }
        public byte Difficulty { get; set; }
        public bool ShowCape { get; set; }
        public override void Write()
        {
            String = Locale;
            Byte = ViewDistance;
            Byte = ChatFlags;
            Byte = Difficulty;
            Bool = ShowCape;
        }
        public override void Read()
        {
           throw new NotImplementedException(); 
        }
    }
    //0xCD
    public class Packet_ClientStatus : Packet
    {
        public byte Payload { get; set; }
        public override void Write()
        {
            Byte = (0xCD);
            Byte = (Payload);
        }
        public override void Read()
        {
            Payload = Byte;
        }
    }
    //TODO: Scoreboards! (0xCF-0xD1)
    //0xFA
    public class Packet_PluginMessage : Packet
    {
        string Channel { get; set; }
        short Length { get; set; }
        byte[] Data { get; set; }
        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            Channel = String;
            Length = Short;
            Data = ReadByteArray(Length);
        }
    }
    //0xFC
    public class Packet_EncryptionResponse : Packet
    {
        public short KeyLength { get { return (short)Key.Length; } set { Key = new byte[value]; } }
        public byte[] Key { get; set; }
        public short TokenLength { get { return (short)Token.Length; } set { Token = new byte[value]; } }
        public byte[] Token { get; set; }

        public override void Write()
        {
            Byte = (0xFC);
            WriteShort(KeyLength);
            WriteByteArray(Key, KeyLength);
            Short = (TokenLength);
            WriteByteArray(Token, TokenLength);
        }
        public override void Read()
        {
            KeyLength = Short;
            Key = ReadByteArray(KeyLength);
            TokenLength = Short;
            Token = ReadByteArray(TokenLength);
        }
    }
    //0xFD
    public class Packet_EncryptionRequest : Packet
    {
        public string ServerID { get; set; }
        public short KeyLength { get; set; }
        public byte[] Key { get; set; }
        public short TokenLength { get; set; }
        public byte[] Token { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }
        public override void Read()
        {
            ServerID = String;
            KeyLength = Short;
            Key = ReadByteArray(KeyLength);
            TokenLength = Short;
            Token = ReadByteArray(TokenLength);
        }
    }
    // TODO: Implement 0xFE Server List Ping (Basically get server info)
    //0xFE
    public class Packet_ServerListPing : Packet
    {
        public byte Magic { get; set; }

        public override void Write()
        {
            Magic = 1;
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
    //0xFF
    public class Packet_Kick : Packet
    {
        public string Reason { get; set; }

        public override void Write()
        {
            String = (Reason);
        }
        public override void Read()
        {
            Reason = String;
        }
    }
}

