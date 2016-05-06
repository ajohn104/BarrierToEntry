using UnityEngine;
using System.Collections;
using SixenseCore;

namespace BarrierToEntry
{
    public enum Controller
    {
        RIGHT, LEFT
    }

    public class Controls
    {
        
        public Device device;
        public Player owner;
        public Tracker controllerLeft;      // Moved to Controls; used with player input
        public Tracker controllerRight;     // Moved to Controls; used with player input

        /* 
         * Note to self: careful with swapping hands, as the input 
         * controls (with the thumbsticks, buttons, and such) should
         * stay the same. Only the saber placement should change.
         * That's not to say that controls shouldn't be customizable.
        */

        public Controls(Device deviceBase, Player player)
        {
            this.owner = player;
            this.device = deviceBase;
            InputCheck();
        }

        public bool InputCheck()
        {
            if (!SixenseCore.Device.BaseConnected) return false;

            controllerLeft = SixenseCore.Device.GetTrackerByIndex(0);
            controllerRight = SixenseCore.Device.GetTrackerByIndex(1);
            return controllerLeft != null && controllerRight != null;
        }


        // Integrating this with Calibrate Shoulder instead.
        public bool Recenter {
            get { return controllerRight.GetButtonDown(Buttons.START); }
        }

        public bool CalibrateShoulder
        {
            get { return controllerLeft.GetButton(Buttons.BUMPER) && controllerRight.GetButton(Buttons.BUMPER); }
        }

        public bool CalibrateUserArmLength
        {
            get { return controllerLeft.GetButton(Buttons.JOYSTICK) && controllerRight.GetButton(Buttons.JOYSTICK); }
        }

        public bool ChangeBeamColorUp
        {
            get { return controllerRight.GetButtonDown(Buttons.X); }
        }

        public bool ChangeBeamColorDown
        {
            get { return controllerRight.GetButtonDown(Buttons.B); }
        }

        public Vector3 GetRealPosition(Controller side)
        {
            return _GetRealPosition((side == Controller.RIGHT) ? controllerRight : controllerLeft);
        }

        private Vector3 _GetRealPosition(Tracker con)
        {
            return con.Position * device.m_worldUnitScaleInMillimeters;
        }

        public bool DisplaySaberTransform
        {
            get { return controllerRight.GetButtonDown(Buttons.Y); }
        }

        public bool StopTime
        {
            get { return controllerRight.GetButtonDown(Buttons.A); }
        }

        public bool Grab
        {
            get { return controllerLeft.GetButtonDown(Buttons.TRIGGER); }
        }

        public bool ReleaseGrab
        {
            get { return !controllerLeft.GetButton(Buttons.TRIGGER) && owner.InGrab; }
        }

        public bool ResetLevel
        {
            get { return controllerLeft.GetButtonDown(Buttons.START); }
        }

        public bool EndGame
        {
            get { return Input.GetKeyDown(KeyCode.Escape); }
        }

    }
}