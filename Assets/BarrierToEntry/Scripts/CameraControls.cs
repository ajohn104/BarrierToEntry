using UnityEngine;
using System.Collections;
using Coliseo;

namespace BarrierToEntry
{
    public class CameraControls : MonoBehaviour
    {

        public Transform camTrans;
        public PlayerControls controls;

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

            if(controls.InputCheck())
            {
                rotationY = verticalSpeed * controls.controllerRight.JoystickY*Time.deltaTime;
            }

            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                camTrans.Rotate(-rotationY, rotationX, 0);
                camTrans.rotation = Quaternion.Euler(new Vector3(camTrans.rotation.eulerAngles.x, camTrans.rotation.eulerAngles.y, 0));
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                camTrans.Rotate(-rotationY / verticalSpeed * 40f, rotationX / horizontalSpeed * 40f, 0);
                camTrans.rotation = Quaternion.Euler(new Vector3(camTrans.rotation.eulerAngles.x, camTrans.rotation.eulerAngles.y, 0));
            }
        }
    }
}