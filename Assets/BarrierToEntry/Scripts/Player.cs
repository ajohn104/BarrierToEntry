using UnityEngine;
using System.Collections;
using SixenseCore;
using Coliseo;

namespace BarrierToEntry
{
    public class Player : Actor
    {
        private readonly Vector3 HandGripIKOffset = new Vector3(0, -180, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);
        private readonly Vector3 GripFineTuneRotOffset = new Vector3(-30, 0, 0);

        public Controls controls;
        public Device device;

        private PlayerConfig _config;
        public override ActorConfig config
        {
            get
            {
                return _config;
            }

            set
            {
                _config = (PlayerConfig) value;
            }
        }


        // Use this for initialization
        void Start()
        {
            _config = new PlayerConfig(this);
            controls = new Controls(device);
            Physics.IgnoreCollision(collider, weapon.collider);
            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            config.GenerateArmLength();
            _config.GenerateHandSize();
        }

        protected override void Think()     // TODO: Move control stuff in Player.Think to Controls.cs
        {
            if (!controls.InputCheck()) return;
            // Face screen with Rift and press "A" on the primary controller (currently primary is always in your right hand) to reset and calibrate the Oculus
            if (VRCenter.VREnabled && controls.controllerRight.GetButtonDown(SixenseCore.Buttons.START))
            {
                VRCenter.Recenter();
            }

            if (controls.controllerLeft.GetButton(SixenseCore.Buttons.BUMPER) && controls.controllerRight.GetButton(SixenseCore.Buttons.BUMPER))
            {
                _config.CalibrateShoulderPositions();
                weapon.transform.position = transform.TransformPoint(controls.controllerRight.Position + _config.rightCalibOffset);
            }

            if (controls.controllerLeft.GetButton(SixenseCore.Buttons.JOYSTICK) && controls.controllerRight.GetButton(SixenseCore.Buttons.JOYSTICK))
            {
                _config.CalibrateUserArmLength();
            }
            CheckMovementInput();
            _UpdateDominantHand();
            _UpdateNonDominantHand();
        }

        protected override void Feedback(float errorOffset, float errorAngle)
        {
            errorAngle -= weapon.rumbleCutoff;
            weapon.saberErrorDist = errorOffset + Mathf.Clamp(errorAngle / weapon.rumbleRange, 0, 0.5f) * 2f;
        }

        protected override void _UpdateDominantHand()
        {
            weapon.target.localRotation = controls.controllerRight.Rotation;
            weapon.target.Rotate(GripFineTuneRotOffset);
            weapon.target.Rotate(weapon.SaberHandGripRotOffset);
        }

        protected override void _UpdateNonDominantHand()
        {
            this.NonDominantHandPos = controls.controllerLeft.Position + _config.leftCalibOffset;

            this.NonDominantHandRot = controls.controllerLeft.Rotation * Quaternion.Euler(weapon.HandRotOffset);
        }

        protected void CheckMovementInput()
        {
            this.MoveSpeedForward = controls.controllerLeft.JoystickY;
            this.RotationSpeedHoriz = controls.controllerRight.JoystickX;
            this.DominantHandPos = controls.controllerRight.Position + _config.rightCalibOffset;     // TODO: Make all this shit dependent on dominant/nondominant hand (use enums, prob)
        }

        void OnAnimatorIK()
        {
            Quaternion computedRot = Quaternion.Euler(weapon.transform.rotation.eulerAngles) * Quaternion.Euler(HandGripIKOffset);
            Quaternion computedRot2 = Quaternion.Euler(hand.rotation.eulerAngles) * Quaternion.Euler(handIKOffset) * transform.rotation;

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            anim.SetIKPosition(AvatarIKGoal.RightHand, weapon.transform.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, computedRot);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, computedRot2);
        }

        void OnDrawGizmos()
        {
            if (controls == null || !controls.InputCheck()) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(weapon.transform.position, weapon.transform.position + weapon.target.up);

            Gizmos.DrawSphere(transform.TransformPoint(controls.controllerRight.Position + _config.rightCalibOffset), 0.01f);
        }
    }
}