using UnityEngine;
using System.Collections;
using System;

namespace BarrierToEntry
{
    public class NPC : Actor
    {
        private ActorConfig _config;
        public override ActorConfig config
        {
            get { return _config; }
            set { _config = value; }
        }
        public AITactics tactics;
        

        // Use this for initialization
        void Start()
        {
            config = new ActorConfig(this);
            config.GenerateArmLength();

            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            Physics.IgnoreCollision(collider, weapon.collider);
            modelDesign.Prepare();
            ModelGenerator.RandomizeModel(this);
            tactics = new AITactics(this);
            _observer = new Observer(this);
        }

        int iffy = 0;
        protected override void Think()
        {
            //if (++iffy%100 == 0) ModelGenerator.RandomizeModel(this);
            ConsiderTactics();

            //_UpdateDominantHand();   
        }

        protected override void _UpdateDominantHand()
        {
            //this.DominantHandPos = new Vector3(0.151f, 1.139f, 0.562f);
            //weapon.target.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            //this.RotationSpeedHoriz = this.transform.
            
        }

        protected void ConsiderTactics()
        {
            if(tactics.currentTarget != null)
            {
                Vector3 newRot = Quaternion.LookRotation(tactics.currentTarget.transform.position - transform.position).eulerAngles;
                newRot.x = 0;
                newRot.z = 0;
                Quaternion newQuat = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRot), Time.fixedDeltaTime * 8f);
                float offset = (Quaternion.Inverse(newQuat) * transform.rotation).eulerAngles.y - 180f;//newQuat.eulerAngles.y - transform.rotation.eulerAngles.y;
                this.RotationSpeedHoriz = Mathf.Clamp(offset/2f, -1f, 1f);
            }
            
            tactics.EvaluateNextMove();
        }

        protected override void _UpdateNonDominantHand()
        {
            throw new NotImplementedException();
        }

        protected override void Feedback(float errorOffset, float errorAngle) { /* purposefully ignored for ai */ }

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

            // Still need to put non-dom hand on weapon.
            
            if (tactics.currentTarget != null)
            {
                anim.SetLookAtWeight(1.0f);
                anim.SetLookAtPosition(tactics.currentTarget.anim.GetBoneTransform(HumanBodyBones.Head).position);
            }
        }
    }
}