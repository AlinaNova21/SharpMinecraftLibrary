using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MinecraftLibrary
{
    public abstract class Packet
    {
        public abstract void write(Stream str);
        public abstract void read(Stream str);
        protected byte[] reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
            //byte[] res = new byte[data.Length];
            //for (int i = 0; i < data.Length; i++)
            //{
            //    res[i] = data[data.Length - 1 - i];
            //}
            //return res;
        }
        protected byte[] readSTUB(Stream str, int len)
        {
            byte[] tmp = new byte[len];
            str.Read(tmp, 0, len);
            //System.Diagnostics.Debug.WriteLine("STUBBED!");
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
 0x10C, 0x10D, 0x10E, 0x10F, 0x122, //#WOOD
 0x110, 0x111, 0x112, 0x113, 0x123, //#STONE
 0x10B, 0x100, 0x101, 0x102, 0x124, //#IRON
 0x114, 0x115, 0x116, 0x117, 0x125, //#DIAMOND
 0x11B, 0x11C, 0x11D, 0x11E, 0x126, //#GOLD
 
 //#ARMOUR
 //#helmet, chestplate, leggings, boots
 0x12A, 0x12B, 0x12C, 0x12D, //#LEATHER
 0x12E, 0x12F, 0x130, 0x131, //#CHAIN
 0x132, 0x133, 0x134, 0x135, //#IRON
 0x136, 0x137, 0x138, 0x139, //#DIAMOND
 0x13A, 0x13B, 0x13C, 0x14D  //#GOLD};
                             };
            if (readShort(str) != -1)
            {
                readSByte(str);
                if (enchantable.Contains(readShort(str)))
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
            writeInt(str,ID);
        }
        public override void read(Stream str)
        {
                ID = readInt(str);
        }
    }
    //0x01
    public class Packet_Login : Packet
    {
        public int protocol;
        public string username;

        public int eID;
        public long seed;
        public int mode;
        public SByte dim;
        public SByte difficulty;
        public byte worldHeigth;
        public byte maxPlayers;

        public override void write(Stream str)
        {
            writeByte(str,0x01);
            writeInt(str,protocol);
            writeString(str, username);
            writeLong(str, 0);
            writeInt(str,0);
            writeSByte(str, 0x00);
            writeSByte(str, 0x00);
            writeByte(str, 0x00);
            writeByte(str, 0x00);
        }
        public override void read(Stream str)
        {
            eID=readInt(str);
            readString(str);//unused
            seed=readLong(str);
            mode=readInt(str);
            dim=readSByte(str);
            difficulty=readSByte(str);
            worldHeigth=readByte(str);
            maxPlayers=readByte(str);
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
            dataString=readString(str);
        }
    }
    //0x04
    public class Packet_Time : Packet
    {
        public long time;
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            time=readLong(str);
        }
    }
    //0x05
    public class Packet_EntityEquipment : Packet
    {
        public int eID;
        public short slot;
        public short itemID;
        public short damage;
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            slot = readShort(str);
            itemID= readShort(str);
            damage = readShort(str);
        }
    }
    //0x06
    public class Packet_SpawnPosition : Packet
    {
        public int x;
        public int y;
        public int z;
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
        public int user;
        public int target;
        public bool left;
        public override void write(Stream str)
        {
            writeByte(str, 0x07);
            writeInt(str, user);
            writeInt(str, target);
            writeBool(str, left);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    //0x08
    public class Packet_UpdateHealth : Packet
    {
        public short health;
        public short food;
        public float saturation;

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            health=readShort(str);
            food = readShort(str);
            saturation = readFloat(str);
        }
    }
    //0x09
    public class Packet_Respawn : Packet
    {
        public sbyte dim;
        public sbyte difficulty;
        public sbyte creative;
        public short worldHeight;
        public long mapSeed;
        public override void write(Stream str)
        {
            writeByte(str, 0x09);
            writeSByte(str, dim);
            writeSByte(str, difficulty );
            writeSByte(str, creative);
            writeShort(str, worldHeight);
            writeLong(str, mapSeed);
        }
        public override void read(Stream str)
        {
            dim = readSByte(str);
            difficulty  = readSByte(str);
            creative  = readSByte(str);
            worldHeight  = readShort(str);
            mapSeed = readLong(str);
        }
    }
    //0x0A
    public class Packet_Player : Packet
    {
        public bool onGround;
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
        public double x;
        public double y;
        public double stance;
        public double z;
        public bool onGround;
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
        public float yaw;
        public float pitch;
        public bool onGround;
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
        public double x;
        public double y;
        public double stance;
        public double z;
        public float yaw;
        public float pitch;
        public bool onGround;
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
        public sbyte status;
        public int x;
        public sbyte y;
        public int z;
        public sbyte face;

        public override void write(Stream str)
        {
            writeByte(str, 0x0E);
            writeSByte(str,status);
            writeInt(str, x);
            writeSByte(str, y);
            writeInt(str, z);
            writeSByte(str, face);
        }
        public override void read(Stream str)
        {
            throw new NotImplementedException();
        }
    }
    // TODO: Implement 0x0F Player Block Placement 
    // TODO: Implement 0x10 Holding Change
    //0x11
    public class Packet_UseBed : Packet
    {
        public int eID;
        public sbyte inBed;
        public int x;
        public sbyte y;
        public int z;
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID=readInt(str);
            inBed = readSByte(str);
            x = readInt(str);
            y = readSByte(str);
            z = readInt(str);
        }
    }
    //0x12
    public class Packet_Animation : Packet
    {
        public int eID;
        public sbyte animation;
        //TODO: Implement write 0x12 Animation
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            animation=readSByte(str);
        }
    }
    // TODO: Implement 0x13 Entity Action
    //0x14
    public class Packet_NamedEntitySpawn : Packet
    {
        public int eID;
        public string name;
        public int x;
        public int y;
        public int z;
        public SByte rotation;
        public SByte pitch;
        public short item;
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
            rotation = readSByte(str);
            pitch = readSByte(str);
            item = readShort(str);
        }
    }
    //0x15
    public class Packet_PickupSpawn : Packet
    {
        public int eID;
        public short item;
        public sbyte count;
        public short damage;
        public int x;
        public int y;
        public int z;
        public sbyte rotation;
        public sbyte pitch;
        public sbyte roll;

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID = readInt(str);
            item = readShort(str);
            count = readSByte(str);
            damage=readShort(str);
            x = readInt(str);
            y = readInt(str);
            z = readInt(str);
            rotation = readSByte(str);
            pitch = readSByte(str);
            roll = readSByte(str);
        }
    }
    //0x16
    public class Packet_CollectItem : Packet
    {
        public int itemEID;
        public int playerEID;
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            itemEID= readInt(str);
            playerEID = readInt(str);
        }
    }
    //0x17
    public class Packet_AddObjVehicle : Packet
    {
        public int eID;
        public sbyte type;
        public int x;
        public int y;
        public int z;
        public int fbeID;
        public short unknown;

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
            fbeID = readInt(str);
            if (fbeID > 0)
            {
                unknown = readShort(str);
                unknown = readShort(str);
                unknown = readShort(str);
            }
        }
    }
    //0x18
    public class Packet_MobSpawn : Packet
    {
        public int eID;
        public sbyte type;
        public int x;
        public int y;
        public int z;
        public sbyte yaw;
        public sbyte pitch;

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
            yaw = readSByte(str);
            pitch = readSByte(str);
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
        public int eID;
        public string name;
        public int x;
        public int y;
        public int z;
        public int dir;
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            eID=readInt(str);
            name=readString(str);
            x=readInt(str);
            y=readInt(str);
            z=readInt(str);
            dir=readInt(str);
        }
    }
    //0x1A
    public class Packet_ExpOrb : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x1A ExpOrb)
            readInt(str);
            readInt(str);
            readInt(str);
            readInt(str);
            readShort(str);
        }
    }
    //0x1C
    public class Packet_EntityVel : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x1C EntityVel)
            readInt(str);
            readShort(str);
            readShort(str);
            readShort(str);
        }
    }
    //0x1D
    public class Packet_DestroyEntity : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x1D DestroyEntity)
            readInt(str);
        }
    }
    //0x1E
    public class Packet_Entity : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x1E Entity)
            readInt(str);
        }
    }
    //0x1F
    public class Packet_EntityRelativeMove : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x1F EntityRelativeMove)
            readInt(str);
            readSByte(str);
            readSByte(str);
            readSByte(str);
        }
    }
    //0x20
    public class Packet_EntityLook : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x20 EntityLook)
            readInt(str);
            readSByte(str);
            readSByte(str);
        }
    }
    //0x21
    public class Packet_EntityLookAndRelativeMove : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x21 EntityLookAndRelativeMove)
            readInt(str);
            readSByte(str);
            readSByte(str);
            readSByte(str);
            readSByte(str);
            readSByte(str);
        }
    }
    //0x22
    public class Packet_EntityTeleport : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x22 EntityTeleport)
            readInt(str);
            readInt(str);
            readInt(str);
            readInt(str);
            readSByte(str);
            readSByte(str);
        }
    }
    //0x26
    public class Packet_EntityStatus : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x26 EntityStatus)
            readInt(str);
            readSByte(str);
        }
    }
    //0x27
    public class Packet_AttachEntity : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x27 AttachEntity)
            readInt(str);
            readInt(str);
        }
    }
    // TODO: Implement 0x28 Entity Metadata
    //0x29
    public class Packet_EntityEffect : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x29 EntityEffect)
            readInt(str);
            readSByte(str);
            readSByte(str);
            readShort(str);
        }
    }
    //0x2A
    public class Packet_RemoveEntityEffect : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x2A RemoveEntityEffect)
            readInt(str);
            readSByte(str);
        }
    }
    // TODO: Implement 0x2B Experience
    //0x32
    public class Packet_PreChunk : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x32 Prechunk)
            readInt(str);
            readInt(str);
            readBool(str);
        }
    }
    //0x33
    public class Packet_MapChunk : Packet
    {
        public int x;
        public short y;
        public int z;
        public SByte size_x;
        public SByte size_y;
        public SByte size_z;
        public int size;
        public byte[] data;

        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            x = readInt(str);
            y = readShort(str);
            z = readInt(str);
            size_x = readSByte(str);
            size_y = readSByte(str);
            size_z = readSByte(str);
            size = readInt(str);
            data = new byte[size];
            str.Read(data, 0, size);
        }
    }
    //0x34
    public class Packet_MultiBlockChange : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x34 MultiBlockChange)
            readInt(str);
            readInt(str);
            int cnt = readShort(str);
            for (int i = 0; i < cnt; i++)
            {
                readShort(str);
            }
            for (int i = 0; i < cnt; i++)
            {
                readSByte(str);
            }
            for (int i = 0; i < cnt; i++)
            {
                readSByte(str);
            }
        }
    }
    //0x35
    public class Packet_BlockChange : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x35 BlockChange)
            readInt(str);
            readSByte(str);
            readInt(str);
            readSByte(str);
            readSByte(str);
        }
    }
    //0x36?
    public class Packet_BlockAction : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x36? BlockAction)
            readInt(str);
            readShort(str);
            readInt(str);
            readSByte(str);
            readSByte(str);
        }
    }
    // TODO: Implement 0x3C Explosion
    //0x3D
    public class Packet_SoundEffect : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x3D SoundEffect)
            readInt(str);
            readInt(str);
            readSByte(str);
            readInt(str);
            readInt(str);
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
    //0x47
    public class Packet_Thunder : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x47 Thunder)
            readInt(str);
            readBool(str); //always true
            readInt(str);
            readInt(str);
            readInt(str);
        }
    }
    // TODO: Implement 0x64 Open Window
    //0x65
    public class Packet_CloseWnd : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x65 CloseWnd)
            readSByte(str);
        }
    }
    // TODO: Implement 0x66 Window Click
    //0x67
    public class Packet_SetSlot : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x67 SetSlot)
            readSByte(str);
            readShort(str);
            readSlotData(str);
        }
    }
    //0x68
    public class Packet_WndItems : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x68 WndItems)
            readSByte(str);
            short cnt=readShort(str);
            for (int i=0;i<cnt;i++)
            {
                readSlotData(str);
            }
        }
    }
    //0x69
    public class Packet_UpdateWndProp : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0x69 UpdateWndProp)
            readSByte(str);
            readShort(str);
            readShort(str);
        }
    }
    // TODO: Implement 0x6A Transaction
    // TODO: Implement 0x6B Creative inventory action
    // TODO: Implement 0x6C Enchant item
    //0x82
    public class Packet_UpdateSign : Packet
    {
        public int x;
        public short y;
        public int z;
        public string text1;
        public string text2;
        public string text3;
        public string text4;

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
    // TODO: Implement 0x83 Item Data (Maps)
    //0xC8
    public class Packet_IncStatistic : Packet
    {
        public override void write(Stream str)
        {
            throw new NotImplementedException();
        }
        public override void read(Stream str)
        {
            // TODO: Add variables (0xC8 IncStatistic)
            readInt(str);
            readSByte(str);
        }
    }
    // TODO: Implement 0xFE Server List Ping (Basically get server info)
    //0xC9
    public class Packet_PlayerListItem : Packet
    {
        public string name;
        public bool online;
        public short ping;

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
            writeString(str, dataString) ;
        }
        public override void read(Stream str)
        {
            dataString = readString(str);
        }
    }

}

