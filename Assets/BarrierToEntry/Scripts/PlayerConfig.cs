using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class PlayerConfig : ActorConfig
    {

        private Vector3 realLeftShoulderPos = Vector3.zero;
        private Vector3 realRightShoulderPos = Vector3.zero;

        public Vector3 leftCalibOffset = Vector3.zero;
        public Vector3 rightCalibOffset = Vector3.zero;

        private float handDist = 0;
        private new Player owner;


        public void GenerateHandSize()
        {
            // Temporarily disabled. TODO: Find out if this is helpful.
            //handDist = Vector3.Distance(owner.anim.GetBoneTransform(HumanBodyBones.RightHand).position, anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position) / 2f;
        }

        // This is for position calibration, and the user will press their hands to the front of their shoulders.
        public void CalibrateShoulderPositions()
        {
            if (!owner.controls.InputCheck()) return;
            Transform root = owner.anim.transform;
            Transform rightArm = owner.anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform leftArm = owner.anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            Vector3 bodyOffset = Vector3.forward * handDist;

            // TODO: Clean this shit up. Perhaps move part of it to controls, or another class entirely. I'm just not sure it belongs in PlayerConfig. But...maybe it does.
            rightCalibOffset = owner.transform.InverseTransformPoint(rightArm.position) + bodyOffset - owner.controls.controllerRight.Position;
            leftCalibOffset = owner.transform.InverseTransformPoint(leftArm.position) + bodyOffset - owner.controls.controllerLeft.Position;

            realLeftShoulderPos = owner.controls.GetRealPosition(Controller.LEFT);
            realRightShoulderPos = owner.controls.GetRealPosition(Controller.RIGHT);
        }

        // In this test, the user must fully extend their arms in any direction, and the system will 
        // calculate their arm length based off the current calibration offsets. This will be used to 
        // greatly improve the accuracy of relative world-game position tracking.
        public void CalibrateUserArmLength()
        {
            float realRightArmLength = Vector3.Distance(owner.controls.GetRealPosition(Controller.RIGHT), realRightShoulderPos);
            float realLeftArmLength = Vector3.Distance(owner.controls.GetRealPosition(Controller.LEFT), realLeftShoulderPos);
            float averageRealArmLength = (realRightArmLength + realLeftArmLength) / 2f;
            //Debug.Log("your average arm length (in mm): " + averageRealArmLength);
            //Debug.Log("character arm length (in units): " + armLength);
            //Debug.Log("1 unit to mm: " + (averageRealArmLength / armLength));
            owner.controls.device.m_worldUnitScaleInMillimeters = (ArmLength != 0f && averageRealArmLength != 0f) ? (averageRealArmLength / ArmLength) : owner.controls.device.m_worldUnitScaleInMillimeters;
        }

        public PlayerConfig(Player owner) : base(owner)
        {
            this.owner = owner;
        }
    }
}