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
        public GameObject pointer;

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

            controls = new Controls(device, this);
            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            //Physics.IgnoreCollision(collider, weapon.collider);

            modelDesign.Prepare();
            _observer = new Observer(this);
            DominantIK = weapon.gameObject;
            Time.timeScale = 0;
        }

        public void Update()
        {
            if (!controls.InputCheck()) return;
            CheckRecenter();
            CheckCalibrateShoulder();
            CheckCalibrateUserArmLength();
            CheckChangeBeamColorUp();
            CheckChangeBeamColorDown();
            CheckMovementInput();
            CheckObjectThrow();
            CheckGrab();
            CheckReleaseGrab();
            CheckResetLevel();
            CheckEndGame();
            CheckDisplayPointer();
            // CheckStopTime();     // Only necessary for making screenshots easier to produce, really.
        }
        
        protected override void Think()     // TODO: Move control stuff in Player.Think to Controls.cs
        {
            if (!controls.InputCheck()) return;

            _UpdateDominantHand();
            _UpdateNonDominantHand();
        }

        public void LateUpdate()
        {
            if (!controls.InputCheck()) return;
            CheckDisplaySaberTransform();
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
                if (VRCenter.VREnabled) { VRCenter.Recenter(); }
                Time.timeScale = 1f;
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

            this.NonDominantHandRot *= Quaternion.Euler(new Vector3(0, -20, 0));
        }

        protected void CheckMovementInput()
        {
            this.MoveSpeedForward = controls.controllerLeft.JoystickY*1.2f;
            this.MoveSpeedStrafe = controls.controllerLeft.JoystickX * 1.2f;
            this.RotationSpeedHoriz = controls.controllerRight.JoystickX;
            this.DominantHandPos = controls.controllerRight.Position + _config.rightCalibOffset;     // TODO: Make all this shit dependent on dominant/nondominant hand (use enums, prob)
        }

        private int currentSaberColor = 5;

        private void CheckChangeBeamColorUp()
        {
            if(controls.ChangeBeamColorUp)
            {
                this.modelDesign.LockIn(false);
                currentSaberColor++;
                currentSaberColor %= ModelGenerator.beamColors.Length;
                modelDesign.SetColor(BodyPart.BEAM, ModelGenerator.beamColors[currentSaberColor]);
            }
        }

        private void CheckChangeBeamColorDown()
        {
            if (controls.ChangeBeamColorDown)
            {
                this.modelDesign.LockIn(false);
                currentSaberColor--;
                if(currentSaberColor < 0) { currentSaberColor = ModelGenerator.beamColors.Length - 1; }
                modelDesign.SetColor(BodyPart.BEAM, ModelGenerator.beamColors[currentSaberColor]);
            }
        }

        private void CheckDisplayPointer()
        {
            pointer.SetActive(controls.DisplayPointer);
        }

        private void CheckDisplaySaberTransform()
        {
            if(controls.DisplaySaberTransform) {
                Debug.Log("The current weapon position is: " + DominantHandPos);
                Debug.Log("The current weapon rotation is: " + weapon.target.localRotation.eulerAngles);
                Debug.Log("=====================================================================");
            }
        }

        bool timePaused = false;
        private void CheckStopTime()
        {
            if (controls.StopTime)
            {
                timePaused = !timePaused;
                Time.timeScale = (timePaused) ? 0f : 1f;
            }
        }

        public bool InGrab = false;
        Rigidbody[] heldObjects = new Rigidbody[0];

        private void addRigidBody(Rigidbody body)
        {
            Saber attachedWeapon;
            if (body == this.rb) return;
            if (body == null) return;
            if (body.GetComponent<Saber>() != null)
            {
                attachedWeapon = body.GetComponent<Saber>();
                if (attachedWeapon.owner.DominantHandAttached) return;
            }

            foreach (Rigidbody oldBody in heldObjects)
            {
                if (oldBody == body) return;
            }
            Rigidbody[] newObjects = new Rigidbody[heldObjects.Length + 1];
            System.Array.Copy(heldObjects, newObjects, heldObjects.Length);
            newObjects[heldObjects.Length] = body;
            heldObjects = newObjects;
            if (body.GetComponent<Actor>() != null)
            {
                body.GetComponent<Actor>().CanMove = false;
            }
        }

        private void CheckGrab()
        {
            if(controls.Grab)
            {
                InGrab = true;
                //RaycastHit[] hits = Physics.SphereCastAll(hand.position, 0.7f, -1 * (hand.transform.rotation * Vector3.forward), Mathf.Infinity, 1, QueryTriggerInteraction.Collide);
                RaycastHit[] hits = Physics.SphereCastAll(hand.position, 5f, (hand.transform.up), Mathf.Infinity, 1, QueryTriggerInteraction.Collide);
                foreach (RaycastHit hit in hits)
                {
                    addRigidBody(hit.rigidbody);
                }
            }
        }


        private Vector3 lastHandPos;
        private void CheckObjectThrow()
        {
            if (lastHandPos == null) lastHandPos = hand.localPosition;
            Vector3 handMovement = hand.localPosition - lastHandPos;
            if (InGrab)
            {
                foreach(Rigidbody body in heldObjects)
                {
                    body.useGravity = false;
                    //body.AddForce(handMovement * 20f, ForceMode.VelocityChange);
                    //body.MovePosition(body.position + handMovement * 20f);
                    body.velocity = transform.rotation * handMovement * 600f; // Quaternion.Inverse(hand.transform.rotation)
                }
            }
            lastHandPos = hand.localPosition;
        }

        private void CheckReleaseGrab()
        {
            if (controls.ReleaseGrab)
            {
                foreach(Rigidbody body in heldObjects)
                {
                    if (body.GetComponent<Saber>() == null) {
                        body.useGravity = true;
                        if(body.GetComponent<Actor>() != null)
                        {
                            body.GetComponent<Actor>().CanMove = true;
                        }
                    } else if(!body.GetComponent<Saber>().owner.DominantHandAttached)
                    {
                        body.useGravity = true;
                    }

                        
                }

                InGrab = false;
                heldObjects = new Rigidbody[0];
            }
        }

        void CheckResetLevel()
        {
            if(controls.ResetLevel)
            {
                Application.LoadLevel(0);
            }
        }

        void CheckEndGame()
        {
            if(controls.EndGame)
            {
                Application.Quit();
            }
        }

        void OnDrawGizmos()
        {
            if (controls == null || !controls.InputCheck()) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hand.transform.position, hand.transform.position + -6 * (hand.transform.rotation * Vector3.forward));
            /*Gizmos.color = Color.red;
            Gizmos.DrawLine(weapon.transform.position, weapon.transform.position + weapon.target.up);

            Gizmos.DrawSphere(transform.TransformPoint(controls.controllerRight.Position + _config.rightCalibOffset), 0.01f);*/
        }

        void OnAnimatorIK()
        {
            Quaternion computedRot = Quaternion.Euler(DominantIK.transform.rotation.eulerAngles) * Quaternion.Euler(HandGripIKOffset);
            Quaternion computedRot2 = Quaternion.Euler(hand.rotation.eulerAngles) * Quaternion.Euler(handIKOffset);// * transform.rotation;

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            anim.SetIKPosition(AvatarIKGoal.RightHand, DominantIK.transform.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, computedRot);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, hand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, computedRot2);
        }
    }
}