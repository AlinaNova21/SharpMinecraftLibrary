using Mogre;
using System;
using MOIS;
using MinecraftLibrary;



namespace Mogre.Tutorials
{
    public class CameraMan
    {
        private Camera mCamera;
        private bool mGoingForward;
        private bool mGoingBack;
        private bool mGoingRight;
        private bool mGoingLeft;
        private bool mGoingUp;
        private bool mGoingDown;
        private bool mFastMove;
        private bool mFreeze;
        private MinecraftLibrary.Client mClient;



        public CameraMan(Camera camera,MinecraftLibrary.Client client)
        {
            mCamera = camera;
            mClient = client;
        }

        public bool GoingForward
        {
            set { mGoingForward = value; }
            get { return mGoingForward; }
        }

        public bool GoingBack
        {
            set { mGoingBack = value; }
            get { return mGoingBack; }
        }

        public bool GoingLeft
        {
            set { mGoingLeft = value; }
            get { return mGoingLeft; }
        }

        public bool GoingRight
        {
            set { mGoingRight = value; }
            get { return mGoingRight; }
        }

        public bool GoingUp
        {
            set { mGoingUp = value; }
            get { return mGoingUp; }
        }

        public bool GoingDown
        {
            set { mGoingDown = value; }
            get { return mGoingDown; }
        }

        public bool FastMove
        {
            set { mFastMove = value; }
            get { return mFastMove; }
        }

        public bool Freeze
        {
            set { mFreeze = value; }
            get { return mFreeze; }
        }

        public void UpdateCamera(float timeFragment)
        {
            if (mFreeze)
                return;

            // build our acceleration vector based on keyboard input composite
            var move = Vector3.ZERO;
            if (mGoingForward) move += mCamera.Direction;
            if (mGoingBack) move -= mCamera.Direction;
            if (mGoingRight) move += mCamera.Right;
            if (mGoingLeft) move -= mCamera.Right;
            if (mGoingUp) move += mCamera.Up;
            if (mGoingDown) move -= mCamera.Up;

            move.Normalise();
            //move *= 3; // Natural speed is 150 units/sec.
            if (mFastMove)
                move *= 3; // With shift button pressed, move twice as fast.

            if (move != Vector3.ZERO)
                mCamera.Move(move * timeFragment);

            Client mc = mClient;
            mc.x = mCamera.Position.x;
            mc.y = mCamera.Position.y - 3;// -2;
            mc.z = mCamera.Position.z;
            mc.stance = mc.y + 1;
            mc.pitch = mCamera.Orientation.Pitch.ValueDegrees;
            if (mc.pitch > 50)
                mc.pitch = 50;
            if (mc.pitch < -90)
                mc.pitch = -90;
            mc.yaw = 180 + 360 - mCamera.Orientation.Yaw.ValueDegrees;
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

        public void MouseMovement(int x, int y)
        {
            if (mFreeze)
                return;

            mCamera.Yaw(new Degree(-x * 0.15f));
            mCamera.Pitch(new Degree(-y * 0.15f));
        }
    }
}