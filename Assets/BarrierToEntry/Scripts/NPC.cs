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
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void Think()
        {
            throw new NotImplementedException();
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