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
        Player = 0x0A,
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
        public abstract void write(Stream str);
        public abstract void read(Stream str);
        protected byte[] reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
        }
        protected byte[] readSTUB(Stream str, int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            System.Diagnostics.Debug.WriteLine("STUBBED!");
            return tmp;
        }
        protected byte[] readByteArray(Stream str, int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            return tmp;
        }
        protected bool readBool(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return BitConverter.ToBoolean(tmp, 0);
        }
        protected void writeBool(Stream str, bool data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected byte readByte(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return tmp[0];
        }
        protected void writeByte(Stream str, byte data)
        {
            str.WriteByte(data);
        }
        protected SByte readSByte(Stream str)
        {
            byte[] tmp = new byte[1];
            str.Read(tmp, 0, 1);
            return (SByte)tmp[0];
        }
        protected void writeSByte(Stream str, SByte data)
        {
            str.WriteByte((byte)data);
        }
        protected string readString(Stream str)
        {
            short len = readShort(str);
            byte[] tmp = new byte[(len * 2)];
            str.Read(tmp, 0, len * 2);
            return UnicodeEncoding.BigEndianUnicode.GetString(tmp, 0, tmp.Length);
        }
        protected void writeString(Stream str, string data)
        {
            writeShort(str, (short)data.Length);
            byte[] name = ASCIIEncoding.BigEndianUnicode.GetBytes(data);
            str.Write(name, 0, name.Length);
        }
        protected short readShort(Stream str)
        {
            byte[] tmp = new byte[2];
            str.Read(tmp, 0, 2);
            return BitConverter.ToInt16(reverse(tmp), 0);
        }
        protected void writeShort(Stream str, short data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected int readInt(Stream str)
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            tmp = reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        protected void writeInt(Stream str, int data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected long readLong(Stream str)
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToInt64(reverse(tmp), 0);
        }
        protected void writeLong(Stream str, long data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected float readFloat(Stream str)
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            return BitConverter.ToSingle(reverse(tmp), 0);
        }
        protected void writeFloat(Stream str, float data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected double readDouble(Stream str)
        {
            byte[] tmp = new byte[8];
            str.Read(tmp, 0, 8);
            return BitConverter.ToDouble(reverse(tmp), 0);
        }
        protected void writeDouble(Stream str, double data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected void readSlotData(Stream str)
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
            short itemID = readShort(str);
            if (itemID != -1)
            {
                sbyte cnt = readSByte(str);
                short damage = readShort(str);
                if (enchantable.Contains(itemID))
                {
                    short tmp = readShort(str);
                    if (tmp != -1)
                    {
                        readSTUB(str, tmp);
                    }
                }
            }
        }
    }
    //0x00
    public class Packet_KeepAlive : Packet
    {
        public int ID { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x00);
            writeInt(str, ID);
        }
        public override void read(Stream str)
        {
            ID = readInt(str);
        }
    }
    //0x01
    public class Packet_Login : Packet
    {
        public int protocol { get; set; }
        public string username { get; set; }
        public string sessionid { get; set; }
        public string serverid { get; set; }

        public int eID { get; set; }
        public string levelType { get; set; }
        public int mode { get; set; }
        public int dimension;
        public SByte difficulty { get; set; }
        //Currently not used
        private byte worldHeight { get; set; }
        public byte maxPlayers { get; set; }
        public override void write(Stream str)
        {
            //TODO: Move this to somewhere else, packets should only parse messages
            if (serverid != "-")
            {
                WebClient wc = new WebClient();
                string answer = wc.DownloadString(string.Format("http://session.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}", username, sessionid, serverid));
                if (answer != "OK") { Console.WriteLine("Answer: " + answer); throw new Exception("invalid answer D:<"); }
            }

            writeByte(str, 0x01);
            writeInt(str, protocol);
            writeString(str, username);
            writeString(str, "");
            writeInt(str, 0);
            writeInt(str, 0);
            writeSByte(str, 0x00);
            writeByte(str, 0x00);
            writeByte(str, 0x00);
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            username = readString(str); //Undocumented, but this is what the client does 
            levelType = readString(str);
            mode = readInt(str);
            dimension = readInt(str);
            difficulty = readSByte(str);
            worldHeight = readByte(str);
            maxPlayers = readByte(str);
        }
    }
    //0x02
    public class Packet_Handshake : Packet
    {
        public string dataString { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x02);
            writeString(str, dataString);
        }
        public override void read(Stream str)
        {
            dataString = readString(str);
        }
    }
    //0x03
    public class Packet_Chat : Packet
    {
        public string dataString { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x03);
            writeString(str, dataString);
        }
        public override void read(Stream str)
        {
            dataString = readString(str);
        }
    }
    //0x04
    public class Packet_Time : Packet
    {
        public long time { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            time = readLong(str);
        }
    }
    //0x05
    public class Packet_EntityEquipment : Packet
    {
        public int eID { get; set; }
        public short slot { get; set; }
        public short itemID { get; set; }
        public short damage { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            slot = readShort(str);
            itemID = readShort(str);
            damage = readShort(str);
        }
    }
    //0x06
    public class Packet_SpawnPosition : Packet
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
        }
    }
    //0x07
    public class Packet_UseEntity : Packet
    {
        public int user { get; set; }
        public int target { get; set; }
        public byte left { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x07);
            writeInt(str, user);
            writeInt(str, target);
            writeByte(str, left);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x08
    public class Packet_UpdateHealth : Packet
    {
        public short health { get; set; }
        public short food { get; set; }
        public float saturation { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            health = readShort(str);
            food = readShort(str);
            saturation = readFloat(str);
        }
    }
    //0x09
    public class Packet_Respawn : Packet
    {
        public int dimension { get; set; }
        public byte difficulty { get; set; }
        public byte creative { get; set; }
        public short worldHeight { get; set; }
        public string levelType { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x09);
            writeInt(str, dimension);
            writeByte(str, difficulty);
            writeByte(str, creative);
            writeShort(str, worldHeight);
            writeString(str, levelType);
        }
        public override void read(Stream str)
        {
            dimension = readInt(str);
            difficulty = readByte(str);
            creative = readByte(str);
            worldHeight = readShort(str);
            levelType = readString(str);
        }
    }
    //0x0A
    public class Packet_Player : Packet
    {
        public bool onGround { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x0A);
            writeBool(str, onGround);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0B
    public class Packet_PlayerPos : Packet
    {
        public double x { get; set; }
        public double y { get; set; }
        public double stance { get; set; }
        public double z { get; set; }
        public bool onGround { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x0B);
            writeDouble(str, x);
            writeDouble(str, y);
            writeDouble(str, stance);
            writeDouble(str, z);
            writeBool(str, onGround);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0C
    public class Packet_PlayerLook : Packet
    {
        public float yaw { get; set; }
        public float pitch { get; set; }
        public bool onGround { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x0C);
            writeFloat(str, yaw);
            writeFloat(str, pitch);
            writeBool(str, onGround);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0D
    public class Packet_PlayerPosAndLook : Packet
    {
        public double x { get; set; }
        public double y { get; set; }
        public double stance { get; set; }
        public double z { get; set; }
        public float yaw { get; set; }
        public float pitch { get; set; }
        public bool onGround { get; set; }

        public override void write(Stream str)
        {
            writeByte(str, 0x0D);
            writeDouble(str, x);
            writeDouble(str, y);
            writeDouble(str, stance);
            writeDouble(str, z);
            writeFloat(str, yaw);
            writeFloat(str, pitch);
            writeBool(str, onGround);
        }

        public override void read(Stream str)
        {
            x = readDouble(str);
            stance = readDouble(str);
            y = readDouble(str);
            z = readDouble(str);
            yaw = readFloat(str);
            pitch = readFloat(str);
            onGround = readBool(str);
        }
    }
    //0x0E
    public class Packet_PlayerDigging : Packet
    {
        public DigStatus status { get; set; }
        public int x { get; set; }
        public byte y { get; set; }
        public int z { get; set; }
        public Face face { get; set; }

        public override void write(Stream str)
        {
            writeByte(str, 0x0E);
            writeSByte(str, (sbyte)status);
            writeInt(str, x);
            writeByte(str, y);
            writeInt(str, z);
            writeByte(str, (byte)face);
        }

        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x0F
    public class Packet_PlayerBlockPlacement : Packet
    {
        public int x { get; set; }
        public sbyte y { get; set; }
        public int z { get; set; }
        public Face direction { get; set; }
        public int held { get; set; }

        public override void write(Stream str)
        {
            writeByte(str, 0x0F);
            writeInt(str, x);
            writeSByte(str, y);
            writeInt(str, z);
            writeSByte(str, (sbyte)direction);
            writeInt(str, held);
        }

        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x10
    public class Packet_HoldingChange : Packet
    {
        public short slotID { get; set; }
        public override void write(Stream str)
        {
            writeByte(str, 0x0F);
            writeShort(str, slotID);

        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x11
    public class Packet_UseBed : Packet
    {
        public int eID { get; set; }
        public sbyte inBed { get; set; }
        public int x { get; set; }
        public sbyte y { get; set; }
        public int z { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            inBed = readSByte(str);
            x = readInt(str);
            y = readSByte(str);
            z = readInt(str);
        }
    }
    //0x12
    public class Packet_Animation : Packet
    {
        public int eID { get; set; }
        public Animation animation { get; set; }

        public override void write(Stream str)
        {
            writeInt(str, eID);
            writeSByte(str, (sbyte)animation);
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            animation = (Animation)readSByte(str);
        }
    }
    //0x13
    public class Packet_EntityAction : Packet
    {
        public int eID { get; set; }
        public MobAction mobAction { get; set; }

        public override void write(Stream str)
        {
            writeInt(str, eID);
            writeSByte(str, (sbyte)mobAction);
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            mobAction = (MobAction)readSByte(str);
        }
    }
    //0x14
    public class Packet_NamedEntitySpawn : Packet
    {
        public int eID { get; set; }
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public Byte rotation { get; set; }
        public Byte pitch { get; set; }
        public short item { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            name = readString(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            rotation = readByte(str);
            pitch = readByte(str);
            item = readShort(str);
        }
    }
    //0x15
    public class Packet_PickupSpawn : Packet
    {
        public int eID { get; set; }
        public short item { get; set; }
        public byte count { get; set; }
        public short damage { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public byte rotation { get; set; }
        public byte pitch { get; set; }
        public byte roll { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            item = readShort(str);
            count = readByte(str);
            damage = readShort(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            rotation = readByte(str);
            pitch = readByte(str);
            roll = readByte(str);
        }
    }
    //0x16
    public class Packet_CollectItem : Packet
    {
        public int itemEID { get; set; }
        public int playerEID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void read(Stream str)
        {
            itemEID = readInt(str);
            playerEID = readInt(str);
        }
    }
    //0x17
    public class Packet_AddObjVehicle : Packet
    {
        public int eID { get; set; }
        public byte type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int fbeID { get; set; }
        public short speedX { get; set; }
        public short speedY { get; set; }
        public short speedZ { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }

        public override void read(Stream str)
        {
            eID = readInt(str);
            type = readByte(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            fbeID = readInt(str);
            if (fbeID > 0)
            {
                speedX = readShort(str);
                speedY = readShort(str);
                speedZ = readShort(str);
            }
        }
    }
    //0x18
    public class Packet_MobSpawn : Packet
    {
        public int eID { get; set; }
        public sbyte type { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }
        public byte hyaw { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            type = readSByte(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            yaw = readByte(str);
            pitch = readByte(str);
            hyaw = readByte(str);
            byte xx;
            xx = readByte(str);
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        readSByte(str);
                        break;
                    case 1:
                        readShort(str);
                        break;
                    case 2:
                        readInt(str);
                        break;
                    case 3:
                        readFloat(str);
                        break;
                    case 4:
                        readString(str);
                        break;
                    case 5:
                        readShort(str);
                        readSByte(str);
                        readShort(str);
                        break;
                    case 6:
                        readInt(str);
                        readInt(str);
                        readInt(str);
                        break;
                }
                xx = readByte(str);
            }
        }
    }
    //0x19
    public class Packet_EntityPainting : Packet
    {
        public int eID { get; set; }
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int dir { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            name = readString(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            dir = readInt(str);
        }
    }
    //0x1A
    public class Packet_ExpOrb : Packet
    {
        public int eID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public short count { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            count = readShort(str);
        }
    }
    //0x1C
    public class Packet_EntityVel : Packet
    {
        public int eID { get; set; }
        public short vX { get; set; }
        public short vY { get; set; }
        public short vZ { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            vX = readShort(str);
            vY = readShort(str);
            vZ = readShort(str);
        }
    }
    //0x1D
    public class Packet_DestroyEntity : Packet
    {
        public int eID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
        }
    }
    //0x1E
    public class Packet_Entity : Packet
    {
        public int eID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
        }
    }
    //0x1F
    public class Packet_EntityRelativeMove : Packet
    {
        public int eID { get; set; }
        public byte dX { get; set; }
        public byte dY { get; set; }
        public byte dZ { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            dX = readByte(str);
            dY = readByte(str);
            dZ = readByte(str);
        }
    }
    //0x20
    public class Packet_EntityLook : Packet
    {
        public int eID { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            yaw = readByte(str);
            pitch = readByte(str);
        }
    }
    //0x21
    public class Packet_EntityLookAndRelativeMove : Packet
    {
        public int eID { get; set; }
        public int dX { get; set; }
        public int dY { get; set; }
        public int dZ { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            dX = readByte(str);
            dY = readByte(str);
            dZ = readByte(str);
            yaw = readByte(str);
            pitch = readByte(str);
        }
    }
    //0x22
    public class Packet_EntityTeleport : Packet
    {
        public int eID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public byte yaw { get; set; }
        public byte pitch { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            yaw = readByte(str);
            pitch = readByte(str);
        }
    }
    //0x23
    public class Packet_EntityHeadLook : Packet
    {
        public int eID { get; set; }
        public int headYaw { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            headYaw = readByte(str);
        }
    }
    //0x26
    public class Packet_EntityStatus : Packet
    {
        public int eID { get; set; }
        public EntityStatus eStatus { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            eStatus = (EntityStatus)readByte(str);
        }
    }
    //0x27
    public class Packet_AttachEntity : Packet
    {
        public int eID { get; set; }
        public int vehicleID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            vehicleID = readInt(str);
        }
    }
    //0x28
    public class Packet_EntityMetadata : Packet
    {
        // TODO: Add more variables (0x28 EntityMetadata)
        public int eID { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            byte xx;
            xx = readByte(str);
            while (xx != (byte)127)
            {
                int index = xx & 0x1F; //Lower 5 bits
                int ty = xx >> 5;     //Upper 3 bits
                switch (ty)
                {
                    case 0:
                        readSByte(str);
                        break;
                    case 1:
                        readShort(str);
                        break;
                    case 2:
                        readInt(str);
                        break;
                    case 3:
                        readFloat(str);
                        break;
                    case 4:
                        readString(str);
                        break;
                    case 5:
                        readShort(str);
                        readSByte(str);
                        readShort(str);
                        break;
                    case 6:
                        readInt(str);
                        readInt(str);
                        readInt(str);
                        break;
                }
                xx = readByte(str);
            }
        }
    }
    //0x29
    public class Packet_EntityEffect : Packet
    {
        public int eID { get; set; }
        public byte effectID { get; set; }
        public byte amplifier { get; set; }
        public short duration { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            effectID = readByte(str);
            amplifier = readByte(str);
            duration = readShort(str);
        }
    }
    //0x2A
    public class Packet_RemoveEntityEffect : Packet
    {
        public int eID { get; set; }
        public byte effectID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            effectID = readByte(str);
        }
    }
    //0x2B
    public class Packet_Experience : Packet
    {
        public float xpBar { get; set; }
        public short level { get; set; }
        public short totalxp { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            xpBar = readFloat(str);
            level = readShort(str);
            totalxp = readShort(str);
        }
    }
    //0x32
    public class Packet_PreChunk : Packet
    {
        public int x { get; set; }
        public int z { get; set; }
        public bool mode { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            z = readInt(str);
            mode = readBool(str);
        }
    }
    //0x33  //!
    public class Packet_MapChunk : Packet
    {
        public int x { get; set; }
        public int z { get; set; }
        public bool groundUC { get; set; }
        public short primaryBM { get; set; }
        public short addBM { get; set; }
        public int size { get; set; }
        public byte[] data { get; set; }
        private byte[] rawData { get; set; }
        public byte[] chunkData
        {
            get
            {
                if (rawData == null)
                    rawData = ZlibStream.UncompressBuffer(data);

                return rawData;
            }
            set
            {
                rawData = value;
            }
        }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            z = readInt(str);
            groundUC = readBool(str);
            primaryBM = readShort(str);
            addBM = readShort(str);
            size = readInt(str);
            readInt(str);
            data = new byte[size];
            str.Read(data, 0, size);
        }
    }
    //0x34 //!
    public class Packet_MultiBlockChange : Packet
    {
        public int x { get; set; }
        public int z { get; set; }
        public short count { get; set; }
        public int[] rawdata { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            z = readInt(str);
            int count = readShort(str);
            rawdata = new int[count];
            int ds = readInt(str);
            for (int i = 0; i < count; i++)
                rawdata[i] = readInt(str);
        }
    }
    //0x35
    public class Packet_BlockChange : Packet
    {
        public int x { get; set; }
        public short y { get; set; }
        public int z { get; set; }
        public byte type { get; set; }
        public byte metadata { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readByte(str);
            z = readInt(str);
            type = readByte(str);
            metadata = readByte(str);
        }
    }
    //0x36?
    public class Packet_BlockAction : Packet
    {
        public int x { get; set; }
        public short y { get; set; }
        public int z { get; set; }
        public byte byte1 { get; set; }
        public byte byte2 { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readShort(str);
            z = readInt(str);
            byte1 = readByte(str);
            byte2 = readByte(str);
        }
    }
    //0x3C
    public class Packet_Explosion : Packet
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public float radius { get; set; }
        public int recordcount { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add Records (0x3C Explosion)
            x = readDouble(str);
            y = readDouble(str);
            z = readDouble(str);
            radius = readFloat(str);
            recordcount = readInt(str);
            for (int i = 0; i < recordcount; i++)
            {
                readByte(str);
                readByte(str);
                readByte(str);
            }
        }
    }

    //0x3D
    public class Packet_SoundEffect : Packet
    {
        public int effectID { get; set; }
        public int x { get; set; }
        public byte y { get; set; }
        public int z { get; set; }
        public int data { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            effectID = readInt(str);
            x = readInt(str);
            y = readByte(str);
            z = readInt(str);
            data = readInt(str);
        }
    }
    //0x3E?
    public class Packet_NewOrInvalidState : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x3E? NewOrInvalidState)
            readSByte(str);
            readSByte(str);
        }
    }
    //0x46
    public class Packet_GameStateChange : Packet
    {
        public ChangeGameStateReason reason { get; set; }
        public byte gamemode { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            reason = (ChangeGameStateReason)readByte(str);
            gamemode = readByte(str);
        }
    }

    //0x47
    public class Packet_Thunder : Packet
    {
        public int eID { get; set; }
        public int isLightningBolt { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            isLightningBolt = readByte(str); //always true
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
        }
    }
    //0x64
    public class Packet_OpenWnd : Packet
    {
        public byte wndID { get; set; }
        public byte invtype { get; set; }
        public string wndtitle { get; set; }
        public byte slotcount { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            wndID = readByte(str);
            invtype = readByte(str);
            wndtitle = readString(str);
            slotcount = readByte(str);
        }
    }
    //0x65
    public class Packet_CloseWnd : Packet
    {
        public byte wndID { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            wndID = readByte(str);
        }
    }
    //0x66
    public class Packet_WndClick : Packet
    {
        public sbyte wndID { get; set; }
        public short slot { get; set; }
        public sbyte rightClick { get; set; }
        public short actionNumber { get; set; }
        public bool shift { get; set; }
        public override void write(Stream str)
        {
            writeSByte(str, wndID);
            writeShort(str, slot);
            writeSByte(str, rightClick);
            writeShort(str, actionNumber);
            writeBool(str, shift);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x67
    public class Packet_SetSlot : Packet
    {
        sbyte wndID { get; set; }
        short slot { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add slotData variable (0x67 SetSlot)
            wndID = readSByte(str);
            slot = readShort(str);
            readSlotData(str);
        }
    }
    //0x68
    public class Packet_WndItems : Packet
    {
        byte wndID { get; set; }
        short count { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add slotdata array (0x68 WndItems)
            wndID = readByte(str);
            count = readShort(str);
            for (int i = 0; i < count; i++)
            {
                readSlotData(str);
            }
        }
    }
    //0x69
    public class Packet_UpdateWndProp : Packet
    {
        byte wndID { get; set; }
        short property { get; set; }
        short value { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            wndID = readByte(str);
            property = readShort(str);
            value = readShort(str);
        }
    }
    //0x6A
    public class Packet_Transaction : Packet
    {
        public sbyte wndID { get; set; }
        public short actionNumber { get; set; }
        public bool accepted { get; set; }
        public override void write(Stream str)
        {
            writeSByte(str, wndID);
            writeShort(str, actionNumber);
            writeBool(str, accepted);
        }
        public override void read(Stream str)
        {
            wndID = readSByte(str);
            actionNumber = readShort(str);
            accepted = readBool(str);
        }
    }
    //0x6B
    public class Packet_CreativeInventoryAction : Packet
    {
        public short slot { get; set; }
        //TODO: Implement write 0x6B CreativeInventoryAction (Needs Slot data object to be implemented first)
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            slot = readShort(str);
            readSlotData(str);
        }
    }
    //0x6C
    public class Packet_EnchantItem : Packet
    {
        public sbyte wndID { get; set; }
        public sbyte enchantment { get; set; }
        public override void write(Stream str)
        {
            writeSByte(str, wndID);
            writeSByte(str, enchantment);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x82
    public class Packet_UpdateSign : Packet
    {
        public int x { get; set; }
        public short y { get; set; }
        public int z { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
        public string text3 { get; set; }
        public string text4 { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readShort(str);
            z = readInt(str);
            text1 = readString(str);
            text2 = readString(str);
            text3 = readString(str);
            text4 = readString(str);
        }
    }
    //0x83
    public class Packet_ItemData : Packet
    {
        public short type { get; set; }
        public short ID { get; set; }
        public byte textlength { get; set; }
        public byte[] text { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            type = readShort(str);
            ID = readShort(str);
            textlength = readByte(str);
            text = readByteArray(str, textlength);
        }
    }
    //0x84
    public class Packet_EntityTileUpdate : Packet
    {
        public int x { get; set; }
        public short y { get; set; }
        public int z { get; set; }
        public byte action { get; set; }
        public int custom1 { get; set; }
        public int custom2 { get; set; }
        public int custom3 { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readShort(str);
            z = readInt(str);
            action = readByte(str);
            custom1 = readInt(str);
            custom2 = readInt(str);
            custom3 = readInt(str);
        }
    }
    //0xC8
    public class Packet_IncStatistic : Packet
    {
        public int statisticID { get; set; }
        public byte amount { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            statisticID = readInt(str);
            amount = readByte(str);
        }
    }
    //0xCA
    public class Packet_PlayerAbilities : Packet
    {
        public bool invulnerability { get; set; }
        public bool isFlying { get; set; }
        public bool canFly { get; set; }
        public bool instantDestroy { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            invulnerability = readBool(str);
            isFlying = readBool(str);
            canFly = readBool(str);
            instantDestroy = readBool(str);
        }
    }
    //0xFA
    public class Packet_PluginMessage : Packet
    {
        string channel { get; set; }
        short length { get; set; }
        byte[] data { get; set; }
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            channel = readString(str);
            length = readShort(str);
            data = readByteArray(str, length);
        }
    }
    // TODO: Implement 0xFE Server List Ping (Basically get server info)
    //0xC9
    public class Packet_PlayerListItem : Packet
    {
        public string name { get; set; }
        public bool online { get; set; }
        public short ping { get; set; }

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            name = readString(str);
            online = readBool(str);
            ping = readShort(str);
        }
    }
    //0xFF
    public class Packet_Kick : Packet
    {
        public string dataString { get; set; }

        public override void write(Stream str)
        {
            writeString(str, dataString);
        }
        public override void read(Stream str)
        {
            dataString = readString(str);
        }
    }
}

