using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Attack : Action
    {
        public Attack(NPC actor) : base(actor)
        {
            this.owner = actor;
        }
    }
}
