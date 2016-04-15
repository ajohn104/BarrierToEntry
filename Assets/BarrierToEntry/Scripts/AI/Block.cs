using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Block : Action
    {
        public Block(NPC actor) : base(actor)
        {
            this.owner = actor;
        }
    }
}