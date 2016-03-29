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
        public Rigidbody rbPlayer;
        public Rigidbody rbSaber;

        public Transform targetA;
        public Transform targetB;

        private readonly Vector3 saberHandGripRotOffset = new Vector3(-90, 180, 0);
        private readonly Vector3 handRotOffset = new Vector3(180, 90, 90);

        private readonly Vector3 handGripIKOffset = new Vector3(0, -180, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);

        private Vector3 leftCalibOffset = Vector3.zero;
        private Vector3 rightCalibOffset = Vector3.zero;

        public Collider playerCol;
        public Collider saberCol;

        private Vector3 initSaberPos;
        
        private float armLength = 0f;
        private readonly HumanBodyBones[] rightArmBones = new HumanBodyBones[] {
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand
        };

        private Vector3 realLeftShoulderPos = Vector3.zero;
        private Vector3 realRightShoulderPos = Vector3.zero;

        private float handDist = 0;

        public Tracker controllerLeft;
        public Tracker controllerRight;

        private Vector3 gripFineTuneRotOffset = new Vector3(-30, 0, 0);
        public Device device;
        public float collisionPrevention = 1f;
        public Transform saberCoM;

        public float rumbleCutoff = 10f;
        public float rumbleRange = 15f;


        private float _saberErrorDist = 0f;
        public float saberErrorDist {
            get
            {
                return _saberErrorDist;
            }
        }

        void Start()
        {
            GenerateArmLength();
            GenerateHandSize();
            initSaberPos = targetA.transform.localPosition;
            Physics.IgnoreCollision(playerCol, saberCol);
            //fixedJointHandGrip.autoConfigureConnectedAnchor = true;
            rbSaber.centerOfMass = rbSaber.transform.InverseTransformPoint(saberCoM.position);
        }

        public bool InputCheck()
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
            handDist = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.RightHand).position, anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position)/2f;
        }

        private Vector3 GetRealPosition(Tracker con)
        {
            return con.Position * device.m_worldUnitScaleInMillimeters;
        }

        // This will be position calibration, and the user will press their hands to the front of their shoulders.
        private void CalibrateShoulderPositions()
        {
            if (!InputCheck()) return;

            Transform root = anim.transform;
            Transform rightArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform leftArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            Vector3 bodyOffset = Vector3.forward * handDist;

            rightCalibOffset = transform.InverseTransformPoint(rightArm.position) + bodyOffset - controllerRight.Position;
            leftCalibOffset = transform.InverseTransformPoint(leftArm.position) + bodyOffset - controllerLeft.Position;

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
            //Debug.Log("your average arm length (in mm): " + averageRealArmLength);
            //Debug.Log("character arm length (in units): " + armLength);
            //Debug.Log("1 unit to mm: " + (averageRealArmLength / armLength));
            device.m_worldUnitScaleInMillimeters = (armLength != 0f && averageRealArmLength != 0f) ? (averageRealArmLength / armLength) : device.m_worldUnitScaleInMillimeters;
            
        }
        Vector3 lastRot = Vector3.zero;
        void FixedUpdate()
        {
            if (!InputCheck()) return;

            // Face screen with Rift and press "A" on the primary controller (currently primary is always in your right hand) to reset and calibrate the Oculus
            if (VRCenter.VREnabled && controllerRight.GetButtonDown(Buttons.START))
            {
                VRCenter.Recenter();
            }

            if(controllerLeft.GetButton(Buttons.BUMPER) && controllerRight.GetButton(Buttons.BUMPER))
            {
                CalibrateShoulderPositions();
                handGrip.position = transform.TransformPoint(controllerRight.Position + rightCalibOffset);
            }

            if (controllerLeft.GetButton(Buttons.JOYSTICK) && controllerRight.GetButton(Buttons.JOYSTICK))
            {
                CalibrateUserArmLength();
            }

            anim.SetFloat("Forward", controllerLeft.JoystickY);
            transform.Rotate(new Vector3(0, -3f* (Mathf.Rad2Deg * Mathf.Acos(controllerRight.JoystickX)-90f)/180f, 0));

            Vector3 calculatedGripPosition = transform.TransformPoint(controllerRight.Position + rightCalibOffset);
            Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 rightArmOffset = calculatedGripPosition - rightArmPos;

            if (rightArmOffset.magnitude > armLength)
            {
                calculatedGripPosition = rightArmPos + (armLength / rightArmOffset.magnitude) * rightArmOffset;
            }

            targetA.localRotation = controllerRight.Rotation;
            targetA.Rotate(gripFineTuneRotOffset);
            targetA.Rotate(saberHandGripRotOffset);

            Vector3 moveOffset = calculatedGripPosition - rbSaber.position;
            Vector3 partialMove = Vector3.Lerp(rbSaber.position, calculatedGripPosition, collisionPrevention);
            rbSaber.MovePosition(partialMove);
            _saberErrorDist = moveOffset.magnitude;

            rbSaber.MoveRotation(Quaternion.Lerp(rbSaber.rotation, targetA.rotation, collisionPrevention));

            float errorAngle = Mathf.Abs(Quaternion.Angle(rbSaber.rotation, targetA.rotation));
            
            errorAngle -= rumbleCutoff;
            
            _saberErrorDist += Mathf.Clamp(errorAngle / rumbleRange, 0, 0.5f)*2f;
            
            hand.position = transform.rotation * (controllerLeft.Position + leftCalibOffset) + transform.position;

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
            Quaternion computedRot =  Quaternion.Euler(handGrip.rotation.eulerAngles) * Quaternion.Euler(handGripIKOffset);
            Quaternion computedRot2 = Quaternion.Euler(hand.rotation.eulerAngles) * Quaternion.Euler(handIKOffset) * transform.rotation;

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            anim.SetIKPosition(AvatarIKGoal.RightHand, handGrip.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, computedRot);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, computedRot2);
        }

        void OnDrawGizmos()
        {
            if (!InputCheck()) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(handGrip.position, handGrip.position+ targetA.up);

            Gizmos.DrawSphere(transform.TransformPoint(controllerRight.Position + rightCalibOffset), 0.01f);
        }
    }
}