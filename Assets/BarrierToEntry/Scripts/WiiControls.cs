using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;

public class WiiControls : MonoBehaviour
{
    public GameObject saber;

    private Wiimote wiimote;

    private bool foundWiimote = false;
    private bool requestedWMP = false;
    private bool activatedWMP = false;
    private bool setupCamera = false;

    private bool calib1 = false;
    private bool calib2 = false;
    private bool calib3 = false;

    private float dx = 0;
    private float dy = 0;
    private float dz = 0;
    private Vector3 lastAccel = Vector3.zero;
    private Vector3 lastAccelG = Vector3.zero;
    private static Vector3 initOff = Vector3.zero;//new Vector3(-90, 0, 0);
    private Vector3 wmpoffset = initOff;
    private Vector3 lastAccNoGrav = Vector3.zero;
    private Vector3 initSaberOff = new Vector3(0.5395362f, -0.5596324f, 0.1693648f);
    private Vector3 vel = Vector3.zero;
    private bool debugPosition = false;


    void Start()
    {
        //saber = transform.Find("LSaber").gameObject;
        WiimoteManager.FindWiimotes();
    }
    bool allowSaber = false;
    void Update()
    {

        if (!foundWiimote)
        {
            if (!WiimoteManager.HasWiimote())
            {
                return;
            }
            wiimote = WiimoteManager.Wiimotes[0];
            foundWiimote = true;
            return;
        }

        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();

            if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                Vector3 offset = new Vector3(Mathf.Round(-wiimote.MotionPlus.PitchSpeed*100f)/100f,
                                                Mathf.Round(wiimote.MotionPlus.RollSpeed * 100f) / 100f,
                                                Mathf.Round(-wiimote.MotionPlus.YawSpeed * 100f) / 100f) / 95f; // Divide by 95Hz (average updates per second from wiimote)
                wmpoffset += offset;
                if (allowSaber)
                {
                    saber.transform.Rotate(offset, Space.Self);
                }
                if (calib1 && calib2 && calib3)
                {
                    if (wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && !debugPosition)
                    {
                        vel = Vector3.zero;
                    }
                    // point wiimote at screen, with wiimote "A" facing upwards, then press wiimote "B" to recalibrate position only
                    if (wiimote.Button.d_down && wiimote.current_ext == ExtensionController.MOTIONPLUS)
                    {
                        saber.transform.rotation = Quaternion.FromToRotation(saber.transform.rotation * GetAccelVector(), Vector3.up) * saber.transform.rotation;
                        saber.transform.rotation = Quaternion.FromToRotation(saber.transform.forward, Vector3.forward) * saber.transform.rotation;
                        saber.transform.Rotate(new Vector3(90, 0, 0));
                        saber.transform.localPosition = initSaberOff;
                        wmpoffset = initOff;
                    }
                    float slow = 0.8f;
                    //lastAccNoGrav *= slow;
                    vel += Time.deltaTime * lastAccNoGrav;
                    
                    //vel *= slow;
                    float resistance = Vector3.Distance(saber.transform.localPosition, initSaberOff);
                    Vector3 pressure = initSaberOff - saber.transform.localPosition;
                    pressure *= 1f;
                    //Debug.Log(resistance + ", " + pressure);

                    if (!(wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && !debugPosition))
                    {
                        saber.transform.localPosition += Time.deltaTime * (new Vector3(-vel[0] * 10, -vel[1] * 10, -vel[2]*2) + pressure * resistance);
                    }
                    if(Vector3.Distance(saber.transform.localPosition, initSaberOff) > 10f)
                    {
                        saber.transform.localPosition = initSaberOff;
                    }
                }
            }
        } while (ret > 0);

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

        // Face screen with Rift and press "A" to reset and calibrate the Oculus
        if (wiimote.Button.a)
        {
            VRCenter.Recenter();
        }

        //Debug.Log(wiimote.Accel.GetCalibratedAccelData()[0] + ", " + wiimote.Accel.GetCalibratedAccelData()[1] + ", " + wiimote.Accel.GetCalibratedAccelData()[2]);
        /*if(calib1 && calib2 && calib3 && (wiimote.Accel.GetCalibratedAccelData()[0]>1 || wiimote.Accel.GetCalibratedAccelData()[0] < -1))
        {
            Debug.Log("x over |1|");
            Debug.Log(wiimote.Accel.GetCalibratedAccelData()[0] + ", " + wiimote.Accel.GetCalibratedAccelData()[1] + ", " + wiimote.Accel.GetCalibratedAccelData()[2]);
        }
        if (calib1 && calib2 && calib3 && (wiimote.Accel.GetCalibratedAccelData()[1] > 1 || wiimote.Accel.GetCalibratedAccelData()[1] < -1))
        {
            Debug.Log("y over |1|");
            Debug.Log(wiimote.Accel.GetCalibratedAccelData()[0] + ", " + wiimote.Accel.GetCalibratedAccelData()[1] + ", " + wiimote.Accel.GetCalibratedAccelData()[2]);
        }
        if (calib1 && calib2 && calib3 && (wiimote.Accel.GetCalibratedAccelData()[2] > 1 || wiimote.Accel.GetCalibratedAccelData()[2] < -1))
        {
            Debug.Log("z over |1|");
            Debug.Log(wiimote.Accel.GetCalibratedAccelData()[0] + ", " + wiimote.Accel.GetCalibratedAccelData()[1] + ", " + wiimote.Accel.GetCalibratedAccelData()[2]);
        }*/
        if(calib1 && calib2 && calib3)
        {
            float[] acc = wiimote.Accel.GetCalibratedAccelData();
            Vector3 accel = new Vector3(acc[0], acc[2], acc[1]);
            Vector3 accelG = Vector3.down;//Quaternion.Euler(saber.transform.rotation.eulerAngles-new Vector3(0, 0, 0))*Vector3.down;
            lastAccel = accel;
            lastAccelG = accelG;
            //Debug.Log(accel + ", " + accelG + ", " + (accel + accelG));
        }

        // place wiimote on level surface, pointing at screen with wiimote "A" button up, then press keyboard "space" to reset and calibrate position and Wii Motion Plus
        if (Input.GetKeyDown(KeyCode.Space) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
        {
            MotionPlusData data = wiimote.MotionPlus;
            data.SetZeroValues();
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.rotation * GetAccelVector(), Vector3.up) * saber.transform.rotation;
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.forward, Vector3.forward) * saber.transform.rotation;
            saber.transform.Rotate(new Vector3(90, 0, 0));
            wmpoffset = initOff;
            lastAccNoGrav = Vector3.zero;
            vel = Vector3.zero;
            saber.transform.localPosition = initSaberOff;
            allowSaber = true;
        }

        // point wiimote at screen, with wiimote "A" facing upwards, then press wiimote "B" to recalibrate position only
        if (wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS && debugPosition)
        {
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.rotation * GetAccelVector(), Vector3.up) * saber.transform.rotation;
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.forward, Vector3.forward) * saber.transform.rotation;
            saber.transform.Rotate(new Vector3(90, 0, 0));
            saber.transform.localPosition = initSaberOff;
            wmpoffset = initOff;
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

    void OnDrawGizmos()
    {
        if (calib1 && calib2 && calib3)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(saber.transform.position, saber.transform.position + Quaternion.Euler(new Vector3(wmpoffset[0], -wmpoffset[2], -wmpoffset[1])) * Vector3.down);
            //Debug.Log(wmpoffset);

            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(saber.transform.position, saber.transform.position + Quaternion.Euler(wmpoffset)*GetAccelVector());

            Gizmos.color = Color.green;
            //Gizmos.DrawLine(saber.transform.position, saber.transform.position + GetAccelVector());

            Vector3 apprxGrav = Quaternion.Euler(new Vector3(wmpoffset[0], -wmpoffset[2], -wmpoffset[1])) * Vector3.down;
            Vector3 accel = GetNonNormalAccelVector();
            

            Vector3 accelNoGrav = accel - apprxGrav;
            float scale = 100f;
            accelNoGrav = new Vector3(Mathf.Round(accelNoGrav[0] * scale) / scale, Mathf.Round(accelNoGrav[1] * scale) / scale, Mathf.Round(accelNoGrav[2] * scale) / scale);
            lastAccNoGrav = accelNoGrav;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(saber.transform.position, saber.transform.position + accelNoGrav);

            //Debug.Log(lastAccelG);
        }
    }

    private Vector3 GetAccelVector()
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

}
