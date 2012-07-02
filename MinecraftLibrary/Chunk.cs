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
        public short ID = 0;
        public short metaData = 0;
        public int x = 0;
        public int y = 0;
        public int z = 0;
        public Block() { }
        public Block(int x, int y, int z, short ID, short metaData)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.ID = ID;
            this.metaData = metaData;
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
            t.Add(12, new Point(3, 1));
            t.Add(13, new Point(2, 1));
            t.Add(14, new Point(0, 2));
            t.Add(15, new Point(0, 2));
            t.Add(16, new Point(2, 2));
            t.Add(17, new Point(5, 1));
            t.Add(18, new Point(2, 4));
            t.Add(19, new Point(3, 0));
            t.Add(20, new Point(1, 3));
            t.Add(21, new Point(0, 11));
            t.Add(22, new Point(0, 9));
            t.Add(23, new Point(14, 2)); //Dispenser - Has More Than One Face
            t.Add(24, new Point(0, 2)); //Sandstone - Has More Than One Face
            t.Add(25, new Point(10, 4)); //NoteBlock - Has More Than One Face
            t.Add(26, new Point(7, 8)); //Bed - Has More Than One Face
            t.Add(27, new Point(3, 11)); //Powered Rails - Two Types Active/Non
            t.Add(28, new Point(3, 12));
            t.Add(29, new Point(10, 7)); //Sticky Piston - Has More Than One Face
            t.Add(30, new Point(11, 0));
            //t.Add(31, new Point(0, 0)); //Tall Grass - Isn't On Terrain.png
            t.Add(32, new Point(7, 3));
            t.Add(33, new Point(11, 7)); //Normal Piston - Has More Than One Face
            t.Add(34, new Point(11, 7)); //Piston Activated - Isn't On Terrain.png
            t.Add(35, new Point(4, 0));
            //t.Add(36, new Point(0, 0)); //Block Moved By Piston
            t.Add(37, new Point(12, 0));
            t.Add(38, new Point(13, 0));
            t.Add(39, new Point(13, 1));
            t.Add(40, new Point(12, 1));
            t.Add(41, new Point(7, 1));
            t.Add(42, new Point(6, 1));
            t.Add(43, new Point(5, 0)); //Double Stone Slab
            t.Add(44, new Point(6, 0)); //Single Stone Slap
            t.Add(45, new Point(7, 0));
            t.Add(46, new Point(8, 0)); //TNT My Faveourite - More Than One Side
            //t.Add(47, new Point(0, 0)); //Bookshelf - Not On Terrain.png
            t.Add(48, new Point(4, 2));
            t.Add(49, new Point(5, 2));
            t.Add(50, new Point(0, 5)); //Torch, Needs Special Rendering
            //t.Add(51, new Point(0, 0)); //Fire - Not On Terrain.png
            //t.Add(52, new Point(0, 0)); //Monster Spawner - Not On Terrain.png
            //t.Add(53, new Point(0, 0)); //Wooden Stairs - Not On Terrain.png
            t.Add(54, new Point(11, 1));
            //t.Add(55, new Point(0, 0)); //Redstone Wire - Not On Terrain.png
            t.Add(56, new Point(2, 3));
            t.Add(57, new Point(8, 1));
            t.Add(58, new Point(12, 3));
            t.Add(59, new Point(15, 5));
            //t.Add(60, new Point(0, 0));//'Farmland' - Don't Know What To Do!
            t.Add(61, new Point(12, 1));
            //t.Add(63, new Point(0, 0));//SignPost - Not On Terrain.png
            //t.Add(64, new Point(0, 0));//Door - Needs Rendering
            t.Add(65, new Point(3, 5));
            t.Add(66, new Point(0, 8));
            t.Add(67, new Point(6, 3));
            //t.Add(68, new Point(0, 0));//'WallSign' Needs Special Rendering
            t.Add(69, new Point(0, 6));
            //t.Add(70, new Point(0, 0));//StonePressurePlate - Not On Terrain.png
            //t.Add(71, new Point(0, 0));//Iron Door - Needs Special Rendering
            //t.Add(72, new Point(0, 0));//WoodenPressurePlate - Not On Terrain.png
            t.Add(73, new Point(3, 3));
            t.Add(74, new Point(3, 3));//Redstone Ore (Glowing Not On Terrain.png)
            t.Add(75, new Point(3, 7));
            t.Add(76, new Point(3, 6));
            //t.Add(77, new Point(0, 0));//Stone Button - Can't See On Terrain.png
            //t.Add(78, new Point(0, 0));//Snow - Needs Rendering
            t.Add(79, new Point(3, 4));
            t.Add(80, new Point(2, 4));
            t.Add(81, new Point(6, 4));
            t.Add(82, new Point(8, 4));
            t.Add(83, new Point(9, 4));
            t.Add(84, new Point(11, 4));//NoteBox - Needs Special Rendering
            //t.Add(85, new Point(0, 0));//Fence - Can't See On Terrain.png
            t.Add(86, new Point(7, 7));
            t.Add(87, new Point(7, 6));
            t.Add(88, new Point(8, 6));
            t.Add(89, new Point(9, 6));
            //t.Add(90, new Point(0, 0));//Nether Portal - Not On Terrain.png
            t.Add(91, new Point(8, 7));
            t.Add(92, new Point(10, 7));
            t.Add(93, new Point(3, 8));
            t.Add(94, new Point(3, 9));
            t.Add(95, new Point(0, 0));//'Locked Chesk' WTF? xD
            t.Add(96, new Point(4, 5));
            //t.Add(87, new Point(0 ,0));//'Monster Egg' CStone, Stone Or StoneSlabs
            t.Add(98, new Point(6, 3));
            //t.Add(99, new Point(0, 0));//'Huge Brown Mushroom'
            //t.Add(100, new Point(0, 0));//'Huge Red Mushroom'
            t.Add(101, new Point(5, 5));
            //t.Add(102, new Point(0, 0));//Glass Pane - Not On Terrain.png
            t.Add(103, new Point(8, 7));//Melon - Needs Rendering
            //t.Add(104, new Point(0, 0));//Pumpkin Stem - Not On Terrain.png
            //t.Add(105, new Point(0, 0));//Pumpkin Stem - Not On Terrain.png
            //t.Add(106, new Point(0, 0));//Vines - Isn't On Terrain.png
            //t.Add(107, new Point(0, 0));//Fence Gate - Not On Terrain.png
            //t.Add(108, new Point(0, 0));//Brick Stairs - Not On Terrain.png
            t.Add(109, new Point(6, 3));
            t.Add(110, new Point(13, 4));
            t.Add(111, new Point(12, 4));//LillyPad - Not Sure If It's Correct S
            t.Add(112, new Point(0, 14));
            //t.Add(113, new Point(0 ,0));//Nether Brick Fence - Not On Terrain.png
            //t.Add(114, new Point(0, 0));//Nether Brick Stairs - Not On Terrain.png
            t.Add(115, new Point(3, 14));//Nether Wart - Render - 3 Stages
            t.Add(116, new Point(6, 11));//Enchantment Table - Needs Rendering
            t.Add(117, new Point(13, 9));//Brewing Stand - Needs Rendering
            //t.Add(118, new Point(0, 0));//Cauldron - Can't Choose Which One
            //t.Add(119, new Point(0, 0));//End Portal - Not On Terrain.png
            t.Add(120, new Point(15, 9));//End Frame - Needs Rendering
            t.Add(121, new Point(15, 10));
            //t.Add(122, new Point(0, 0));//Ender Dragon Egg - Not On Terrain.png
            t.Add(123, new Point(3, 13));
            t.Add(124, new Point(4, 13));
            t.Add(125, new Point(4, 0));
            //t.Add(126, new Point(0, 0));//Wooden Slab - Not On Terrain.png - Render

            if (t.ContainsKey(ID))
                return t[ID];
            return new Point();
        }
        public bool isTransparent()
        {
            bool ret = false;
            if (ID == 50)
                ret = true;
            return ret;
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

        public Chunk()
        {
            for (int z = 0; z < 16; ++z)
                for (int y = 0; y < 256; ++y)
                    for (int x = 0; x < 16; ++x)
                    {
                        blocks[x, y, z] = new Block(x, y, z, 0, 0);
                    }
        }

        public void update(Packet_MultiBlockChange data)
        {
            int[] d = data.Rawdata;
            foreach (int bl in d)
            {
                int metadata, x, y, z, bid;
                metadata = bl & 0x3;
                bid = (bl & 0xFFF0) >> 4;
                y = (bl & 0xFF0000) >> 16;
                z = (bl & 0xF000000) >> 24;
                x = (int)(bl & (uint)0xF0000000) >> 28;
                //Block b=blocks[x,y,z];
                //b.metaData = (short)metadata;
                //b.ID = (short)bid;
            }
        }
        public void update(Packet_MapChunk data)
        {
            x = data.X;
            z = data.Z;
            byte[] cubic_chunk_data = new byte[4096];
            //Loop over 16x16x16 chunks in the 16x256x16 column
            int ii = -1;
            for (int i = 0; i < 16; i++)
            {
                //If the bitmask indicates this chunk has been sent...
                if ((data.PrimaryBM & (1 << i)) != 0)
                {
                    ii = ii + 1;
                    //Read data...
                    Array.ConstrainedCopy(data.ChunkData, 4096 * ii, cubic_chunk_data, 0, 4096);

                    for (int j = 0; j < cubic_chunk_data.Length; j++)
                    {
                        //Retrieve x,y,z and data from each element in cubic_chunk_array

                        //Byte arrays
                        //int bx = data.x * 16 + j & 0x0F;
                        //int by = i * 16 + j >> 8;
                        //int bz = data.z * 16 + (j & 0xF0) >> 4;
                        int bx = j & 0x0F;
                        int by = (i * 16) + (j >> 8);
                        int bz = (j & 0xF0) >> 4;
                        short bid = cubic_chunk_data[j];

                        //Nibble arrays
                        int data1 = cubic_chunk_data[j] & 0x0F;
                        int data2 = cubic_chunk_data[j] >> 4;

                        int k = 2 * j;
                        int bx1 = data.X * 16 + k & 0x0F;
                        int by1 = i * 16 + k >> 8;
                        int bz1 = data.Z * 16 + (k & 0xF0) >> 4;

                        k++;
                        int bx2 = data.X * 16 + k & 0x0F;
                        int by2 = i * 16 + k >> 8;
                        int bz2 = data.Z * 16 + (k & 0xF0) >> 4;
                        blocks[bx, by, bz] = new Block(bx, by, bz, bid, 0);
                    }
                }
            }
        }
    }
}
