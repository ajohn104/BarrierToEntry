using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Offensive : Tactic
    {
        Attack attack;
        public bool isDone = false;

        public Offensive(NPC actor) : base(actor)
        {
            this.owner = actor;
        }

        private float _reactionTime = 0.5f;
        protected override float reactionTime
        {
            get
            {
                return _reactionTime;
            }

            set
            {
                _reactionTime = value;
            }
        }
        
        public override void Perform()
        {
            if (attack == null) attack = new Attack(owner);
            reactionTime += Time.fixedDeltaTime;
            isDone = attack.Update();

        }
    }
}