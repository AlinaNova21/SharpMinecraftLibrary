using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MinecraftLibrary
{
    public abstract  class Packet
    {
        public abstract void write(Stream str);
        public abstract void read(Stream str);
        protected byte[] reverse(byte[] data)
        {
            byte[] res = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                res[i]=data[data.Length-1-i];
            }
            return res;
        }
        protected string readString(Stream str)
        {
            int len = readShort(str);
            byte[] tmp=new byte[len*2];
            str.Read(tmp, 0, len*2);
            return UnicodeEncoding.BigEndianUnicode.GetString(tmp, 0, tmp.Length);
        }
        protected void writeString(Stream str, string data)
        {
            writeShort(str,(short)data.Length);
            byte[] name = ASCIIEncoding.BigEndianUnicode.GetBytes(data);
            str.Write(name, 0, name.Length);
        }
        protected short readShort(Stream str)
        {
            byte[] tmp=new byte[2];
            str.Read(tmp, 0, 2);
            return BitConverter.ToInt16(reverse(tmp), 0);
        }
        protected void writeShort(Stream str,short data)
        {
            byte[] tmp=reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
        }
        protected int readInt(Stream str)
        {
            byte[] tmp = new byte[4];
            str.Read(tmp, 0, 4);
            tmp = reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        protected void writeInt(Stream str,int data)
        {
            byte[] tmp= reverse(BitConverter.GetBytes(data));
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
            return BitConverter.ToDouble (reverse(tmp), 0);
        }
        protected void writeDouble(Stream str, double data)
        {
            byte[] tmp = reverse(BitConverter.GetBytes(data));
            str.Write(tmp, 0, tmp.Length);
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
        protected sbyte readSByte(Stream str)
        {
            byte[] tmp=new byte[1];
            str.Read(tmp, 0,1);
            return (sbyte)tmp[0];

        }
        protected byte readByte (Stream str)
        {
            byte[] tmp=new byte[1];
            str.Read(tmp, 0,1);
            return (byte)tmp[0];
     }

    class Packet_KeepAlive : Packet
    {
        public int ID { get; set; }
        public override byte[] write()
        {
            writeInt(ID), ref data);
            return data.ToArray();

        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x00)
            {
                data.Dequeue();
                ID = readInt(fromQueue(4, ref data), 0);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Login : Packet
    {
        public int protocol;
        public string username;

        public int eID;
        public long seed;
        public int mode;
        public SByte dim;
        public SByte difficulty;
        public byte worldHeight;
        public byte maxPlayers;

        public override byte[] write()
        {
            Queue<byte> data = new Queue<byte>();
            data.Enqueue(0x01);
            toQueue(writeInt(protocol), ref data);
            toQueue(writeString(username), ref data);
            toQueue(writeLong(0), ref data);
            data.Enqueue(0x00);//int
            data.Enqueue(0x00);//int
            data.Enqueue(0x00);//int
            data.Enqueue(0x00);//int
            data.Enqueue(0x00);//byte
            data.Enqueue(0x00);//byte
            data.Enqueue(0x00);//ubyte
            data.Enqueue(0x00);//ubyte
            return data.ToArray();
        }
        public override void read(Stream str)
        {
            readInt(str);
            readString(str);
            readLong(str);
            readInt(str);
            readSByte(str);
            readSByte(str);
            readByte(str);
            readByte(str);
            
        }
    }
    class Packet_Handshake : Packet
    {
        public string dataString { get; set; }
        public override byte[] write()
        {
            Queue<byte> data = new Queue<byte>();
            data.Enqueue(0x02);
            byte[] strData;
            strData = writeString(dataString);
            for (int I = 0; I < strData.Length; I++)
            {
                data.Enqueue(strData[I]);
            }
            return data.ToArray();

        }
        public override void read(Stream str)
        {
            dataString=readString(str);
        }
    }
    class Packet_Chat : Packet
    {
        public string dataString { get; set; }
        public override byte[] write()
        {
            Queue<byte> data = new Queue<byte>();
            data.Enqueue(0x03);
            byte[] strData;
            strData = writeString(dataString);
            for (int I = 0; I < strData.Length; I++)
            {
                data.Enqueue(strData[I]);
            }
            return data.ToArray();

        }
        public override void read(Stream str)
        {
            dataString=readString(str);
        }
    }
    class Packet_Time : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override void read(Stream str)
        {
            time=readLong(str);
        }//:P Im going back to my pc to write writing code. :P I suppose ill let you finish it :P awwwww....
        //gotta go for a bath -.-
    }
    class Packet_EntityEquipment : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x05)
            {
                fromQueue(11, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_SpawnPosition : Packet
    {
        public int x;
        public int y;
        public int z;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x06)
            {
                if (data.Count < 13) return false;
                data.Dequeue();
                x = readInt(fromQueue(4, ref data), 0);
                y = readInt(fromQueue(4, ref data), 0);
                z = readInt(fromQueue(4, ref data), 0);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_UpdateHealth: Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x08)
            {
                fromQueue(9, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Respawn : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x09)
            {
                fromQueue(14, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_PlayerPosAndLook : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x0D)
            {
                fromQueue(42, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_UseBed : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x11)
            {
                fromQueue(15, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Animation : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x12)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_NamedEntitySpawn : Packet
    {
        public int eID;
        public string name;
        public int x;
        public int y;
        public int z;
        public SByte rotation;
        public SByte pitch;
        public short item;

        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x14)
            {
                if (data.Count < 23) return false;
                data.Dequeue();
                eID = readInt(fromQueue(4, ref data),0);
                string tmp = readString(data.ToArray());
                if (tmp != "")
                {
                    fromQueue((tmp.Length + 1) * 2, ref data);
                    name = tmp;
                }
                x = readInt(fromQueue(4, ref data), 0);
                y = readInt(fromQueue(4, ref data), 0);
                z = readInt(fromQueue(4, ref data), 0);
                rotation = (SByte)data.Dequeue();
                pitch = (SByte)data.Dequeue();
                item = readShort(fromQueue(2, ref data), 0);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_PickupSpawn : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x15)
            {
                fromQueue(25, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_CollectItem : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x16)
            {
                fromQueue(9, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_AddObjVehicle : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x17)
            {
                fromQueue(22, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_MobSpawn : Packet
    {
        public int eID;
        public byte type;
        public int x;
        public int y;
        public int z;
        public byte yaw;
        public byte pitch;

        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x18)
            {
                if (!data.Contains(0x7f)) return false;
                if (data.Count < 20) return false;
                data.Dequeue();
                eID = readInt(fromQueue(4, ref data), 0);
                type = data.Dequeue();
                x = readInt(fromQueue(4, ref data), 0);
                y = readInt(fromQueue(4, ref data), 0);
                z = readInt(fromQueue(4, ref data), 0);
                yaw = data.Dequeue();
                pitch=data.Dequeue();

                byte xx = data.Dequeue();
                while (xx != 127)
                {
                    int index = xx & 0x1F ;//# Lower 5 bits
                    int ty    = xx >> 5   ;//# Upper 3 bits
                    //Console.WriteLine(xx + "_" + index + "_" + ty);
                    if (ty == 0) data.Dequeue();
                    if (ty == 1) fromQueue(2,ref data);
                    if (ty == 2) fromQueue(4,ref data); 
                    if (ty == 3) fromQueue(4,ref data);
                    if (ty == 4)
                    {
                       string tmp= readString(data.ToArray());
                       for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                       {
                           data.Dequeue();
                       }
                    }
                    xx = data.Dequeue();
                }
                return true;
            }
            else { return false; }
        }
    }
    class Packet_ExpOrb : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x1A)
            {
                fromQueue(19, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityVel : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x1C)
            {
                fromQueue(11, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_DestroyEntity : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x1D)
            {
                fromQueue(5, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Entity : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x1E)
            {
                fromQueue(5, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityRelativeMove : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x1F)
            {
                fromQueue(8, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityPainting : Packet
    {
        public string name;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x19)
            {
                if (data.Count < 32) return false;
                data.Dequeue();
                    fromQueue(4, ref data);
                    string tmp = readString(data.ToArray());
                    for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                    {
                        data.Dequeue();
                    }
                    name = tmp;
                    fromQueue(16, ref data);
                    return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityLook : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x20)
            {
                fromQueue(7, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityLookAndRelativeMove : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x21)
            {
                fromQueue(10, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityTeleport : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x22)
            {
                fromQueue(19, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityStatus : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x26)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_AttachEntity : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x27)
            {
                fromQueue(9, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_EntityEffect : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x29)
            {
                fromQueue(9, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_RemoveEntityEffect : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x2A)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_PreChunk : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x32)
            {
                fromQueue(10, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_MapChunk : Packet
    {
        public int x;
        public short y;
        public int z;
        public SByte size_x;
        public SByte size_y;
        public SByte size_z;
        public int size;
        public byte[] data;

        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x33)
            {
                data.Dequeue();
                x = readInt(fromQueue(4, ref data), 0);
                y = readShort(fromQueue(2, ref data), 0);
                z = readInt(fromQueue(4, ref data), 0);
                size_x = (SByte)data.Dequeue();
                size_y = (SByte)data.Dequeue();
                size_z = (SByte)data.Dequeue();
                size = readInt(fromQueue(4, ref data), 0);
                if (data.Count < size) return false;
                this.data = fromQueue(size, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_MultiBlockChange : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x34)
            {
                fromQueue(9, ref data);
                int cnt = readShort(fromQueue(2, ref data),0);
                fromQueue(cnt * 4, ref data);

                return true;
            }
            else { return false; }
        }
    }
    class Packet_BlockChange : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x35)
            {
                fromQueue(12, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_BlockAction : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x36)
            {
                fromQueue(13, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_SoundEffect : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x3D)
            {
                fromQueue(18, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_NewOrInvalidState : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x46)
            {
                fromQueue(3, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Thunder : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x47)
            {
                fromQueue(18, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_CloseWnd : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x65)
            {
                fromQueue(2, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_SetSlot : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x67)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_WndItems : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x68)
            {
                fromQueue(94, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_UpdateWndProp : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x69)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_UpdateSign : Packet
    {
        public int x;
        public short y;
        public int z;
        public string text1;
        public string text2;
        public string text3;
        public string text4;

        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0x82)
            {
                //todo Messy! Cleanup!
                data.Dequeue();
                x = readInt(fromQueue(4, ref data), 0);
                y = readShort(fromQueue(2, ref data), 0);
                z = readInt(fromQueue(4, ref data), 0);
                string tmp;
                tmp = readString(data.ToArray());
                for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                {
                    data.Dequeue();
                }
                text1 = tmp;
                tmp = readString(data.ToArray());
                for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                {
                    data.Dequeue();
                }
                text2 = tmp;
                tmp = readString(data.ToArray());
                for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                {
                    data.Dequeue();
                }
                text3 = tmp;
                tmp = readString(data.ToArray());
                for (int I = 0; I < (tmp.Length + 1) * 2; I++)
                {
                    data.Dequeue();
                }
                text4 = tmp;
                return true;
            }
            else { return false; }
        }
    }
    class Packet_IncStatistic : Packet
    {
        public long time;
        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0xC8)
            {
                fromQueue(6, ref data);
                return true;
            }
            else { return false; }
        }
    }

    class Packet_PlayerListItem : Packet
    {
        public string name;
        public bool online;
        public short ping;

        public override byte[] write()
        {
            return new byte[] { };
        }
        public override bool read(Queue<byte> data)
        {
            if (data.Peek() == 0xC9)
            {
                if (data.Count < 6) return false;
                data.Dequeue();
                string tmp = readString(data.ToArray());
                if (tmp != "")
                {
                    fromQueue((tmp.Length + 1) * 2, ref data);
                    name = tmp;
                }
                online = readBool(fromQueue(1, ref data),0);
                ping = readShort(fromQueue(2, ref data),0);
                return true;
            }
            else { return false; }
        }
    }
    class Packet_Kick : Packet
    {
        public string dataString { get; set; }
        public override byte[] write()
        {
            return new byte[] { };
        }
        
        public override void read(Stream str)
        {
            dataString=readString(str);
        }
    }

}
