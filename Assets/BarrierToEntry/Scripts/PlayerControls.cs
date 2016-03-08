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

        public Rigidbody rbTargetA;
        public Rigidbody rbTargetB;

        //public FixedJoint fixedJointTargetA;
        public ConfigurableJoint confJointTargetA;
        

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
            GenerateHandSize();
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

            Transform root = anim.transform;
            Transform rightArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform leftArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            
            Vector3 bodyOffsetRight = root.forward * handDist - anim.transform.position;
            Vector3 bodyOffsetLeft = root.forward * handDist - anim.transform.position;

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
        Vector3 lastRot = Vector3.zero;
        void FixedUpdate()
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

            anim.SetFloat("Forward", controllerRight.JoystickY);
            //Debug.Log(fixedJointTargetA.anchor);
            //fixedJointTargetA.anchor = controllerRight.Position;
            //fixedJointTargetA.
            // rbTargetA.position = controllerRight.Position;
            SoftJointLimit softLimit = confJointTargetA.linearLimit;
            softLimit.limit = armLength;
            confJointTargetA.linearLimit = softLimit;
            confJointTargetA.targetPosition = controllerRight.Position;
            //confJointTargetA.anchor = controllerRight.Position;
            confJointTargetA.targetRotation = controllerRight.Rotation;
            //confJointTargetA.


            return;

            float dt = Time.deltaTime;
            Vector3 saberPositionalOffset = (controllerRight.Position + rightCalibOffset + anim.transform.position - handGrip.position);
            
            float saberDistanceOffset = saberPositionalOffset.magnitude; // Clamp top speed in future? Would it even be necessary or beneficial? I may need to for the future force increments in block sequences
            rbSaber.velocity = ( dt != 0f ? saberPositionalOffset / dt : Vector3.zero ); // Divide by zero is avoided.

            /*
            rbSaber.velocity *= Mathf.Clamp(3*Vector3.Distance(controllerRight.Position + rightCalibOffset + anim.transform.position, handGrip.position), 0f, 1f);
            rbSaber.velocity = rbSaber.velocity.magnitude * (controllerRight.Position + rightCalibOffset + anim.transform.position - handGrip.position).normalized;
            rbSaber.AddForce((controllerRight.Position + rightCalibOffset + anim.transform.position- handGrip.position).normalized * Mathf.Clamp(100 * Vector3.Distance(controllerRight.Position + rightCalibOffset + anim.transform.position, handGrip.position), 0f, 75f), ForceMode.Force);
            */

            //rbSaber.MovePosition(controllerRight.Position + rightCalibOffset + anim.transform.position);
            //rbSaber.velocity = rbSaber.velocity.normalized*Mathf.Clamp(100*Vector3.Distance(controllerRight.Position + rightCalibOffset + anim.transform.position, handGrip.position), 0f, 50f);
            //Debug.Log(rbSaber.velocity);
            //handGrip.localPosition = controllerRight.Position;
            //handGrip.localPosition += rightCalibOffset;

            Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 rightArmOffset = handGrip.position - rightArmPos;

            if (rightArmOffset.magnitude > armLength)
            {
                rbSaber.position = rightArmPos + (armLength / rightArmOffset.magnitude)*rightArmOffset;
            }

            /*Quaternion originalRotGrip = handGrip.rotation;
            Quaternion originalLocalRotGrip = handGrip.localRotation;*/
            targetA.localRotation = controllerRight.Rotation;
            targetA.Rotate(gripFineTuneRotOffset);
            targetA.Rotate(saberHandGripRotOffset);

            /*Debug.Log("-------------------------------");
            Debug.Log("controller world rotation: " + controllerRight.Rotation.eulerAngles);
            Debug.Log("gripFineTuneRotOffset rotation: " + gripFineTuneRotOffset);
            Debug.Log("saberHandGripRotOffset rotation: " + saberHandGripRotOffset);
            Debug.Log("final intended world rotation: " + handGrip.eulerAngles);
            Debug.Log("final intended local rotation: " + handGrip.localEulerAngles);
            Vector3 calculatedRot = controllerRight.Rotation.eulerAngles;
            //calculatedRot = Quaternion.Euler(calculatedRot) * gripFineTuneRotOffset + calculatedRot;
            //calculatedRot =  (Quaternion.Euler(calculatedRot) * (Quaternion.Euler(gripFineTuneRotOffset) * (Quaternion.Euler(calculatedRot)))).eulerAngles;
            calculatedRot = (Quaternion.Euler( gripFineTuneRotOffset) * Quaternion.Euler(calculatedRot)).eulerAngles;
            calculatedRot.x %= 360f;
            calculatedRot.y %= 360f;
            calculatedRot.z %= 360f;

            //calculatedRot = Quaternion.Euler(calculatedRot) * saberHandGripRotOffset + calculatedRot;
            //calculatedRot =  (Quaternion.Euler(calculatedRot) * (Quaternion.Euler(saberHandGripRotOffset) * (Quaternion.Euler(calculatedRot)))).eulerAngles;
            calculatedRot = (Quaternion.Euler(saberHandGripRotOffset) * Quaternion.Euler(calculatedRot)).eulerAngles;
            calculatedRot.x %= 360f;
            calculatedRot.y %= 360f;
            calculatedRot.z %= 360f;

            Debug.Log("final calculated rotation: " + calculatedRot);*/

            float saberRotOffsetAngle = Quaternion.Angle(targetA.localRotation, handGrip.localRotation);
            //Debug.Log("rot offset: " + saberRotOffsetAngle);

            Vector3 optimal = (targetA.localEulerAngles - handGrip.localEulerAngles);
            optimal.x %= 360;
            if (optimal.x > 180) optimal.x -= 360;
            else if (optimal.x <= -180) optimal.x += 360;
            optimal.y %= 360;
            if (optimal.y > 180) optimal.y -= 360;
            else if (optimal.y <= -180) optimal.y += 360;
            optimal.z %= 360;
            if (optimal.z > 180) optimal.z -= 360;
            else if (optimal.z <= -180) optimal.z += 360;
            optimal *= Mathf.Deg2Rad;
            Debug.Log("---------------------");
            Debug.Log("saber velocity: " + rbSaber.angularVelocity);
            Debug.Log("optimal offset: " + optimal);
            Debug.Log("dt: " + dt);

            float saberRotMarginOfError = 0.01f;

            //rbSaber.angularVelocity = (dt != 0) ? new Vector3(
            //        (Mathf.Abs(optimal.x) > saberRotMarginOfError) ? optimal.x : 0f,
            //        (Mathf.Abs(optimal.y) > saberRotMarginOfError) ? optimal.y : 0f,
            //        (Mathf.Abs(optimal.z) > saberRotMarginOfError) ? optimal.z : 0f
            //    ) : Vector3.zero;

            //rbSaber.angularVelocity = Vector3.zero;

            Vector3 resultAngleVel = optimal;
            //resultAngleVel = resultAngleVel.normalized;
            resultAngleVel = (dt != 0) ? resultAngleVel / dt: Vector3.zero;
            //resultAngleVel *= Mathf.Deg2Rad;
            //resultAngleVel /= 100f;
            //Debug.Log("resultAngleVel: " + resultAngleVel);
            //Debug.Log("expected rotation change: " + resultAngleVel*dt);
            rbSaber.maxAngularVelocity = 20f;
            if (Mathf.Abs(saberRotOffsetAngle) > 0.01f)
            {
                rbSaber.angularVelocity = handGrip.rotation * resultAngleVel;
                //rbSaber.angularVelocity = handGrip.rotation * rbSaber.angularVelocity;
                Vector3 finalRotVel = new Vector3(
                    (Mathf.Abs(rbSaber.angularVelocity.x) > saberRotMarginOfError) ? rbSaber.angularVelocity.x : 0f,
                    (Mathf.Abs(rbSaber.angularVelocity.y) > saberRotMarginOfError) ? rbSaber.angularVelocity.y : 0f,
                    (Mathf.Abs(rbSaber.angularVelocity.z) > saberRotMarginOfError) ? rbSaber.angularVelocity.z : 0f
                );
                Debug.Log("finalRotVel: " + finalRotVel);
                rbSaber.angularVelocity = finalRotVel;
                //rbSaber.angularVelocity = finalRotVel;//, ForceMode.Impulse);
            }

            //rbSaber.MoveRotation(targetA.rotation);
            //rbSaber.angularVelocity = resultAngleVel;

            //rbSaber.AddTorque(Vector3.up*Mathf.PI*10f, ForceMode.VelocityChange);
            /*Vector3 resultAngleVel = new Vector3(
                    (Mathf.Abs(optimal.x) > saberRotMarginOfError) ? Mathf.Floor(optimal.x) / dt : 0f,
                    (Mathf.Abs(optimal.y) > saberRotMarginOfError) ? Mathf.Floor(optimal.y) / dt : 0f,
                    (Mathf.Abs(optimal.z) > saberRotMarginOfError) ? Mathf.Floor(optimal.z) / dt : 0f
                );
            Debug.Log("expected angular velocity: " + resultAngleVel);*/
            //Debug.Log("max angular velocity: " + rbSaber.maxAngularVelocity);

            //handGrip.localRotation = targetA.localRotation;
            /*Quaternion goalRotGrip = handGrip.rotation;
            Quaternion goalLocalRotGrip = handGrip.localRotation;
            //handGrip.rotation = originalRotGrip;
            
            rbSaber.MoveRotation(goalRotGrip);*/
            //handGrip.eulerAngles = new Vector3(goalRotGrip.eulerAngles.x, goalRotGrip.eulerAngles.y, handGrip.eulerAngles.z);

            /*Debug.Log("Last rotation: " + lastRot);
            Debug.Log("New rotation: " + handGrip.localEulerAngles);
            Debug.Log("Difference: " + (handGrip.localEulerAngles - lastRot));
            lastRot = handGrip.localEulerAngles;*/
            //rbSaber.angularVelocity = Vector3.up*(2f*Mathf.PI);
            /*//Debug.Log(rbSaber.angularVelocity);
            

            Vector3 optimal = (goalLocalRotGrip.eulerAngles - originalLocalRotGrip.eulerAngles);
            optimal.x %= 360;
            if (optimal.x > 180) optimal.x -= 360;
            else if (optimal.x <= -180) optimal.x += 360;
            optimal.y %= 360;
            if (optimal.y > 180) optimal.y -= 360;
            else if (optimal.y <= -180) optimal.y += 360;
            optimal.z %= 360;
            if (optimal.z > 180) optimal.z -= 360;
            else if (optimal.z <= -180) optimal.z += 360;
            Debug.Log(optimal);

            rbSaber.angularVelocity = new Vector3(
                rbSaber.angularVelocity.x * Mathf.Clamp(Mathf.Abs(optimal.x) / 10f, 0, 1f),
                rbSaber.angularVelocity.y * Mathf.Clamp(Mathf.Abs(optimal.y) / 10f, 0, 1f),
                rbSaber.angularVelocity.z * Mathf.Clamp(Mathf.Abs(optimal.z) / 10f, 0, 1f)
            );

            rbSaber.angularVelocity = optimal.normalized * rbSaber.angularVelocity.magnitude;

            rbSaber.AddRelativeTorque(optimal * 5f);
            */
            //rbSaber.angularVelocity *= Mathf.Clamp(Mathf.Abs(Vector3.Angle(originalRotGrip.eulerAngles, goalRotGrip.eulerAngles)) / 5f , 0f, 1f);
            //rbSaber.angularVelocity = rbSaber.angularVelocity.magnitude * (goalRotGrip.eulerAngles - originalRotGrip.eulerAngles).normalized;
            //Debug.Log(Mathf.Clamp(Mathf.Abs(Vector3.Angle(originalRotGrip.eulerAngles, goalRotGrip.eulerAngles)) / 5f, 0f, 1f));
            /*rbSaber.angularVelocity = new Vector3(
                Mathf.Clamp((goalRotGrip.eulerAngles - originalRotGrip.eulerAngles).x / 100f, 0, 1f),
                Mathf.Clamp((goalRotGrip.eulerAngles - originalRotGrip.eulerAngles).y / 100f, 0, 1f),
                Mathf.Clamp((goalRotGrip.eulerAngles - originalRotGrip.eulerAngles).z / 100f, 0, 1f)
                );
            
            //Debug.Log(Mathf.Clamp(Mathf.Abs((goalRotGrip.eulerAngles - originalRotGrip.eulerAngles).x) / 300f, 0, 1f));
            Debug.Log(rbSaber.angularVelocity);
            Vector3 diff = goalRotGrip.eulerAngles - originalRotGrip.eulerAngles;
            diff.x %= 360;
            if(diff.x > 180) diff.x-=360;
            diff.y %= 360;
            if (diff.y > 180) diff.y -= 360;
            diff.z %= 360;
            if (diff.z > 180) diff.z -= 360;
            Debug.Log(diff.normalized);
            //diff.z = 0;
            //diff.y = 0;
            rbSaber.AddTorque(diff.normalized);*/
            //rbSaber.MoveRotation(goalRotGrip);
            //rbSaber.AddTorque(rot.normalized, ForceMode.Force);

            //rbSaber.AddTorque(Quaternion.FromToRotation(initFor, finalFor).eulerAngles.normalized);
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
            Quaternion computedRot = Quaternion.Euler(handGrip.rotation.eulerAngles) * Quaternion.Euler(handGripIKOffset);
            Quaternion computedRot2 = Quaternion.Euler(hand.rotation.eulerAngles) * Quaternion.Euler(handIKOffset);

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
            if (controllerRight == null || controllerLeft == null)
                return;
            Quaternion originalRotGrip = handGrip.rotation;

            handGrip.rotation = controllerRight.Rotation;
            handGrip.Rotate(gripFineTuneRotOffset, Space.Self);
            handGrip.Rotate(saberHandGripRotOffset);

            Quaternion goalRotGrip = handGrip.rotation;
            Vector3 up = handGrip.up;
            handGrip.rotation = originalRotGrip;
            //Debug.Log(goalRotGrip.eulerAngles);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(handGrip.position, handGrip.position+up);
            //Gizmos.DrawSphere(anim.transform.position + controllerRight.Position + rightCalibOffset, 0.01f);
//(controllerRight.Position + rightCalibOffset - handGrip.localPosition).normalized
        }
    }
}