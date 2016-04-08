using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Tactic 
    {
        protected NPC owner;

        public Tactic(NPC actor)
        {
            this.owner = actor;
        }
    }
}