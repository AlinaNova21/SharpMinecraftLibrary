using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zlib;
using System.Net;

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
        PreChunk = 0x32,
        MapChunk = 0x33,
        MultiBlockChange = 0x34,
        BlockChange = 0x35,
        BlockAction = 0x36,
        Explosion = 0x3C,
        SoundEffect = 0x3D,
        NewOrInvalidState = 0x46,
        Thunder = 0x47,
        OpenWnd = 0x64,
        CloseWnd = 0x65,
        WndClick = 0x66,
        SetSlot = 0x67,
        WndItems = 0x68,
        UpdateWndProp = 0x69,
        Transaction = 0x6A,
        CreativeInventoryAction = 0x6B,
        UpdateSign = 0x82,
        ItemData = 0x83,
        EntityTileUpdate = 0x84,
        IncStatistic = 0xC8,
        PlayerListItem = 0xC9,
        PlayerAbilities = 0xCA,
        PluginMessage = 0xFA,
        ServerListPing = 0xFE,
        Kick = 0xFF
    }

    public enum DigStatus
    {
        StartedDigging = 0,
        FinishedDigging = 2,
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
        public abstract void Write(Stream str);
        public abstract void Read(Stream str);
        protected byte[] reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
        }
        protected byte[] ReadSTUB(Stream str, int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            System.Diagnostics.Debug.WriteLine("STUBBED!");
            return tmp;
        }
        protected byte[] ReadByteArray(Stream str, int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            return tmp;
        }
        protected bool ReadBool(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return BitConverter.ToBoolean(tmp, 0);
        }
        protected void WriteBool(Stream str, bool data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected byte ReadByte(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return tmp[0];
        }
        protected void WriteByte(Stream str, byte data)
        {
            str.WriteByte(data);
        }
        protected SByte ReadSByte(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return (SByte)tmp[0];
        }
        protected void WriteSByte(Stream str, SByte data)
        {
            str.WriteByte((byte)data);
        }
        protected string ReadString(Stream str)
        {
            short len = ReadShort(str);
            byte[] tmp = new byte[(len * 2)];
            str.Read(tmp, 0, len * 2);
            return UnicodeEncoding.BigEndianUnicode.GetString(tmp, 0, tmp.Length);
        }
        protected void WriteString(Stream str, string data)
        {
            WriteShort(str, (short)data.Length);
            byte[]name = ASCIIEncoding.BigEndianUnicode.GetBytes(data);
            str.Write(name, 0, name.Length);
        }
        protected short ReadShort(Stream str)
        {
            byte[] tmp = new byte[2];
            str.Read(tmp, 0, 2);
            return BitConverter.ToInt16(reverse(tmp), 0);
        }
        protected void WriteShort(Stream str, short data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected int ReadInt(Stream str)
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            tmp = reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        protected void WriteInt(Stream str, int data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected long ReadLong(Stream str)
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToInt64(reverse(tmp), 0);
        }
        protected void WriteLong(Stream str, long data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected float ReadFloat(Stream str)
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            return BitConverter.ToSingle(reverse(tmp), 0);
        }
        protected void WriteFloat(Stream str, float data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected double ReadDouble(Stream str)
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToDouble(reverse(tmp), 0);
        }
        protected void WriteDouble(Stream str, double data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected void ReadSlotData(Stream str)
        {
            short[] enchantable = {
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
            short itemID = ReadShort(str);
            if (itemID != -1)
            {
                sbyte cnt = ReadSByte(str);
                short Damage = ReadShort(str);
                if (enchantable.Contains(itemID))
                {
                    short tmp = ReadShort(str);
                    if (tmp != -1)
                    {
                        ReadSTUB(str, tmp);
                    }
                }
            }
        }
    }
    //0x00
    public class Packet_KeepAlive : Packet
    {
        public int ID { get; set; }
        public override void Write(Stream str)
        {
            WriteByte(str, 0x00);
            WriteInt(str, ID);
        }
        public override void Read(Stream str)
        {
            ID = ReadInt(str);
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
        public override void Write(Stream str)
        {
            //TODO: Move this to somewhere else, packets should only parse messages
            if (ServerID != "-")
            {
                WebClient wc = new WebClient();
                string answer = wc.DownloadString(string.Format("http://session.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}", Username, SessionID, ServerID));
                if (answer != "OK") { Console.WriteLine("Answer: " + answer); throw new Exception("invalid answer D:<"); }
            }

            WriteByte(str, 0x01);
            WriteInt(str, ProtocolVersion);
            WriteString(str, Username);
            WriteString(str, "");
            WriteInt(str, 0);
            WriteInt(str, 0);
            WriteByte(str, 0x00);
            WriteByte(str, 0x00);
            WriteByte(str, 0x00);
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Username = ReadString(str); 
            LevelType = ReadString(str);
            Gamemode = ReadInt(str);
            Dimension = ReadInt(str);
            Difficulty = ReadByte(str);
            WorldHeight = ReadByte(str);
            MaxPlayers = ReadByte(str);
        }
    }
    //0x02
    public class Packet_Handshake : Packet
    {
        public string Username { get; set; }
        public string Host { get; set; }
        public string ServerID { get; set; }
        public override void Write(Stream str)
        {
            string UsernameAndHost = string.Format("{0};{1}", Username, Host);
            WriteByte(str, 0x02);
            WriteString(str, Username);
        }
        public override void Read(Stream str)
        {
            ServerID = ReadString(str);
        }
    }
    //0x03
    public class Packet_Chat : Packet
    {
        const int MaxPacketSize = 119;
        public string Message { get; set; }
        public override void Write(Stream str)
        {
            WriteByte(str, 0x03);
            if (Message.Length > MaxPacketSize)
                Message = Message.Substring(0, MaxPacketSize);
            WriteString(str, Message);
        }
        public override void Read(Stream str)
        {
            Message = ReadString(str);
        }
    }
    //0x04
    public class Packet_Time : Packet
    {
        public long Time { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Time = ReadLong(str);
        }
    }
    //0x05
    public class Packet_EntityEquipment : Packet
    {
        public int EID { get; set; }
        public short Slot { get; set; }
        public short ItemID { get; set; }
        public short ItemDamage { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Slot = ReadShort(str);
            ItemID = ReadShort(str);
            ItemDamage = ReadShort(str);
        }
    }
    //0x06
    public class Packet_SpawnPosition : Packet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
        }
    }
    //0x07
    public class Packet_UseEntity : Packet
    {
        public int User { get; set; }
        public int Target { get; set; }
        public byte IsLeftClick { get; set; }
        public override void Write(Stream str)
        {
            WriteByte(str, 0x07);
            WriteInt(str, User);
            WriteInt(str, Target);
            WriteByte(str, IsLeftClick);
        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x08
    public class Packet_UpdateHealth : Packet
    {
        public short Health { get; set; }
        public short Food { get; set; }
        public float Saturation { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Health = ReadShort(str);
            Food = ReadShort(str);
            Saturation = ReadFloat(str);
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
        public override void Write(Stream str)
        {
            WriteByte(str, 0x09);
            WriteInt(str, Dimension);
            WriteSByte(str, Difficulty);
            WriteSByte(str, Gamemode);
            WriteShort(str, WorldHeight);
            WriteString(str, LevelType);
        }
        public override void Read(Stream str)
        {
            Dimension = ReadInt(str);
            Difficulty = ReadSByte(str);
            Gamemode = ReadSByte(str);
            WorldHeight = ReadShort(str);
            LevelType = ReadString(str);
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
        public override void Write(Stream str)
        {
            WriteByte(str, 0x0A);
            WriteBool(str, OnGround);
        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0B
    public class Packet_PlayerPos : Packet_Player
    {
        public override void Write(Stream str)
        {
            WriteByte(str, 0x0B);
            WriteDouble(str, X);
            WriteDouble(str, Y);
            WriteDouble(str, Stance);
            WriteDouble(str, Z);
            WriteBool(str, OnGround);
        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0C
    public class Packet_PlayerLook : Packet_Player
    {
        public override void Write(Stream str)
        {
            WriteByte(str, 0x0C);
            WriteFloat(str, Yaw);
            WriteFloat(str, Pitch);
            WriteBool(str, OnGround);
        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0D
    public class Packet_PlayerPosAndLook : Packet_Player
    {
        public override void Write(Stream str)
        {
            WriteByte(str, 0x0D);
            WriteDouble(str, X);
            WriteDouble(str, Y);
            WriteDouble(str, Stance);
            WriteDouble(str, Z);
            WriteFloat(str, Yaw);
            WriteFloat(str, Pitch);
            WriteBool(str, OnGround);
        }

        public override void Read(Stream str)
        {
            X = ReadDouble(str);
            Stance = ReadDouble(str);
            Y = ReadDouble(str);
            Z = ReadDouble(str);
            Yaw = ReadFloat(str);
            Pitch = ReadFloat(str);
            OnGround = ReadBool(str);
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

        public override void Write(Stream str)
        {
            WriteByte(str, 0x0E);
            WriteSByte(str, (sbyte)DigStatus);
            WriteInt(str, X);
            WriteByte(str, Y);
            WriteInt(str, Z);
            WriteByte(str, (byte)Face);
        }

        public override void Read(Stream str)
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

        public override void Write(Stream str)
        {
            WriteByte(str, 0x0F);
            WriteInt(str, X);
            WriteSByte(str, Y);
            WriteInt(str, Z);
            WriteSByte(str, (sbyte)Direction);
            WriteInt(str, Held);
        }

        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x10
    public class Packet_HoldingChange : Packet
    {
        public short SlotID { get; set; }
        public override void Write(Stream str)
        {
            WriteByte(str, 0x0F);
            WriteShort(str, SlotID);

        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
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

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            InBed = ReadSByte(str);
            X = ReadInt(str);
            Y = ReadSByte(str);
            Z = ReadInt(str);
        }
    }
    //0x12
    public class Packet_Animation : Packet
    {
        public int 
            EID { get; set; }
        public Animation animation { get; set; }

        public override void Write(Stream str)
        {
            WriteInt(str, EID);
            WriteSByte(str, (sbyte)animation);
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            animation = (Animation)ReadSByte(str);
        }
    }
    //0x13
    public class Packet_EntityAction : Packet
    {
        public int EID { get; set; }
        public MobAction mobAction { get; set; }

        public override void Write(Stream str)
        {
            WriteInt(str, EID);
            WriteSByte(str, (sbyte)mobAction);
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            mobAction = (MobAction)ReadSByte(str);
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

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Name = ReadString(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Rotation = ReadByte(str);
            Pitch = ReadByte(str);
            Item = ReadShort(str);
        }
    }
    //0x15
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

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Item = ReadShort(str);
            Count = ReadByte(str);
            Damage = ReadShort(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Rotation = ReadByte(str);
            Pitch = ReadByte(str);
            Roll = ReadByte(str);
        }
    }
    //0x16
    public class Packet_CollectItem : Packet
    {
        public int ItemEID { get; set; }
        public int PlayerEID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void Read(Stream str)
        {
            ItemEID = ReadInt(str);
            PlayerEID = ReadInt(str);
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
        public int FbEID { get; set; }
        public short SpeedX { get; set; }
        public short SpeedY { get; set; }
        public short SpeedZ { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            type = ReadByte(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            FbEID = ReadInt(str);
            if (FbEID > 0)
            {
                SpeedX = ReadShort(str);
                SpeedY = ReadShort(str);
                SpeedZ = ReadShort(str);
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

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            type = ReadSByte(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Yaw = ReadByte(str);
            Pitch = ReadByte(str);
            HeadYaw = ReadByte(str);
            byte xx;
            xx = ReadByte(str);
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        ReadSByte(str);
                        break;
                    case 1:
                        ReadShort(str);
                        break;
                    case 2:
                        ReadInt(str);
                        break;
                    case 3:
                        ReadFloat(str);
                        break;
                    case 4:
                        ReadString(str);
                        break;
                    case 5:
                        ReadShort(str);
                        ReadSByte(str);
                        ReadShort(str);
                        break;
                    case 6:
                        ReadInt(str);
                        ReadInt(str);
                        ReadInt(str);
                        break;
                }
                xx = ReadByte(str);
            }
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Name = ReadString(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Dir = ReadInt(str);
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Count = ReadShort(str);
        }
    }
    //0x1C
    public class Packet_EntityVel : Packet
    {
        public int EID { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            VelocityX = ReadShort(str);
            VelocityY = ReadShort(str);
            VelocityZ = ReadShort(str);
        }
    }
    //0x1D
    public class Packet_DestroyEntity : Packet
    {
        public int EID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
        }
    }
    //0x1E
    public class Packet_Entity : Packet
    {
        public int EID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
        }
    }
    //0x1F
    public class Packet_EntityRelativeMove : Packet
    {
        public int EID { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            X = ReadByte(str);
            Y = ReadByte(str);
            Z = ReadByte(str);
        }
    }
    //0x20
    public class Packet_EntityLook : Packet
    {
        public int EID { get; set; }
        public byte Yaw { get; set; }
        public byte Pitch { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Yaw = ReadByte(str);
            Pitch = ReadByte(str);
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            X = ReadByte(str);
            Y = ReadByte(str);
            Z = ReadByte(str);
            Yaw = ReadByte(str);
            Pitch = ReadByte(str);
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
            Yaw = ReadByte(str);
            Pitch = ReadByte(str);
        }
    }
    //0x23
    public class Packet_EntityHeadLook : Packet
    {
        public int EID { get; set; }
        public int HeadYaw { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            HeadYaw = ReadByte(str);
        }
    }
    //0x26
    public class Packet_EntityStatus : Packet
    {
        public int EID { get; set; }
        public EntityStatus Status { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            Status = (EntityStatus)ReadByte(str);
        }
    }
    //0x27
    public class Packet_AttachEntity : Packet
    {
        public int EID { get; set; }
        public int VehicleEID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            VehicleEID = ReadInt(str);
        }
    }
    //0x28
    public class Packet_EntityMetadata : Packet
    {
        // TODO: Add more variables (0x28 EntityMetadata)
        public int EID { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            byte xx;
            xx = ReadByte(str);
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        ReadSByte(str);
                        break;
                    case 1:
                        ReadShort(str);
                        break;
                    case 2:
                        ReadInt(str);
                        break;
                    case 3:
                        ReadFloat(str);
                        break;
                    case 4:
                        ReadString(str);
                        break;
                    case 5:
                        ReadShort(str);
                        ReadSByte(str);
                        ReadShort(str);
                        break;
                    case 6:
                        ReadInt(str);
                        ReadInt(str);
                        ReadInt(str);
                        break;
                }
                xx = ReadByte(str);
            }
        }
    }
    //0x29
    public class Packet_EntityEffect : Packet
    {
        public int EID { get; set; }
        public byte EffectID { get; set; }
        public byte Amplifier { get; set; }
        public short Duration { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            EffectID = ReadByte(str);
            Amplifier = ReadByte(str);
            Duration = ReadShort(str);
        }
    }
    //0x2A
    public class Packet_RemoveEntityEffect : Packet
    {
        public int EID { get; set; }
        public byte EffectID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            EffectID = ReadByte(str);
        }
    }
    //0x2B
    public class Packet_Experience : Packet
    {
        public float XPBar { get; set; }
        public short Level { get; set; }
        public short TotalXP { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            XPBar = ReadFloat(str);
            Level = ReadShort(str);
            TotalXP = ReadShort(str);
        }
    }
    //0x32
    public class Packet_PreChunk : Packet
    {
        public int X { get; set; }
        public int Z { get; set; }
        public bool Mode { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Z = ReadInt(str);
            Mode = ReadBool(str);
        }
    }
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            //Some variables missing
            X = ReadInt(str);
            Z = ReadInt(str);
            GroundUC = ReadBool(str);
            PrimaryBM = ReadShort(str);
            AddBM = ReadShort(str);
            Size = ReadInt(str);
            ReadInt(str);
            Data = new byte[Size];
            str.Read(Data, 0, Size);
        }
    }
    //0x34 //!
    public class Packet_MultiBlockChange : Packet
    {
        public int X { get; set; }
        public int Z { get; set; }
        public short Count { get; set; }
        public int[] Rawdata { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Z = ReadInt(str);
            int count = ReadShort(str);
            Rawdata = new int[count];
            int ds = ReadInt(str);
            for (int i = 0; i < count; i++)
                Rawdata[i] = ReadInt(str);
        }
    }
    //0x35
    public class Packet_BlockChange : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public byte Type { get; set; }
        public byte Metadata { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Y = ReadByte(str);
            Z = ReadInt(str);
            Type = ReadByte(str);
            Metadata = ReadByte(str);
        }
    }
    //0x36?
    public class Packet_BlockAction : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Y = ReadShort(str);
            Z = ReadInt(str);
            Byte1 = ReadByte(str);
            Byte2 = ReadByte(str);
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            // TODO: Add Records (0x3C Explosion)
            X = ReadDouble(str);
            Y = ReadDouble(str);
            Z = ReadDouble(str);
            Radius = ReadFloat(str);
            RecordCount = ReadInt(str);
            for (int i = 0; i < RecordCount; i++)
            {
                ReadByte(str);
                ReadByte(str);
                ReadByte(str);
            }
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EffectID = ReadInt(str);
            X = ReadInt(str);
            Y = ReadByte(str);
            Z = ReadInt(str);
            Data = ReadInt(str);
        }
    }
    //0x3E?
    public class Packet_NewOrInvalidState : Packet
    {
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            // TODO: Add variables (0x3E? NewOrInvalidState)
            ReadSByte(str);
            ReadSByte(str);
        }
    }
    //0x46
    public class Packet_GameStateChange : Packet
    {
        public ChangeGameStateReason Reason { get; set; }
        public byte Gamemode { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Reason = (ChangeGameStateReason)ReadByte(str);
            Gamemode = ReadByte(str);
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
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            EID = ReadInt(str);
            isLightningBolt = ReadByte(str); 
            X = ReadInt(str);
            Y = ReadInt(str);
            Z = ReadInt(str);
        }
    }
    //0x64
    public class Packet_OpenWnd : Packet
    {
        public byte WndID { get; set; }
        public byte InvType { get; set; }
        public string WndTitle { get; set; }
        public byte SlotCount { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            WndID = ReadByte(str);
            InvType = ReadByte(str);
            WndTitle = ReadString(str);
            SlotCount = ReadByte(str);
        }
    }
    //0x65
    public class Packet_CloseWnd : Packet
    {
        public byte WndID { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            WndID = ReadByte(str);
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
        public override void Write(Stream str)
        {
            WriteSByte(str, WndID);
            WriteShort(str, Slot);
            WriteSByte(str, RightClick);
            WriteShort(str, ActionNumber);
            WriteBool(str, IsShiftPressed);
        }
        public override void Read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x67
    public class Packet_SetSlot : Packet
    {
        sbyte WndID { get; set; }
        short Slot { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            // TODO: Add slotData variable (0x67 SetSlot)
            WndID = ReadSByte(str);
            Slot = ReadShort(str);
            ReadSlotData(str);
        }
    }
    //0x68
    public class Packet_WndItems : Packet
    {
        byte WndID { get; set; }
        short Count { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            // TODO: Add slotdata array (0x68 WndItems)
            WndID = ReadByte(str);
            Count = ReadShort(str);
            for (int i = 0; i < Count; i++)
            {
                ReadSlotData(str);
            }
        }
    }
    //0x69
    public class Packet_UpdateWndProp : Packet
    {
        byte WndID { get; set; }
        short Property { get; set; }
        short Value { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            WndID = ReadByte(str);
            Property = ReadShort(str);
            Value = ReadShort(str);
        }
    }
    //0x6A
    public class Packet_Transaction : Packet
    {
        public sbyte WndID { get; set; }
        public short ActionNumber { get; set; }
        public bool Accepted { get; set; }
        public override void Write(Stream str)
        {
            WriteSByte(str, WndID);
            WriteShort(str, ActionNumber);
            WriteBool(str, Accepted);
        }
        public override void Read(Stream str)
        {
            WndID = ReadSByte(str);
            ActionNumber = ReadShort(str);
            Accepted = ReadBool(str);
        }
    }
    //0x6B
    public class Packet_CreativeInventoryAction : Packet
    {
        public short Slot { get; set; }
        //TODO: Implement Write 0x6B CreativeInventoryAction (Needs Slot data object to be implemented first)
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Slot = ReadShort(str);
            ReadSlotData(str);
        }
    }
    //0x6C
    public class Packet_EnchantItem : Packet
    {
        public sbyte WndID { get; set; }
        public sbyte Enchantment { get; set; }
        public override void Write(Stream str)
        {
            WriteSByte(str, WndID);
            WriteSByte(str, Enchantment);
        }
        public override void Read(Stream str)
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

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Y = ReadShort(str);
            Z = ReadInt(str);
            Text1 = ReadString(str);
            Text2 = ReadString(str);
            Text3 = ReadString(str);
            Text4 = ReadString(str);
        }
    }
    //0x83
    public class Packet_ItemData : Packet
    {
        public short Type { get; set; }
        public short ID { get; set; }
        public byte TextLength { get; set; }
        public byte[] Text { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Type = ReadShort(str);
            ID = ReadShort(str);
            TextLength = ReadByte(str);
            Text = ReadByteArray(str, TextLength);
        }
    }
    //0x84
    public class Packet_EntityTileUpdate : Packet
    {
        public int X { get; set; }
        public short Y { get; set; }
        public int Z { get; set; }
        public byte Action { get; set; }
        public int Custom1 { get; set; }
        public int Custom2 { get; set; }
        public int Custom3 { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            X = ReadInt(str);
            Y = ReadShort(str);
            Z = ReadInt(str);
            Action = ReadByte(str);
            Custom1 = ReadInt(str);
            Custom2 = ReadInt(str);
            Custom3 = ReadInt(str);
        }
    }
    //0xC8
    public class Packet_IncStatistic : Packet
    {
        public int StatisticID { get; set; }
        public byte Amount { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            StatisticID = ReadInt(str);
            Amount = ReadByte(str);
        }
    }
    //0xC9
    public class Packet_PlayerListItem : Packet
    {
        public string Name { get; set; }
        public bool Online { get; set; }
        public short Ping { get; set; }

        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Name = ReadString(str);
            Online = ReadBool(str);
            Ping = ReadShort(str);
        }
    }
    //0xCA
    public class Packet_PlayerAbilities : Packet
    {
        public bool Invulnerability { get; set; }
        public bool IsFlying { get; set; }
        public bool CanFly { get; set; }
        public bool InstantDestroy { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Invulnerability = ReadBool(str);
            IsFlying = ReadBool(str);
            CanFly = ReadBool(str);
            InstantDestroy = ReadBool(str);
        }
    }
    //0xFA
    public class Packet_PluginMessage : Packet
    {
        string Channel { get; set; }
        short Length { get; set; }
        byte[] Data { get; set; }
        public override void Write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void Read(Stream str)
        {
            Channel = ReadString(str);
            Length = ReadShort(str);
            Data = ReadByteArray(str, Length);
        }
    }
    // TODO: Implement 0xFE Server List Ping (Basically get server info)
    //0xFF
    public class Packet_Kick : Packet
    {
        public string Reason { get; set; }

        public override void Write(Stream str)
        {
            WriteString(str, Reason);
        }
        public override void Read(Stream str)
        {
            Reason = ReadString(str);
        }
    }
}

