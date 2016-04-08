using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Defensive : Tactic
    {
        public Defensive(NPC actor) : base(actor) {
            this.owner = actor;
        }
    }
}