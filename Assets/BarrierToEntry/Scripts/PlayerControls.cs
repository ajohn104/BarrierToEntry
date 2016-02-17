using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using Coliseo;
using BarrierToEntry;
using SixenseCore;

namespace BarrierToEntry
{
    public class PlayerControls : MonoBehaviour
    {
        public GameObject saber;
        public Transform hand;
        public Transform handGrip;
        public Animator anim;
        public Rigidbody rb;
        
        private readonly Vector3 saberHandGripRotOffset = new Vector3(-90, 180, 0);
        private readonly Vector3 handRotOffset = new Vector3(180, 90, 90);

        private readonly Vector3 handGripIKOffset = new Vector3(0, -180, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);

        private Vector3 leftCalibOffset = Vector3.zero;
        private Vector3 rightCalibOffset = Vector3.zero;
        
        private float armLength = 0f;
        private readonly HumanBodyBones[] rightArmBones = new HumanBodyBones[] {
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand
        };

        private Vector3 realLeftShoulderPos = Vector3.zero;
        private Vector3 realRightShoulderPos = Vector3.zero;

        private float handDist = 0;

        private Tracker controllerLeft;
        private Tracker controllerRight;

        private Vector3 gripFineTuneRotOffset = new Vector3(-30, 0, 0);
        public Device device;

        void Start()
        {
            GenerateArmLength();
        }

        private bool InputCheck()
        {
            if(!SixenseCore.Device.BaseConnected)
            {
                return false;
            }
            controllerLeft = SixenseCore.Device.GetTrackerByIndex(0);
            controllerRight = SixenseCore.Device.GetTrackerByIndex(1);
            return controllerLeft != null && controllerRight != null;
        }

        private void GenerateArmLength()
        {
            for (int i = 0; i < rightArmBones.Length - 1; i++)
            {
                Vector3 bone1Position = anim.GetBoneTransform(rightArmBones[i]).position;
                Vector3 bone2Position = anim.GetBoneTransform(rightArmBones[i + 1]).position;
                armLength += Vector3.Distance(bone1Position, bone2Position);
            }
        }

        private void GenerateHandSize()
        {
            handDist = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.RightHand).position, anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position);
        }

        private Vector3 GetRealPosition(Tracker con)
        {
            return con.Position * device.m_worldUnitScaleInMillimeters;
        }

        // This will be position calibration, and the user will press their hands to the front of their shoulders.
        private void CalibrateShoulderPositions()
        {
            if (!InputCheck()) return;

            Transform spine = anim.GetBoneTransform(HumanBodyBones.Spine);
            Transform rightArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform leftArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            Vector3 bodyOffsetRight = spine.forward * handDist - anim.transform.position;
            Vector3 bodyOffsetLeft = spine.forward * handDist - anim.transform.position;

            rightCalibOffset = rightArm.position + bodyOffsetRight - controllerRight.Position;
            leftCalibOffset = leftArm.position + bodyOffsetLeft - controllerLeft.Position;

            realLeftShoulderPos = GetRealPosition(controllerLeft);
            realRightShoulderPos = GetRealPosition(controllerRight);
        }

        // In this test, the user must fully extend their arms in any direction, and the system will 
        // calculate their arm length based off the current calibration offsets. This will be used to 
        // greatly improve the accuracy of relative world-game position tracking.
        public void CalibrateUserArmLength()
        {
            float realRightArmLength = Vector3.Distance(GetRealPosition(controllerRight), realRightShoulderPos);
            float realLeftArmLength = Vector3.Distance(GetRealPosition(controllerLeft), realLeftShoulderPos);
            float averageRealArmLength = (realRightArmLength + realLeftArmLength) / 2f;
            device.m_worldUnitScaleInMillimeters = averageRealArmLength / armLength;
        }

        void Update()
        {
            if(!InputCheck()) return;

            // Face screen with Rift and press "A" on the primary controller (currently primary is always in your right hand) to reset and calibrate the Oculus
            if (VRCenter.VREnabled && controllerRight.GetButtonDown(Buttons.START))
            {
                VRCenter.Recenter();
            }

            if(controllerLeft.GetButton(Buttons.BUMPER) && controllerRight.GetButton(Buttons.BUMPER))
            {
                CalibrateShoulderPositions();
            }

            if (controllerLeft.GetButton(Buttons.JOYSTICK) && controllerRight.GetButton(Buttons.JOYSTICK))
            {
                CalibrateUserArmLength();
            }

            // I fully intend on improving this animation stuff later, but I'm focusing on integrating the hand tracking first
            /*anim.SetBool("RunForward", primaryController.getButton(Controller.Button.D_UP));
            if(primaryController.getButton(Controller.Button.D_UP))
            {
                Vector3 movement = new Vector3(0f, 0f, 1f);

                // Make movement vector proportional to the speed per second.
                movement *= 6 * Time.deltaTime;

                // Move the player to it's current position plus the movement.
                rb.MovePosition(transform.position + transform.rotation * movement);
            }

            anim.SetBool("TurnLeft", primaryController.getButton(Controller.Button.D_LEFT));
            if (primaryController.getButton(Controller.Button.D_LEFT))
            {
                Vector3 vec = new Vector3(0f, -100f, 0f);
                Quaternion deltaRotation = Quaternion.Euler(vec * Time.deltaTime + transform.rotation.eulerAngles);
                rb.AddRelativeTorque(vec * rb.mass / 2);
                transform.rotation = deltaRotation;
            }

            anim.SetBool("TurnRight", primaryController.getButton(Controller.Button.D_RIGHT));
            if (primaryController.getButton(Controller.Button.D_RIGHT))
            {
                Vector3 vec = new Vector3(0f, 100f, 0f);
                Quaternion deltaRotation = Quaternion.Euler(vec * Time.deltaTime + transform.rotation.eulerAngles);
                rb.AddRelativeTorque(vec * rb.mass / 2);
                transform.rotation = deltaRotation;
            }
            */

            handGrip.localPosition = controllerRight.Position;
            handGrip.localPosition += rightCalibOffset;

            Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 rightArmOffset = handGrip.position - rightArmPos;

            if (rightArmOffset.magnitude > armLength)
            {
                handGrip.position = rightArmPos + (armLength / rightArmOffset.magnitude)*rightArmOffset;
            }
                    
            handGrip.rotation = controllerRight.Rotation;
            handGrip.Rotate(gripFineTuneRotOffset, Space.Self);
            handGrip.Rotate(saberHandGripRotOffset);
            //handGrip.Rotate(gripFineTuneRotOffset);

            hand.localPosition = controllerLeft.Position;
            hand.localPosition += leftCalibOffset;

            Vector3 leftArmPos = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            Vector3 leftArmOffset = hand.position - leftArmPos;

            if (leftArmOffset.magnitude > armLength)
            {
                hand.position = leftArmPos + (armLength / leftArmOffset.magnitude) * leftArmOffset;
            }

            hand.localRotation = controllerLeft.Rotation;
            hand.Rotate(handRotOffset);
        }

        void OnAnimatorIK()
        {
            Quaternion rotBefore = handGrip.rotation;
            handGrip.Rotate(handGripIKOffset, Space.Self);
            Quaternion rotPrimary = handGrip.rotation;
            handGrip.rotation = rotBefore;

            rotBefore = hand.rotation;
            hand.Rotate(handIKOffset, Space.Self);
            Quaternion rotSecondary = hand.rotation;
            hand.rotation = rotBefore;
            
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            anim.SetIKPosition(AvatarIKGoal.RightHand, handGrip.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rotPrimary);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, rotSecondary);
        }
    }
}