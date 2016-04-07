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
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            config = new ActorConfig(this);
            config.GenerateArmLength();

            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            Physics.IgnoreCollision(collider, weapon.collider);
            modelDesign.Prepare();
            ModelGenerator.RandomizeModel(this);
        }

        int iffy = 0;
        protected override void Think()
        {
            this.DominantHandPos = new Vector3(0.151f, 1.139f, 0.562f);
            weapon.target.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        protected override void _UpdateDominantHand()
        {
            //weapon.target.localRotation = thought and stuff
            throw new NotImplementedException();
        }

        protected override void _UpdateNonDominantHand()
        {
            throw new NotImplementedException();
        }

        protected override void Feedback(float errorOffset, float errorAngle) { /* purposefully ignored for ai */ }
    }
}