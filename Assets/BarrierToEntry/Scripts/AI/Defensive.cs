using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Defensive : Tactic
    {
        private Block block;
        public Defensive(NPC actor) : base(actor) {
            this.owner = actor;
            block = new Block(owner);
        }
        
        public override void Perform()
        {
            // This probably should go in block but right now I don't care.
            Vector3 blockPosition = Vector3.zero;
            Quaternion blockRotation = Quaternion.Euler(Vector3.zero);

            Actor enemy = owner.tactics.currentTarget;
            Observer obs = enemy.observer;

            owner.domhandpos = enemy.domhandpos;    // Tracks enemy for testing purposes. Will change when I get the math right.
            
            owner.weapon.target.localRotation = enemy.weapon.target.transform.localRotation;

        }
    }
}