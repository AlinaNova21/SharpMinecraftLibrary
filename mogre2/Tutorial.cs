using Mogre;
using Mogre.TutorialFramework;
using System;



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
            node.AttachObject(ent);
        }
    }
}