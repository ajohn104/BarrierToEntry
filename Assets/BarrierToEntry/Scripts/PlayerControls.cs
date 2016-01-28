using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;

namespace BarrierToEntry
{
    public class PlayerControls : MonoBehaviour
    {
        private ControlManager manager = new ControlManager();
        private Controller primaryController;
        private Controller secondaryController;
        Animator anim;

        public GameObject saber;
        public Transform handGrip;
        public Transform saberGrip;

        GameObject leftObj
        {
            get
            {
                return hand; //usingHand ? hand : altHand;
            }
        }

        public GameObject hand;
        public GameObject altHand;

        bool usingHand = true;

        private readonly Matrix4x4 scale = Matrix4x4.Scale(new Vector3(-10f, -10f, -2f));

        private readonly Vector3 saberHandGripOffset = new Vector3(0.3185708f, 0.9067955f, 0.03708483f);
        private readonly Vector3 saberOffset = new Vector3(-0.0908f, 0f, -0.0538f); //new Vector3(0.2912f, 0.8183469f, 0.03847433f);
        private readonly Vector3 handOffset = new Vector3(-0.3053612f, 0.841f, 0.1267993f);

        private readonly Vector3 saberTestOffset = new Vector3(0.2885624f, 1.081484f, 0.3783985f);
        private readonly Quaternion saberTestRot = Quaternion.Euler(new Vector3(357.5609f, 171.6962f, 2.578102f));
        private bool runTest = false;
        // handgrip localPos: (-0.21, 0, 1.84)

        private Vector3 primaryOffset
        {
            get
            {
                return saberOffset; //primaryUseSaber ? saberOffset : new Vector3(saberOffset.x, handOffset.y, saberOffset.z);
            }
        }

        private Vector3 secondaryOffset
        {
            get
            {
                return handOffset; //secondaryUseSaber ? new Vector3(handOffset.x, saberOffset.y, handOffset.z) : handOffset;
            }
        }

        private bool primaryUseSaber = true;
        private bool secondaryUseSaber = false;

        public Transform target1;
        public Transform target2;

        void Start()
        {
            manager.Start();
            manager.assignTargets(target1, target2);
            anim = GetComponent<Animator>();
            if(runTest)
            {
                saber.transform.localRotation = saberTestRot;
            }
        }

        void Update()
        {
            manager.Update();
            if (manager.foundControllers)
            {
                primaryController = primaryController ?? manager.controllers[0];
                secondaryController = secondaryController ?? manager.controllers[1];

                // Face screen with Rift and press "A" on the primary controller (currently primary is always in your right hand) to reset and calibrate the Oculus
                if (VRCenter.VREnabled && primaryController.buttons.a)
                {
                    VRCenter.Recenter();
                }

                if (manager.allowInput)
                {
                    if (primaryController.getButtonDown(Controller.Button.HOME))
                    {
                        Debug.Log("Primary Controller battery level: " + primaryController.getBattery());
                    }

                    if (secondaryController.getButtonDown(Controller.Button.HOME))
                    {
                        Debug.Log("Secondary Controller battery level: " + secondaryController.getBattery());
                    }

                    if (primaryController.getButtonDown(Controller.Button.PLUS))
                    {
                        primaryUseSaber = !primaryUseSaber;
                    }

                    if (secondaryController.getButtonDown(Controller.Button.PLUS))
                    {
                        secondaryUseSaber = !secondaryUseSaber;
                    }

                    if (secondaryController.getButtonDown(Controller.Button.MINUS))
                    {
                        //leftObj.SetActive(false);
                        //usingHand = !usingHand;
                        //leftObj.SetActive(true);
                    }

                    //handGrip.localPosition = scale.MultiplyVector(primaryController.position) + saberHandGripOffset;

                    Vector3 fullSaberRot = new Vector3(primaryController.rotation.x, 0, primaryController.rotation.y);
                    Vector3 fullHandleRot = new Vector3(0, 0, primaryController.rotation.z);

                    //saber.transform.localPosition = primaryOffset;
                    saber.transform.localRotation = Quaternion.Euler(fullSaberRot);
                    handGrip.localRotation = Quaternion.Euler(fullHandleRot);

                    leftObj.transform.localPosition = scale.MultiplyVector(secondaryController.position) + secondaryOffset;
                    leftObj.transform.localRotation = Quaternion.Euler(secondaryController.rotation);

                    
                }
            }
            if(runTest)
            {
                saber.transform.localPosition = saberTestOffset;
            }
        }

        private float _xRot = 0f;
        private float _yRot = 0f;
        private float _zRot = 0f;

        private float xRot
        {
            set
            {
                Debug.Log("xRot: " + _xRot + " -> " + value);
                _xRot = value;
            }

            get
            {
                return _xRot;
            }
        }

        private float yRot
        {
            set
            {
                Debug.Log("yRot: " + _xRot + " -> " + value);
                _yRot = value;
            }

            get
            {
                return _yRot;
            }
        }

        private float zRot
        {
            set
            {
                Debug.Log("zRot: " + _xRot + " -> " + value);
                _zRot = value;
            }

            get
            {
                return _zRot;
            }
        }

        void OnAnimatorIK()
        {
            /*
            bool changed = false;
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                xRot += 90;
                changed = true;
            }

            if(Input.GetKeyDown(KeyCode.Keypad4))
            {
                xRot = 0;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                xRot -= 90;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                yRot += 90;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                yRot = 0;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                yRot -= 90;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                zRot += 90;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                zRot = 0;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                zRot -= 90;
                changed = true;
            }

            if (changed) Debug.Log("rotation => (" + xRot + ", " + yRot + ", " + zRot + ")");
            */

            Quaternion destRot;
            Vector3 saberRot = saber.transform.eulerAngles;
            //Vector3 offsetRot = new Vector3(xRot, yRot, zRot);
            Vector3 offsetRot = new Vector3(180f, -180f, 90f);
            Vector3 finalRot = saberRot + offsetRot;
            finalRot.x = 90f;
            Vector3 scaleDownVec = new Vector3(0.05f, 0.1f, 0.05f);
            Matrix4x4 scaleDown = Matrix4x4.Scale(scaleDownVec);
            Vector3 finalOffset = new Vector3(90f, 45f, -45f);
            destRot = Quaternion.FromToRotation(handGrip.position, saberGrip.position); //Quaternion.Euler(finalOffset + scaleDown.MultiplyVector(finalRot));

            // curr handRot = (180, 0, 90)
            // curr saberRot = (0, 180, 0)
            // ideal offset = (180, -180, 90)

            //Debug.Log(saber.transform.eulerAngles);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, handGrip.position);

            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, destRot);//Quaternion.Euler(new Vector3(180f, -180f, 90f))); //destRot);
        }
    }
}