using Mogre;
using Mogre.TutorialFramework;
using System;
using MinecraftLibrary;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;



namespace Mogre.Tutorials
{
    class Tutorial : BaseApplication
    {
        static void writeDebug(string txt, bool show = false)
        {
            Debug.WriteLine(txt);
            if (show)
                wl(txt);
        }
        static void wl(string txt)
        {
            Console.WriteLine(txt);
            Debug.WriteLine(txt);
        }
        static void w(string txt)
        {
            Console.Write(txt);
            Debug.Write(txt);
        }
        public void packetHandler(object sender,Client.packetReceivedEventArgs e)
        {
            switch (e.ID)
            {
                case 0x0D:
                    //moveCamera((float)mc.x, (float)mc.y, (float)mc.z);
                    break;
                case 8:
                    Packet_UpdateHealth h = (Packet_UpdateHealth)e.packet;
                    Console.WriteLine("Health Update: {0} {1}", h.health, h.food);
                    if (h.health <= 0)
                    {
                        Packet_Respawn r = new Packet_Respawn();
                        r.dim = 0;
                        r.difficulty = 1;
                        r.creative = 0;
                        r.levelType = "default";
                        r.worldHeight = 256;
                        mc.sendPacket(r);
                    }
                    break;
                case 0x32:
                    Packet_PreChunk c = (Packet_PreChunk)e.packet;
                    if (c.mode)
                    {
                        if (!chunks.ContainsKey(c.x + "_" + c.z))
                            chunks.Add(c.x + "_" + c.z, new Chunk());
                    }
                    else
                        if (chunks.ContainsKey(c.x + "_" + c.z))
                            chunks.Remove(c.x + "_" + c.z);
                    break;
                case 0x33:
                    Packet_MapChunk mch = (Packet_MapChunk)e.packet;
                    int cx, cz;
                    cx = mch.x;
                    cz = mch.z;
                    string key = cx + "_" + cz;
                    //output("Chunk: " + key, true);
                    if (!chunks.ContainsKey(key))
                    {
                        chunks.Add(key, new Chunk());
                        //output("Chunk had to be added! " + key, true);
                    }
                    chunks[key].update(mch);
                    renderChunk(chunks[key]);
                    /*
                    byte[] cubic_chunk_data = new byte[4096];
                    byte[] cubic_chunk_data2 = new byte[4096 * 16];
                    //Loop over 16x16x16 chunks in the 16x256x16 column
                    int ii = -1;
                    for (int i = 0; i < 16; i++)
                    {
                        //If the bitmask indicates this chunk has been sent...
                        ii = ii + 1;
                        //Read data...
                        //Array.ConstrainedCopy(data.chunkData, 4096 * ii, cubic_chunk_data, 0, 4096);
                        //io.Read(cubic_chunk_data,0,4096); //2048 for the other arrays, where you'll need to split the data

                        for (int j = 0; j < cubic_chunk_data.Length; j++)
                        {
                            int bx = j & 0x0F;
                            int by = i * 16 + j >> 8;
                            int bz = (j & 0xF0) >> 4;
                            if (chunks[key].blocks[bx, by, bz] == null)
                                chunks[key].blocks[bx, by, bz] = new Block(bx, by, bz, 0, 0);
                            cubic_chunk_data[j] = (byte)chunks[key].blocks[bx, by, bz].ID;
                        }
                        Array.Copy(cubic_chunk_data, 0, cubic_chunk_data2, 4096 * ii, 4096);
                    }
                     */
                    
                    //System.IO.File.WriteAllBytes(@"chunks\" + key + "_bm_" + mch.groundUC.ToString() + ".bin", BitConverter.GetBytes(256));
                    //System.IO.File.WriteAllBytes(@"chunks\" + key + ".bin", cubic_chunk_data2);
                    //for (int y = 0; y < 128; y++)
                    //    for (int x = 0; x < 16; x++)
                    //        for (int z = 0; z < 16; z++)

                    break;
            }
        }
        public Client mc = new Client();
        static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();
        public static Tutorial t=new Tutorial();
        public static void Main()
        {
            Thread renderThread = new Thread(() =>
            {
                t.Go();
            });
            Client mc = t.mc;
            renderThread.Start();
            mc.packetReceived += t.packetHandler;
            wl("Welcome to SharpMCLibrary");
            w("Please enter a name:");
            mc.name = Console.ReadLine();
            mc.output2 = writeDebug;
            //mc.connect("127.0.0.1", 25564); //SMPROXY
            mc.connect("127.0.0.1", 25565); //LOCAL
            //mc.connect("37.59.228.108", 25565); //mcags.com

            
            string tmp = "";
            while (!(tmp == ":exit" || tmp == ":quit"))
            {
                tmp = Console.ReadLine();
                if (tmp.StartsWith(":"))
                {
                    if (tmp == ":respawn")
                    {
                        Packet_Respawn r = new Packet_Respawn();
                        r.dim = 0;
                        r.difficulty = 1;
                        r.creative = 0;
                        r.levelType = "default";
                        r.worldHeight = 256;
                        mc.sendPacket(r);
                    }
                }else{
                    mc.sendPacket(new Packet_Chat() {dataString = tmp});
                }
            }
            mc.disconnect();
        }

        protected override void CreateScene()
        {
            mSceneMgr.AmbientLight = new ColourValue(1, 1, 1);
            //Entity ent = mSceneMgr.CreateEntity("Head", "ogrehead.mesh");
            //SceneNode node = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode");
            //node.AttachObject(ent);
            MaterialPtr mat = MaterialManager.Singleton.Create("BoxColor", "General", true );
	        Technique tech = mat.GetTechnique(0);
	        Pass pass = tech.GetPass(0);
	        TextureUnitState tex = pass.CreateTextureUnitState();
            tex.SetTextureName("sphax.jpg",TextureType.TEX_TYPE_2D);
            tex.NumMipmaps=4;
            tex.TextureAnisotropy=1;
            tex.SetTextureFiltering(FilterOptions.FO_POINT, FilterOptions.FO_POINT, FilterOptions.FO_POINT);

            /*
            foreach (string file in System.IO.Directory.GetFiles("chunks", "*_bm_true.bin"))
            {
                int x = int.Parse(file.Split('\\')[1].Split('_')[0]);
                int z = int.Parse(file.Split('\\')[1].Split('_')[1]);

                Chunk c = new Chunk();
                c.update(new Packet_MapChunk()
                {
                    x=x,z=z,
                    primaryBM = BitConverter.ToInt16(System.IO.File.ReadAllBytes(@"chunks\"+x.ToString()+"_"+z.ToString()+"_bm_True.bin"), 0),
                    chunkData = System.IO.File.ReadAllBytes(@"chunks\" + x.ToString() + "_" + z.ToString() + ".bin")
                });
                SceneNode cn = mSceneMgr.CreateSceneNode("Chunk_" + x + "_" + z);
                cn.SetPosition(x * 16, 0, z * 16);
                cn.AttachObject(chunkMesh(c));
                //-15,160"
                cn.Position+=new Vector3(15, 0, -160);
                mSceneMgr.RootSceneNode.AddChild(cn);
            }*/

            mCamera.SetPosition(15, 0, -160);
        }

        public void moveCamera(float x,float y, float z)
        {
            mCamera.SetPosition(x, y, z);
        }
        public void renderChunk(Chunk c)
        {
            string chunk = "Chunk_" + c.x + "_" + c.z;
            SceneNode root = (SceneNode)mSceneMgr.RootSceneNode;
            SceneNode cn;
            try
            {
                cn = (SceneNode)root.GetChild(chunk);
            }
            catch (Exception ex)
            {
                cn = mSceneMgr.RootSceneNode.CreateChildSceneNode(chunk);
                cn.SetPosition(c.x * 16, 0, c.z * 16);
            }
            cn.RemoveAndDestroyAllChildren();
            cn.AttachObject(chunkMesh(c));
        }

        ColourValue getColor(int ccc)
        {
            Color ret;
            switch (ccc)
            {
                case 0:
                    ret = Color.Transparent;
                    break;
                case 1://stone
                    ret = Color.Gray;
                    break;
                case 2://grass
                    ret = Color.Green;
                    break;
                case 3://dirt
                    ret = Color.Brown;
                    break;
                case 17: //wood
                    ret = Color.BurlyWood;
                    break;
                case 106: //vines
                case 18: //leaves
                    ret = Color.LightGreen;
                    break;
                default:
                    ret = Color.Transparent;
                    break;
            }
            return new ColourValue(ret.R,ret.G,ret.B,ret.A);
        }

        ManualObject chunkMesh(Chunk c)
        {
            Mogre.ManualObject MeshChunk = mSceneMgr.CreateManualObject("MeshManChunk" + c.x + "_" + c.z);

   
	        MeshChunk.Begin("BoxColor",RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
      
	        uint iVertex = 0;
	        Block Block;
            Block Block1;
            float bo = 1f / 16f;
            //Console.WriteLine(bo);
            float U1, U2, V1, V2;
	        for (int z = 0; z < 16; ++z)
	        {
		        for (int y = 0; y < 256; ++y)
		        {
			        for (int x = 0; x < 16; ++x)
			        {
                        Block = c.blocks[x, y, z];
                        if (Block == null) continue;
                        if (Block.ID == 0) continue;

                        //Compute the block's texture coordinates
                        U1 = bo * (float)(Block.U());
                        U2 = U1 + bo;
                        V1 = bo * (float)(Block.V());
                        V2 = V1 + bo;
                        //x-1

                        Block1 = new Block(x, y, z, 0, 0);
				        if (x > 0) Block1 = c.blocks[x-1,y,z];

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x, y,   z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x, y+1, z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x, y+1, z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U2, V1);
					        MeshChunk.Position(x, y,   z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U1, V1);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
					        MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);

					        iVertex += 4;
				        }

                        //x+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (x < 16 - 1) Block1 = c.blocks[x + 1, y, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x+1, y,   z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y+1, z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U2, V1);
					        MeshChunk.Position(x+1, y,   z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U1, V1);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);
 
					        iVertex += 4;
				        }

                        //y-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y > 0) Block1 = c.blocks[x, y - 1, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U1,V1);

					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);
 
					        iVertex += 4;
				        }


                        //y+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y < 256 - 1) Block1 = c.blocks[x, y + 1, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U1,V1);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);
 
					        iVertex += 4;
				        }

                        //z-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (z > 0) Block1 = c.blocks[x, y, z - 1];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U1,V1);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);
 
					        iVertex += 4;
				        }
 
 
					        //z+1
                        Block1 = new Block(x,y,z,0,0);
                        if (z < 16 - 1) Block1 = c.blocks[x, y, z + 1];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U1,V1);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);
 
					        iVertex += 4;
				        }
			        }
		        }
	        }
 
	        MeshChunk.End();
            return MeshChunk;
        }
    }
}