using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Player : Actor
    {
        private readonly Vector3 handGripIKOffset = new Vector3(0, -180, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);
        private readonly Vector3 gripFineTuneRotOffset = new Vector3(-30, 0, 0);

        public Controls controls;
        public new PlayerConfig config;


        // Use this for initialization
        void Start()
        {
            config = new PlayerConfig(this);
            controls = new Controls();
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void _UpdateDominantHand()
        {

        }

        protected override void _UpdateNonDominantHand()
        {

        }

        void OnAnimatorIK()
        {
            Quaternion computedRot = Quaternion.Euler(saber.transform.rotation.eulerAngles) * Quaternion.Euler(handGripIKOffset);
            Quaternion computedRot2 = Quaternion.Euler(hand.rotation.eulerAngles) * Quaternion.Euler(handIKOffset) * transform.rotation;

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            anim.SetIKPosition(AvatarIKGoal.RightHand, saber.transform.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, computedRot);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, computedRot2);
        }

        void OnDrawGizmos()
        {
            if (!controls.InputCheck()) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(saber.transform.position, saber.transform.position + saber.target.up);

            Gizmos.DrawSphere(transform.TransformPoint(controls.controllerRight.Position + config.rightCalibOffset), 0.01f);
        }
    }
}