using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public abstract class Tactic 
    {
        protected NPC owner;

        public Tactic(NPC actor)
        {
            this.owner = actor;
        }
        
        abstract protected float reactionTime { get; set; }
        protected float timePassed = 0f;

        public abstract void Perform();

        public bool CanReact()
        {
            return timePassed >= reactionTime;
        }
    }
}