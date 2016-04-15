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

        public abstract void Perform();
    }
}