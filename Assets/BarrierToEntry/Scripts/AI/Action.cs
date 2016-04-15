using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Action
    {
        protected NPC owner;
        public Action(NPC actor)
        {
            this.owner = actor;
        }
    }
}