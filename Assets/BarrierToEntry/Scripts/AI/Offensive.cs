using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Offensive : Tactic
    {
        public Offensive(NPC actor) : base(actor)
        {
            this.owner = actor;
        }
    }
}