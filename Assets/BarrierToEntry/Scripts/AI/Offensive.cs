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

        Attack attack;
        public bool isDone = false;

        public override void Perform()
        {
            if (attack == null) attack = new Attack(owner);
            isDone = attack.Update();

        }
    }
}