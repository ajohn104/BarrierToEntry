using UnityEngine;
using System.Collections;
using SixenseCore;
using Coliseo;

namespace BarrierToEntry
{
    public class Player : Actor
    {
        
        private readonly Vector3 GripFineTuneRotOffset = new Vector3(-30, 0, 0);

        public Controls controls;
        public Device device;

        private PlayerConfig _config;
        public override ActorConfig config
        {
            get { return _config; }
            set { _config = (PlayerConfig) value; }
        }


        // Use this for initialization
        void Start()
        {
            _config = new PlayerConfig(this);
            _config.GenerateArmLength();
            _config.GenerateHandSize();

            controls = new Controls(device);
            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            Physics.IgnoreCollision(collider, weapon.collider);

            modelDesign.Prepare();
            
        }
        
        protected override void Think()     // TODO: Move control stuff in Player.Think to Controls.cs
        {
            if (!controls.InputCheck()) return;
            CheckRecenter();
            CheckCalibrateShoulder();
            CheckCalibrateUserArmLength();
            CheckMovementInput();

            _UpdateDominantHand();
            _UpdateNonDominantHand();
            
            //if (controls.controllerRight.GetButtonDown(Buttons.TRIGGER)) ModelGenerator.RandomizeModel(this);
        }

        private void CheckRecenter()
        {
            if (VRCenter.VREnabled && controls.Recenter) { VRCenter.Recenter(); }
        }

        private void CheckCalibrateShoulder()
        {
            if (controls.CalibrateShoulder)
            {
                _config.CalibrateShoulderPositions();
                weapon.transform.position = transform.TransformPoint(controls.controllerRight.Position + _config.rightCalibOffset); // TODO: make this relativeness a method in _config
            }
        }

        private void CheckCalibrateUserArmLength()
        {
            if (controls.CalibrateUserArmLength) { _config.CalibrateUserArmLength(); }
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

        void OnDrawGizmos()
        {
            if (controls == null || !controls.InputCheck()) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(weapon.transform.position, weapon.transform.position + weapon.target.up);

            Gizmos.DrawSphere(transform.TransformPoint(controls.controllerRight.Position + _config.rightCalibOffset), 0.01f);
        }
    }
}