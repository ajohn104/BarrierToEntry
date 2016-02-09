using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class Controller
    {
        private Wiimote wiimote;
        public Vector3 position
        {
            get
            {
                return target.localPosition;
            }
            set
            {
                target.localPosition = value;
            }
        }
        
        public Vector3 rotation
        {
            get
            {
                return target.localEulerAngles;
            }
            /*set
            {
                target.localRotation = Quaternion.Euler(value);
            }*/
        }

        public ButtonData buttons
        {
            get
            {
                return wiimote.Button;
            }
        }

        private Vector3 velocity = Vector3.zero;
        private int index;

        public Controller(Wiimote remote, Transform trans, int i)
        {
            this.wiimote = remote;
            this.target = trans;
            lastRot = target.transform.eulerAngles;
            this.index = i;
        }

        public void SetLED(byte num)
        {
            this.wiimote.SendPlayerLED(num == 1, num == 2, num == 3, num == 4); // For now, I'm sticking to a simple approach. Nothing fancy needed.
        }

        private static Vector3 initOff = Vector3.zero;
        private Vector3 wmpoffset = initOff;
        private Vector3 lastAccNoGrav {
            get
            {
                Vector3 apprxGrav = Quaternion.Euler(new Vector3(wmpoffset[0], -wmpoffset[2], -wmpoffset[1])) * Vector3.down;
                Vector3 accel = GetNonNormalAccelVector();


                Vector3 accelNoGrav = accel - apprxGrav;
                float scale = 100f;
                return new Vector3(Mathf.Round(accelNoGrav[0] * scale) / scale, Mathf.Round(accelNoGrav[1] * scale) / scale, Mathf.Round(accelNoGrav[2] * scale) / scale);
            }
        }

        private Vector3 vel = Vector3.zero;
        private Transform target;
        
        private bool requestedWMP = false;
        private bool activatedWMP = false;
        private bool setupCamera = false;

        private bool calib1 = false;
        private bool calib2 = false;
        private bool calib3 = false;

        private ButtonState currentState = new ButtonState();
        private ButtonState prevState = new ButtonState();

        public bool allowTracking
        {
            get
            {
                return calib1 && calib2 && calib3;
            }
        }

        private Vector3 lastRot;

        public void UpdateData()
        {
            //if (Vector3.Magnitude(target.eulerAngles - lastRot) > 10) Debug.Log("large change: " + Vector3.Magnitude(target.eulerAngles - lastRot));
            lastRot = target.eulerAngles;
            prevState = new ButtonState(currentState);
            int ret;
            Vector3 totalOffset = new Vector3(wmpoffset.x, wmpoffset.y, wmpoffset.z);
            do
            {
                ret = wiimote.ReadWiimoteData();
                currentState = new ButtonState(wiimote.Button);
                if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS)
                {
                    Vector3 offset = new Vector3(Mathf.Round(-wiimote.MotionPlus.PitchSpeed * 100f) / 100f,
                                                    Mathf.Round(wiimote.MotionPlus.RollSpeed * 100f) / 100f,
                                                    Mathf.Round(-wiimote.MotionPlus.YawSpeed * 100f) / 100f) / 95f; // Divide by 95Hz (average updates per second from wiimote)
                    //Debug.Log("recent offset: " + offset);
                    totalOffset += offset;
                    wmpoffset += offset;
                    //if(index == 1) Debug.Log("index: " + this.index + ", " + offset);
                    //if (Input.GetKeyDown(KeyCode.LeftControl)) Debug.Log("above meee!!!");
                    if (allowTracking)
                    {
                        target.Rotate(offset, Space.Self);
                    }
                    if (calib1 && calib2 && calib3)
                    {
                        if (wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && ControlManager.lockPosition)
                        {
                            vel = Vector3.zero;
                        }
                        // point wiimote at screen, with wiimote "A" facing upwards, then press wiimote "B" to recalibrate position only
                        if (wiimote.Button.d_down && wiimote.current_ext == ExtensionController.MOTIONPLUS)
                        {
                            ResetData();
                        }
                        vel += Time.deltaTime * lastAccNoGrav;
                        
                        float resistance = Vector3.Distance(target.transform.localPosition, Vector3.zero);
                        Vector3 pressure = -target.transform.localPosition;
                        pressure *= 1f;

                        if (!(wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && ControlManager.lockPosition))
                        {
                            target.transform.localPosition += Time.deltaTime * (new Vector3(vel[0], vel[1], vel[2]) + pressure * resistance);
                        }
                        if (Vector3.Distance(target.transform.localPosition, Vector3.zero) > 3f)
                        {
                            target.transform.localPosition = Vector3.zero;
                        }
                    }
                }
            } while (ret > 0);
            totalOffset -= wmpoffset;
            //Debug.Log("total offset: " + totalOffset);
            //if(Vector3.Magnitude(totalOffset) > 1) Debug.Log("total offset: " + totalOffset);
            if (!requestedWMP)
            {
                wiimote.RequestIdentifyWiiMotionPlus();
                requestedWMP = true;
                return;
            }

            if (!activatedWMP)
            {
                wiimote.ActivateWiiMotionPlus();
                activatedWMP = true;
                return;
            }

            if (!setupCamera)
            {
                wiimote.SetupIRCamera(IRDataType.BASIC);
                setupCamera = true;
                return;
            }
            
            if (calib1 && calib2 && calib3)
            {
                float[] acc = wiimote.Accel.GetCalibratedAccelData();
                Vector3 accel = new Vector3(acc[0], acc[2], acc[1]);
                Vector3 accelG = Vector3.down;
            }



            // place wiimote on level surface, pointing at screen with wiimote "A" button up, then press keyboard "space" to reset and calibrate position and Wii Motion Plus
            if (Input.GetKeyDown(KeyCode.Space) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                MotionPlusData data = wiimote.MotionPlus;
                data.SetZeroValues();
                ResetData();
                //lastAccNoGrav = Vector3.zero;
                vel = Vector3.zero;
                //allowTracking = true;
            }

            // point wiimote at screen, with wiimote "A" facing upwards, then press wiimote "B" to recalibrate position only
            if (wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && !ControlManager.lockPosition)
            {
                ResetData();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
                calib1 = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.LEFT_SIDE_UP);
                calib2 = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.EXPANSION_UP);
                calib3 = true;
            }
        }
        public Vector3 GetAccelVector()
        {
            float accel_x;
            float accel_y;
            float accel_z;

            float[] accel = wiimote.Accel.GetCalibratedAccelData();
            accel_x = accel[0];
            accel_y = -accel[2];
            accel_z = accel[1];

            return new Vector3(accel_x, accel_y, accel_z).normalized;
        }

        private Vector3 GetNonNormalAccelVector()
        {
            float accel_x;
            float accel_y;
            float accel_z;

            float[] accel = wiimote.Accel.GetCalibratedAccelData();
            accel_x = accel[0];
            accel_y = -accel[2];
            accel_z = accel[1];

            return new Vector3(accel_x, accel_y, accel_z);
        }

        public void ResetData()
        {
            Debug.Log("data reset");
            target.rotation = Quaternion.FromToRotation(target.rotation * GetAccelVector(), Vector3.up) * target.rotation;
            target.rotation = Quaternion.FromToRotation(target.forward, Vector3.forward) * target.rotation;
            target.Rotate(new Vector3(90, 0, 0));
            target.localPosition = Vector3.zero;
            wmpoffset = initOff;
        }

        public byte getBattery()
        {
            return wiimote.Status.battery_level;
        }

        public enum Button { D_RIGHT, D_LEFT, D_UP, D_DOWN, A, B, ONE, TWO, PLUS, MINUS, HOME };

        public bool getButton(Button button)
        {
            switch (button) {
                case Button.D_RIGHT:
                    return currentState.d_right;
                case Button.D_LEFT:
                    return currentState.d_left;
                case Button.D_UP:
                    return currentState.d_up;
                case Button.D_DOWN:
                    return currentState.d_down;
                case Button.A:
                    return currentState.a;
                case Button.B:
                    return currentState.b;
                case Button.ONE:
                    return currentState.one;
                case Button.TWO:
                    return currentState.two;
                case Button.PLUS:
                    return currentState.plus;
                case Button.MINUS:
                    return currentState.minus;
                case Button.HOME:
                    return currentState.home;
                default:
                    return false;
            }
        }

        private bool getButtonDown(bool current, bool prev)
        {
            return (current != prev) && current;
        }

        private bool getButtonUp(bool current, bool prev)
        {
            return (current != prev) && !current;
        }

        public bool getButtonDown(Button button)
        {
            switch (button)
            {
                case Button.D_RIGHT:
                    return getButtonDown(currentState.d_right, prevState.d_right);
                case Button.D_LEFT:
                    return getButtonDown(currentState.d_left, prevState.d_left);
                case Button.D_UP:
                    return getButtonDown(currentState.d_up, prevState.d_up);
                case Button.D_DOWN:
                    return getButtonDown(currentState.d_down, prevState.d_down);
                case Button.A:
                    return getButtonDown(currentState.a, prevState.a);
                case Button.B:
                    return getButtonDown(currentState.b, prevState.b);
                case Button.ONE:
                    return getButtonDown(currentState.one, prevState.one);
                case Button.TWO:
                    return getButtonDown(currentState.two, prevState.two);
                case Button.PLUS:
                    return getButtonDown(currentState.plus, prevState.plus);
                case Button.MINUS:
                    return getButtonDown(currentState.minus, prevState.minus);
                case Button.HOME:
                    return getButtonDown(currentState.home, prevState.home);
                default:
                    return false;
            }
        }

        public bool getButtonUp(Button button)
        {
            switch (button)
            {
                case Button.D_RIGHT:
                    return getButtonUp(currentState.d_right, prevState.d_right);
                case Button.D_LEFT:
                    return getButtonUp(currentState.d_left, prevState.d_left);
                case Button.D_UP:
                    return getButtonUp(currentState.d_up, prevState.d_up);
                case Button.D_DOWN:
                    return getButtonUp(currentState.d_down, prevState.d_down);
                case Button.A:
                    return getButtonUp(currentState.a, prevState.a);
                case Button.B:
                    return getButtonUp(currentState.b, prevState.b);
                case Button.ONE:
                    return getButtonUp(currentState.one, prevState.one);
                case Button.TWO:
                    return getButtonUp(currentState.two, prevState.two);
                case Button.PLUS:
                    return getButtonUp(currentState.plus, prevState.plus);
                case Button.MINUS:
                    return getButtonUp(currentState.minus, prevState.minus);
                case Button.HOME:
                    return getButtonUp(currentState.home, prevState.home);
                default:
                    return false;
            }
        }

        class ButtonState
        {
            /// Button: D-Pad Left
            public bool d_left { get { return _d_left; } }
            private bool _d_left;
            /// Button: D-Pad Right
            public bool d_right { get { return _d_right; } }
            private bool _d_right;
            /// Button: D-Pad Up
            public bool d_up { get { return _d_up; } }
            private bool _d_up;
            /// Button: D-Pad Down
            public bool d_down { get { return _d_down; } }
            private bool _d_down;
            /// Button: A
            public bool a { get { return _a; } }
            private bool _a;
            /// Button: B
            public bool b { get { return _b; } }
            private bool _b;
            /// Button: 1 (one)
            public bool one { get { return _one; } }
            private bool _one;
            /// Button: 2 (two)
            public bool two { get { return _two; } }
            private bool _two;
            /// Button: + (plus)
            public bool plus { get { return _plus; } }
            private bool _plus;
            /// Button: - (minus)
            public bool minus { get { return _minus; } }
            private bool _minus;
            /// Button: Home
            public bool home { get { return _home; } }
            private bool _home;

            public ButtonState()
            {

            }

            public ButtonState(ButtonData data)
            {
                ReadData(data);
            }

            public ButtonState(ButtonState data)
            {
                ReadData(data);
            }

            public void ReadData(ButtonData data)
            {
                _d_left = data.d_left;
                _d_right = data.d_right;
                _d_up = data.d_up;
                _d_down = data.d_down;
                _a = data.a;
                _b = data.b;
                _one = data.one;
                _two = data.two;
                _plus = data.plus;
                _minus = data.minus;
                _home = data.home;
            }

            public void ReadData(ButtonState data)
            {
                _d_left = data.d_left;
                _d_right = data.d_right;
                _d_up = data.d_up;
                _d_down = data.d_down;
                _a = data.a;
                _b = data.b;
                _one = data.one;
                _two = data.two;
                _plus = data.plus;
                _minus = data.minus;
                _home = data.home;
            }
        }
    }
}