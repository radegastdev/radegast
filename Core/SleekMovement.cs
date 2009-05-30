using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using OpenMetaverse;

namespace Radegast
{
    public class SleekMovement
    {
        private GridClient client;
        private Timer timer;
        private float angle;
        private Vector3 forward = new Vector3(1, 0, 0);
        private bool turningLeft = false;
        private bool turningRight = false;
        private bool movingForward = false;
        private bool movingBackward = false;

        public bool TurningLeft
        {
            get {
                return turningLeft;
            }
            set {
                turningLeft = value;
                if (value) {
                    client.Self.Movement.AutoResetControls = false;
                    timer_Elapsed(null, null);
                    timer.Enabled = true;
                } else {
                    timer.Enabled = false;
                    client.Self.Movement.TurnLeft = false;
                    client.Self.Movement.SendUpdate();
                    client.Self.Movement.AutoResetControls = true;
                }
            }
        }

        public bool TurningRight
        {
            get
            {
                return turningRight;
            }
            set
            {
                turningRight = value;
                if (value) {
                    client.Self.Movement.AutoResetControls = false;
                    timer_Elapsed(null, null);
                    timer.Enabled = true;
                } else {
                    timer.Enabled = false;
                    client.Self.Movement.TurnRight = false;
                    client.Self.Movement.SendUpdate();
                    client.Self.Movement.AutoResetControls = true;
                }
            }
        }

        public bool MovingForward
        {
            get
            {
                return movingForward;
            }
            set
            {
                movingForward = value;
                if (value) {
                    client.Self.Movement.AutoResetControls = false;
                    client.Self.Movement.AtPos = true;
                    client.Self.Movement.SendUpdate();
                } else {
                    client.Self.Movement.AtPos = false;
                    client.Self.Movement.SendUpdate();
                    client.Self.Movement.AutoResetControls = true;
                }
            }
        }

        public bool MovingBackward
        {
            get
            {
                return movingBackward;
            }
            set
            {
                movingBackward = value;
                if (value) {
                    client.Self.Movement.AutoResetControls = false;
                    client.Self.Movement.AtNeg = true;
                    client.Self.Movement.SendUpdate();
                } else {
                    client.Self.Movement.AtNeg = false;
                    client.Self.Movement.SendUpdate();
                    client.Self.Movement.AutoResetControls = true;
                }
            }
        }

        public SleekMovement(GridClient c)
        {
            client = c;
            angle = client.Self.Movement.BodyRotation.Z;
            timer = new System.Timers.Timer(250);
            timer.Elapsed +=new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (turningLeft) {
                client.Self.Movement.TurnLeft = true;
                angle += 0.2f;
                if (angle > 1.0f) {
                    angle = -1.0f;
                }
                client.Self.Movement.BodyRotation = new Quaternion(0, 0, angle);
                System.Console.WriteLine(client.Self.Movement.BodyRotation.ToString());
                client.Self.Movement.SendUpdate(true);
            } else if (turningRight) {
                client.Self.Movement.TurnRight = true;
                angle -= 0.2f;
                if (angle < -1.0f) {
                    angle = 1.0f;
                }
                client.Self.Movement.BodyRotation = new Quaternion(0, 0, angle);
                System.Console.WriteLine(client.Self.Movement.BodyRotation.ToString());
                client.Self.Movement.SendUpdate(true);
            }
        }


    }
}
