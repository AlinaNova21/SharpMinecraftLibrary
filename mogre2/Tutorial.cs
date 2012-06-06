using Mogre;
using Mogre.TutorialFramework;
using System;
using MinecraftLibrary;
using System.Drawing;



namespace Mogre.Tutorials
{
    class Tutorial : BaseApplication
    {
        public static void Main()
        {
            new Tutorial().Go();
        }

        protected override void CreateScene()
        {
            mSceneMgr.AmbientLight = new ColourValue(1, 1, 1);
            Entity ent = mSceneMgr.CreateEntity("Head", "ogrehead.mesh");
            SceneNode node = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode");
            //node.AttachObject(ent);
            Chunk c=new Chunk();
            foreach (string file in System.IO.Directory.GetFiles("chunks", "*_bm_true.bin"))
            {
                int x = int.Parse(file.Split('\\')[1].Split('_')[0]);
                int z = int.Parse(file.Split('\\')[1].Split('_')[1]);

                c.update(new Packet_MapChunk()
                {
                    x=x,z=z,
                    primaryBM = BitConverter.ToInt16(System.IO.File.ReadAllBytes(@"chunks\"+x.ToString()+"_"+z.ToString()+"_bm_True.bin"), 0),
                    chunkData = System.IO.File.ReadAllBytes(@"chunks\" + x.ToString() + "_" + z.ToString() + ".bin")
                });
                SceneNode cn = mSceneMgr.CreateSceneNode("Chunk_" + x + "_" + z);
                cn.SetPosition(x * 16, 0, z * 16);
                cn.AttachObject(chunkMesh(c));
                mSceneMgr.RootSceneNode.AddChild(cn);
            }
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

   
	        MeshChunk.Begin("BoxColor");
            mes
	        uint iVertex = 0;
	        Block Block;
            Block Block1;
 
	        for (int z = 0; z < 16; ++z)
	        {
		        for (int y = 0; y < 256; ++y)
		        {
			        for (int x = 0; x < 16; ++x)
			        {
                        Block = c.blocks[x, y, z];
                        if (Block == null) continue;
                        if (Block.ID == 0) continue;

                        //x-1
                        Block1 = new Block(x, y, z, 0, 0);
				        if (x > 0) Block1 = c.blocks[x-1,y,z];

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x, y,   z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x, y+1, z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x, y+1, z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x, y,   z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
					        MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            
                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));

					        iVertex += 4;
				        }

                        //x+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (x < 16 - 1) Block1 = c.blocks[x + 1, y, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x+1, y,   z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x+1, y+1, z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x+1, y+1, z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x+1, y,   z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);

                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));
 
					        iVertex += 4;
				        }

                        //y-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y > 0) Block1 = c.blocks[x, y - 1, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x+1, y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x+1, y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x,   y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);

                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));
 
					        iVertex += 4;
				        }


                        //y+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y < 256 - 1) Block1 = c.blocks[x, y + 1, z];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);

                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));
 
					        iVertex += 4;
				        }

                        //z-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (z > 0) Block1 = c.blocks[x, y, z - 1];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x+1, y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x,   y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);

                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));
 
					        iVertex += 4;
				        }
 
 
					        //z+1
                        Block1 = new Block(x,y,z,0,0);
                        if (z < 16 - 1) Block1 = c.blocks[x, y, z + 1];
                        if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(0, 1);
					        MeshChunk.Position(x+1, y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(1, 1);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(1, 0);
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(0, 0);
 
					        MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            MeshChunk.Triangle(iVertex + 2, iVertex + 3, iVertex);

                            MeshChunk.Colour(getColor(Block1.ID));
                            MeshChunk.Colour(getColor(Block1.ID));
 
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