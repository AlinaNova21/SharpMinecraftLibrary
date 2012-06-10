using Mogre;
using Mogre.TutorialFramework;
using System;
using MinecraftLibrary;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Yaml;
using MOIS;



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
                    Packet_PlayerPosAndLook p = (Packet_PlayerPosAndLook)e.packet;
                    moveCamera((float)p.x, (float)p.y+3f, (float)p.z);
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
                    QueueChunk(chunks[key]);
                    break;
                case 0x34:
                    Packet_MultiBlockChange mb = (Packet_MultiBlockChange)e.packet;
                    chunks[mb.x + "_" + mb.z].update(mb);
                    QueueChunk(chunks[mb.x + "_" + mb.z]);
                    break;
            }
        }
        public void QueueChunk(Chunk c)
        {
            lock(chunkq)
                chunkq.Enqueue(c);
            
        }
        public Client mc = new Client();
        static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();
        static Dictionary<string, SceneNode> chunksm = new Dictionary<string,SceneNode>();
        static Queue<Chunk> chunkq = new Queue<Chunk>();

        public static Tutorial t= new Tutorial();

        public static void outputNode(YamlNode node,string ind="")
        {
            Console.WriteLine(ind+"NODE:\n{0}", ind+node.Tag);
            if (node.GetType() == typeof(YamlMapping))
                foreach (YamlNode n in ((YamlMapping)node).Keys)
                {
                    outputNode(n,ind+"-");
                }
            else if (node.GetType() == typeof(YamlScalar))
                Console.WriteLine("{0}\n", ind+((YamlScalar)node));
        }
        public static void Main()
        {
            /*
            //Yaml.Node n = Yaml.Node.FromFile("minecraft.yaml");
            //Console.WriteLine(n);
            //Console.ReadLine();
            YamlMapping ns = (YamlMapping)YamlNode.FromYamlFile("minecraft.yaml")[0];
            outputNode(ns);

            YamlSequence m;
            YamlNode n;
            ns.TryGetValue("blocks", out n);
            m = (YamlSequence)n;
            foreach(YamlNode i in m)
                foreach (YamlNode ii in ((YamlMapping)i).Values)
                    Console.WriteLine(ii.Tag);

            Console.ReadLine();
            return;*/
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
            //mc.connect("127.0.0.1", 25565); //LOCAL
            mc.connect("37.59.228.108", 25565); //mcags.com

            
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
        CameraMan mCamMan;
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
            tex.NumMipmaps=0;
            tex.TextureAnisotropy=0;
            tex.SetTextureFiltering(FilterOptions.FO_POINT, FilterOptions.FO_POINT, FilterOptions.FO_POINT);
            //pass.DepthWriteEnabled=false;
            //pass.SetSceneBlending(SceneBlendType.SBT_TRANSPARENT_ALPHA);
            //pass.CullingMode = CullingMode.CULL_NONE;
            //mCamMan = new Tutorials.CameraMan(mCamera,mc);
            //mCameraMan = null;
            mCamera.SetPosition((float)mc.x, (float)mc.y, (float)mc.z);
            mCamera.Pitch(new Degree(mc.pitch).ValueRadians);
            mCamera.Yaw(new Degree(mc.yaw).ValueRadians);
            oldCamPos = mCamera.Position;
        }
        Vector3 oldCamPos;
        Quaternion oldCamOr;
        float camPosCnt = 0;
        protected override void UpdateScene(FrameEvent evt)
        {
            base.UpdateScene(evt);
        }

        bool ProcessChunks(FrameEvent evt)
        {
            lock (chunkq)
                while (chunkq.Count > 0)
                    renderChunk(chunkq.Dequeue());
            return true;
        }
        bool ProcessCamera(FrameEvent evt)
        {
            //camPosCnt += evt.timeSinceLastFrame;
            if (false) //camPosCnt > .5f && (!mCamera.Orientation.Equals(oldCamOr) || !mCamera.Position.PositionEquals(oldCamPos)))
            {
                //Console.WriteLine(oldCamPos.ToString());
                //Console.WriteLine(mCamera.Position.ToString());
                camPosCnt = 0;
                oldCamPos = mCamera.Position;
                oldCamOr = mCamera.Orientation;
                mc.x = oldCamPos.x;
                mc.y = oldCamPos.y - 3;// -2;
                mc.z = oldCamPos.z;
                mc.stance = mc.y + 1;
                mc.pitch = oldCamOr.Pitch.ValueDegrees;
                if (mc.pitch > 50)
                    mc.pitch = 50;
                if (mc.pitch < -90)
                    mc.pitch = -90;
                mc.yaw = 180 + 360 - oldCamOr.Yaw.ValueDegrees;
                mc.sendPacket(new Packet_PlayerPosAndLook()
                {
                    x = mc.x,
                    y = mc.y,//-2f,
                    z = mc.z,
                    pitch = mc.pitch,//mCamera.Orientation.Pitch.ValueDegrees,
                    yaw = mc.yaw,
                    stance = mc.stance
                });
            }
            return true;
        }

        protected override void CreateFrameListeners()
        {
            base.CreateFrameListeners();
            mRoot.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(ProcessCamera);
            mRoot.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(ProcessChunks);
        }

        public void moveCamera(float x,float y, float z)
        {
            mCamera.SetPosition(x, y, z);
            oldCamPos = mCamera.Position;
        }
        public void renderChunk(Chunk c)
        {
            string chunk = "Chunk_" + c.x + "_" + c.z;
            SceneNode root = (SceneNode)mSceneMgr.RootSceneNode;
            SceneNode cn;
            if(chunksm.ContainsKey(chunk))
            {
                cn = (SceneNode)root.GetChild(chunk);
                mSceneMgr.DestroyManualObject("MeshManChunk" + c.x + "_" + c.z);
                cn.RemoveAndDestroyAllChildren();
            }else{
                cn = mSceneMgr.RootSceneNode.CreateChildSceneNode(chunk);
                cn.SetPosition(c.x * 16, 0, c.z * 16);
                chunksm.Add(chunk,cn);
            }
            
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
                        //if (x == 0) Block1 = chunks[(c.x - 1) + "_" + c.z].blocks[15, y, z];

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x, y,   z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x, y+1, z+1);	MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U2, V1);
					        MeshChunk.Position(x, y+1, z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U1, V1);
					        MeshChunk.Position(x, y,   z);		MeshChunk.Normal(-1,0,0);	MeshChunk.TextureCoord(U1, V2);
 
					        //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
					        //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3);

					        iVertex += 4;
				        }

                        //x+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (x < 15) Block1 = c.blocks[x + 1, y, z];
                        //if (x == 15) Block1 = chunks[(c.x + 1) + "_" + c.z].blocks[0, y, z];

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x+1, y,   z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U2, V1);
					        MeshChunk.Position(x+1, y+1, z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U1, V1);
					        MeshChunk.Position(x+1, y,   z+1);	MeshChunk.Normal(1,0,0); MeshChunk.TextureCoord(U1, V2);

                            //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3);
 
					        iVertex += 4;
				        }

                        //y-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y > 0) Block1 = c.blocks[x, y - 1, z];

                        //if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y, z);		MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y, z+1);	MeshChunk.Normal(0,-1,0); MeshChunk.TextureCoord(U1,V1);

                            //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3);
 
					        iVertex += 4;
				        }


                        //y+1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (y < 256 - 1) Block1 = c.blocks[x, y + 1, z];
                        //if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,1,0); MeshChunk.TextureCoord(U1,V1);

                            //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3); 
 
					        iVertex += 4;
				        }

                        //z-1
                        Block1 = new Block(x, y, z, 0, 0);
                        if (z > 0) Block1 = c.blocks[x, y, z - 1];
                        //if (x == 0) Block1 = chunks[c.x + "_" + (c.z - 1)].blocks[x, y, 15];
                        //if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U2, V1);
					        MeshChunk.Position(x+1, y+1, z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U1, V1);
					        MeshChunk.Position(x+1, y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x,   y,   z);		MeshChunk.Normal(0,0,-1); MeshChunk.TextureCoord(U2, V2);

                            //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3);
					        iVertex += 4;
				        }
 
 
					        //z+1
                        Block1 = new Block(x,y,z,0,0);
                        if (z < 15) Block1 = c.blocks[x, y, z + 1];
                        //if (x == 15) Block1 = chunks[c.x+"_"+(c.z + 1)].blocks[x, y, 0];

                        //if (Block1 == null) Block1 = new Block(x, y, z, 0, 0);

                        if (Block1.ID == 0)
				        {
					        MeshChunk.Position(x,   y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U1, V2);
					        MeshChunk.Position(x+1, y,   z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U2, V2);
					        MeshChunk.Position(x+1, y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U2,V1);
					        MeshChunk.Position(x,   y+1, z+1);		MeshChunk.Normal(0,0,1); MeshChunk.TextureCoord(U1,V1);

                            //MeshChunk.Triangle(iVertex, iVertex+1, iVertex+2);
                            //MeshChunk.Triangle(iVertex+2, iVertex+3, iVertex);
                            MeshChunk.Quad(iVertex, iVertex + 1, iVertex + 2, iVertex + 3);
 
					        iVertex += 4;
				        }
			        }
		        }
	        }
 
	        MeshChunk.End();
            return MeshChunk;
        }
        /*
        protected override bool OnKeyPressed(KeyEvent evt)
        {
            switch (evt.key)
            {
                case KeyCode.KC_W:
                case KeyCode.KC_UP:
                    mCamMan.GoingForward = true;
                    break;

                case KeyCode.KC_S:
                case KeyCode.KC_DOWN:
                    mCamMan.GoingBack = true;
                    break;

                case KeyCode.KC_A:
                case KeyCode.KC_LEFT:
                    mCamMan.GoingLeft = true;
                    break;

                case KeyCode.KC_D:
                case KeyCode.KC_RIGHT:
                    mCamMan.GoingRight = true;
                    break;

                case KeyCode.KC_E:
                case KeyCode.KC_PGUP:
                    mCamMan.GoingUp = true;
                    break;

                case KeyCode.KC_Q:
                case KeyCode.KC_PGDOWN:
                    mCamMan.GoingDown = true;
                    break;

                case KeyCode.KC_LSHIFT:
                case KeyCode.KC_RSHIFT:
                    mCamMan.FastMove = true;
                    break;

                case KeyCode.KC_T:
                    CycleTextureFilteringMode();
                    break;

                case KeyCode.KC_R:
                    //CyclePolygonMode();
                    break;

                case KeyCode.KC_F5:
                    ReloadAllTextures();
                    break;

                case KeyCode.KC_SYSRQ:
                    TakeScreenshot();
                    break;

                case KeyCode.KC_ESCAPE:
                    Shutdown();
                    break;
            }

            return true;
        }

        protected override bool OnKeyReleased(KeyEvent evt)
        {
            switch (evt.key)
            {
                case KeyCode.KC_W:
                case KeyCode.KC_UP:
                    mCamMan.GoingForward = false;
                    break;

                case KeyCode.KC_S:
                case KeyCode.KC_DOWN:
                    mCamMan.GoingBack = false;
                    break;

                case KeyCode.KC_A:
                case KeyCode.KC_LEFT:
                    mCamMan.GoingLeft = false;
                    break;

                case KeyCode.KC_D:
                case KeyCode.KC_RIGHT:
                    mCamMan.GoingRight = false;
                    break;

                case KeyCode.KC_E:
                case KeyCode.KC_PGUP:
                    mCamMan.GoingUp = false;
                    break;

                case KeyCode.KC_Q:
                case KeyCode.KC_PGDOWN:
                    mCamMan.GoingDown = false;
                    break;

                case KeyCode.KC_LSHIFT:
                case KeyCode.KC_RSHIFT:
                    mCamMan.FastMove = false;
                    break;
            }

            return true;
        }

        protected override bool OnMouseMoved(MouseEvent evt)
        {
            mCamMan.MouseMovement(evt.state.X.rel, evt.state.Y.rel);
            return true;
        }*/
    }
}