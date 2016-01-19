using UnityEngine;
using System.Collections;
using Coliseo;

namespace Coliseo
{
    public class VRCenter : MonoBehaviour
    {

        public static void Setup(bool center = false)
        {
            UnityEngine.VR.VRSettings.showDeviceView = true;
            UnityEngine.VR.VRSettings.loadedDevice = UnityEngine.VR.VRDeviceType.Oculus;
            if(center) Recenter();
        }

        public static bool VRPresent
        {
            get { return UnityEngine.VR.VRDevice.isPresent; }
        }

        public static bool VREnabled
        {
            get { return Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer && UnityEngine.VR.VRDevice.isPresent; }
        }

        public static Vector3 position
        {
            get { return UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head); }
        }

        public static Quaternion rotation
        {
            get { return UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.Head); }
        }

        public static void Recenter()
        {
            UnityEngine.VR.InputTracking.Recenter();
        }
    }
}
