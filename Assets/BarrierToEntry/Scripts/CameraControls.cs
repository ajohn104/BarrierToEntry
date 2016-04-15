using UnityEngine;
using System.Collections;
using Coliseo;

namespace BarrierToEntry
{
    public class CameraControls : MonoBehaviour
    {

        public Transform camTrans;
        public Player player;

        // Mouse sensitivity
        public float horizontalSpeed = 4.0F;
        public float verticalSpeed = 4.0F;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = verticalSpeed * Input.GetAxis("Mouse Y");

            float rotationX = h * Time.deltaTime;
            float rotationY = v * Time.deltaTime;

            if(player.controls.InputCheck())
            {
                if(Application.platform == RuntimePlatform.WindowsPlayer) Cursor.visible = false;
                rotationY = verticalSpeed * player.controls.controllerRight.JoystickY*Time.deltaTime;
                rotationX = 0;
            }

            if (VRCenter.VREnabled)
            {
                rotationY = 0;
                rotationX = 0;
            }

            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                camTrans.Rotate(new Vector3(-rotationY / verticalSpeed * 40f, rotationX / horizontalSpeed * 40f, 0));
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                camTrans.Rotate(new Vector3(-rotationY / verticalSpeed * 40f, rotationX / horizontalSpeed * 40f, 0));
            }
        }
    }
}