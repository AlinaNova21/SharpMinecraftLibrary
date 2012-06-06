using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MinecraftLibrary
{
    public class Block
    {
        public short ID=0;
        public short metaData=0;
        public int x=0;
        public int y=0;
        public int z=0;
        public Block(){ }
        public Block(int x,int y, int z, short ID,short metaData)
        {
            this.x=x;
            this.y=y;
            this.z=z;
            this.ID=ID;
            this.metaData=metaData;
        }

        private Point getTextCoord()
        {
            Dictionary<int, Point> t = new Dictionary<int, Point>();
            t.Add(1, new Point(1, 0));
            t.Add(2, new Point(0, 0));
            t.Add(3, new Point(3, 0));
            t.Add(4, new Point(0, 1));
            t.Add(5, new Point(4, 0));
            t.Add(8, new Point(3, 4));
            t.Add(9, new Point(3, 4));
            t.Add(50, new Point(0, 5));
            t.Add(67, new Point(6, 3));
            t.Add(98, new Point(6, 3));
            if (t.ContainsKey(ID))
                return t[ID];
            return new Point();
        }
        public float U()
        {
            float ret = 0;
            ret = getTextCoord().X;
            return ret;
        }
        public float V()
        {
            float ret = 0;
            ret = getTextCoord().Y;
            return ret;
        }
    }
    public class Chunk
    {
        public int x;
        //public int y; s
        public int z;
        public Block[, ,] blocks = new Block[16, 256, 16];
        public void update(Packet_MapChunk data)
        {
            x = data.x;
            z = data.z;
            byte[] cubic_chunk_data=new byte[4096];
            //Loop over 16x16x16 chunks in the 16x256x16 column
            int ii=-1;
             for (int i=0;i<16;i++) {
               //If the bitmask indicates this chunk has been sent...
               if ((data.primaryBM & 1 << i) > 0) {
                   ii = ii + 1;
                   //Read data...
                   Array.ConstrainedCopy(data.chunkData, 4096 * ii, cubic_chunk_data, 0, 4096);
                   //io.Read(cubic_chunk_data,0,4096); //2048 for the other arrays, where you'll need to split the data

                   for (int j = 0; j < cubic_chunk_data.Length; j++)
                   {
                       //Retrieve x,y,z and data from each element in cubic_chunk_array

                       //Byte arrays
                       //int bx = data.x * 16 + j & 0x0F;
                       //int by = i * 16 + j >> 8;
                       //int bz = data.z * 16 + (j & 0xF0) >> 4;
                       int bx = j & 0x0F;
                       int by = i * 16 + j >> 8;
                       int bz = (j & 0xF0) >> 4;
                       short bid = cubic_chunk_data[j];

                       //Nibble arrays
                       int data1 = cubic_chunk_data[j] & 0x0F;
                       int data2 = cubic_chunk_data[j] >> 4;

                       int k = 2 * j;
                       int bx1 = data.x * 16 + k & 0x0F;
                       int by1 = i * 16 + k >> 8;
                       int bz1 = data.z * 16 + (k & 0xF0) >> 4;

                       k++;
                       int bx2 = data.x * 16 + k & 0x0F;
                       int by2 = i * 16 + k >> 8;
                       int bz2 = data.z * 16 + (k & 0xF0) >> 4;
                       blocks[bx, by, bz] = new Block(bx, by, bz, bid, 0);
                   }
               }
             }
        }
    }
}
