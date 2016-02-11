using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using Coliseo;
using BarrierToEntry;
using SixenseCore;

namespace BarrierToEntry
{
    public class PlayerControls : MonoBehaviour
    {
        private ControlManager manager = new ControlManager();
        private Controller primaryController;
        private Controller secondaryController;

        public GameObject saber;
        public Transform hand;
        public Transform handGrip;
        public Transform saberGrip;
        public Animator anim;
        public Transform target1;
        public Transform target2;

        private readonly Matrix4x4 scale = Matrix4x4.Scale(new Vector3(-10f, -10f, -2f));
        private readonly Vector3 saberHandGripOffset = new Vector3(0.3432f, 0.9008f, 0.0357f);
        private readonly Vector3 handOffset = new Vector3(-0.3053612f, 0.841f, 0.1267993f);
        private readonly Vector3 handGripIKOffset = new Vector3(0, -90, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);
        
        private float armLength = 0f;
        private readonly HumanBodyBones[] rightArmBones = new HumanBodyBones[] {
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand
        };

        void Start()
        {
            manager.Start();
            manager.assignTargets(target1, target2);
            
            for (int i = 0; i < rightArmBones.Length - 1; i++)
            {
                Vector3 bone1Position = anim.GetBoneTransform(rightArmBones[i]).position;
                Vector3 bone2Position = anim.GetBoneTransform(rightArmBones[i+1]).position;
                armLength += Vector3.Distance(bone1Position, bone2Position);
            }
        }

        void Update()
        {
            // SixenseCore.Device.GetTrackerByIndex(0).Position             // <-- this is key!
            // SixenseCore.Device.GetTrackerByIndex(0).MagneticFrequency    // <-- use this to tell controllers apart?
            // SixenseCore.Device.GetTrackerByIndex(0).Connected            // <-- probably also helpful
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

                    handGrip.localPosition = scale.MultiplyVector(primaryController.position) + saberHandGripOffset;

                    Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                    Vector3 rightArmOffset = handGrip.position - rightArmPos;

                    if (rightArmOffset.magnitude > armLength)
                    {
                        handGrip.position = rightArmPos + (armLength / rightArmOffset.magnitude)*rightArmOffset;
                    }
                    
                    handGrip.localRotation = Quaternion.Euler(primaryController.rotation);

                    hand.localPosition = scale.MultiplyVector(secondaryController.position) + handOffset;
                    hand.localRotation = Quaternion.Euler(secondaryController.rotation);

                    Vector3 leftArmPos = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
                    Vector3 leftArmOffset = hand.position - leftArmPos;

                    if (leftArmOffset.magnitude > armLength)
                    {
                        hand.position = leftArmPos + (armLength / leftArmOffset.magnitude) * leftArmOffset;
                    }
                }
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
            
            bool changed = false;
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                xRot += 90;
                xRot %= 360;
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
                xRot %= 360;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                yRot += 90;
                yRot %= 360;
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
                yRot %= 360;
                changed = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                zRot += 90;
                zRot %= 360;
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
                zRot %= 360;
                changed = true;
            }

            if (changed) Debug.Log("rotation => (" + xRot + ", " + yRot + ", " + zRot + ")");


            Quaternion rotBefore = handGrip.rotation;
            handGrip.Rotate(handGripIKOffset, Space.Self);
            Quaternion rotPrimary = handGrip.localRotation;
            handGrip.rotation = rotBefore;

            rotBefore = hand.rotation;
            hand.Rotate(handIKOffset, Space.Self);
            Quaternion rotSecondary = hand.localRotation;
            hand.rotation = rotBefore;
            
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, handGrip.position);

            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rotPrimary);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);

            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, rotSecondary);

        }
    }
}