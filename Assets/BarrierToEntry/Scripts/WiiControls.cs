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


    void Start()
    {
        saber = transform.Find("LSaber").gameObject;
        WiimoteManager.FindWiimotes();
    }
    bool allowSaber = false;
    void Update()
    {

        if(!foundWiimote)
        {
            if(!WiimoteManager.HasWiimote())
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
                Vector3 offset = new Vector3(-wiimote.MotionPlus.PitchSpeed,
                                                wiimote.MotionPlus.RollSpeed,
                                                -wiimote.MotionPlus.YawSpeed) / 95f; // Divide by 95Hz (average updates per second from wiimote)
                if (allowSaber) saber.transform.Rotate(offset, Space.Self);
            }
        } while (ret > 0);

        if (!requestedWMP)
        {
            wiimote.RequestIdentifyWiiMotionPlus();
            requestedWMP = true;
            return;
        }

        if(!activatedWMP)
        {
            wiimote.ActivateWiiMotionPlus();
            activatedWMP = true;
            return;
        }

        if(!setupCamera)
        {
            wiimote.SetupIRCamera(IRDataType.BASIC);
            setupCamera = true;
            return;
        }

        // Face screen with Rift and press "A" to reset and calibrate the Oculus
        if(wiimote.Button.a)
        {
            VRCenter.Recenter();
        }

        // place wiimote on level surface, pointing at screen with wiimote "A" button up, then press keyboard "space" to reset and calibrate position and Wii Motion Plus
        if(Input.GetKeyDown(KeyCode.Space) && wiimote.current_ext == ExtensionController.MOTIONPLUS)
        {
            MotionPlusData data = wiimote.MotionPlus;
            data.SetZeroValues();
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.rotation * GetAccelVector(), Vector3.up) * saber.transform.rotation;
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.forward, Vector3.forward) * saber.transform.rotation;
            saber.transform.Rotate(new Vector3(90, 0, 0));
            allowSaber = true;
        }

        // point wiimote at screen, with wiimote "A" facing upwards, then press wiimote "B" to recalibrate position only
        if(wiimote.Button.b && wiimote.current_ext == ExtensionController.MOTIONPLUS)
        {
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.rotation * GetAccelVector(), Vector3.up) * saber.transform.rotation;
            saber.transform.rotation = Quaternion.FromToRotation(saber.transform.forward, Vector3.forward) * saber.transform.rotation;
            saber.transform.Rotate(new Vector3(90, 0, 0));
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
        accel_z = -accel[1];

        return new Vector3(accel_x, accel_y, accel_z).normalized;
    }

}
