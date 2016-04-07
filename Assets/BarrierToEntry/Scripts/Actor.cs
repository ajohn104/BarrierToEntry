using UnityEngine;
using System;
using System.Collections;

namespace BarrierToEntry
{
    public abstract class Actor : MonoBehaviour
    {
        public Transform hand;
        public Animator anim;
        public Rigidbody rb;        // TODO: USE THIS. Maybe. I don't knpw for sure that anim responds to physics.
        public Saber weapon;
        public new Collider collider;
        public virtual ActorConfig config
        {
            get; set;
        }

        protected float MoveSpeedForward;
        protected float MoveSpeedStrafe;
        protected float RotationSpeedHoriz;

        protected Vector3 DominantHandPos;
        protected Vector3 NonDominantHandPos;
        protected Quaternion DominantHandRot;   // Currently not used. weapon.target.transform is used instead
        protected Quaternion NonDominantHandRot;

        private readonly Vector3 HandGripIKOffset = new Vector3(0, -180, -90);
        private readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);

        public ModelDesigner modelDesign;

        protected void GenerateMoveSpeed(float positionForward, float positionStrafe)
        {
            throw new NotSupportedException("TODO: Implement Actor.GenerateMoveSpeed");
        }

        // Update is called once per frame
        void Update()
        {
            Think();
            Act();
        }

        void Start()
        {
            config = new ActorConfig(this);
            config.GenerateArmLength();
            
            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            Physics.IgnoreCollision(collider, weapon.collider);

            modelDesign.Prepare();
        }

        protected abstract void Think();

        private void Act()
        {
            anim.SetFloat("Forward", MoveSpeedForward);
            transform.Rotate(new Vector3(0, -3f * (Mathf.Rad2Deg * Mathf.Acos(RotationSpeedHoriz) - 90f) / 180f, 0));        // TODO: Make this better. I think this is geared for a thumbstick
            MoveDominantHand();
            MoveNonDominantHand();
        }

        void MoveDominantHand()
        {
            Vector3 calculatedGripPosition = transform.TransformPoint(DominantHandPos);
            Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 rightArmOffset = calculatedGripPosition - rightArmPos;

            if (rightArmOffset.magnitude > config.ArmLength)       // Really only intended for the player but whatever
            {
                calculatedGripPosition = rightArmPos + (config.ArmLength / rightArmOffset.magnitude) * rightArmOffset;
            }

            Vector3 moveOffset = calculatedGripPosition - weapon.rb.position;
            Vector3 partialMove = Vector3.Lerp(weapon.rb.position, calculatedGripPosition, weapon.collisionPrevention);
            weapon.rb.MovePosition(partialMove);

            weapon.rb.MoveRotation(Quaternion.Lerp(weapon.rb.rotation, weapon.target.rotation, weapon.collisionPrevention));

            Feedback(moveOffset.magnitude, Mathf.Abs(Quaternion.Angle(weapon.rb.rotation, weapon.target.rotation)));
        }

        void MoveNonDominantHand()
        {
            hand.position = transform.rotation * (NonDominantHandPos) + transform.position;

            hand.localRotation = NonDominantHandRot;
        }

        protected abstract void Feedback(float errorOffset, float errorAngle);

        /// <summary>Turns the actor in the y axis towards theta degrees.</summary>
        protected void Turn(float theta)
        {
            throw new NotSupportedException("TODO: Implement Actor.Turn better than Actor.Act does that meshes well with it");
        }

        /// <summary>Moves the actor towards posX in the local x axis and posZ in the local z axis.</summary>
        protected void Move(float posX, float posZ)
        {
            throw new NotSupportedException("TODO: Implement Actor.Move better than Actor.Act does that meshes well with it");
        }


        // Note, the extra script space here is in case of extra necessary massaging, not to do what Actor.Move* do.
        protected void UpdateDominantHand()
        {

            _UpdateDominantHand();
        }

        /// <summary>
        /// Used to update the desired orientation and position of the dominant hand
        /// </summary>
        protected abstract void _UpdateDominantHand();

        
        protected void UpdateNonDominantHand()
        {

            _UpdateNonDominantHand();
        }

        /// <summary>
        /// Used to update the desired orientation and position of the nondominant hand
        /// </summary>
        protected abstract void _UpdateNonDominantHand();

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
    }
}