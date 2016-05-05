using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Attack : Action
    {
        float time = 0f;
        AttackPattern pattern;
        public Attack(NPC actor) : base(actor)
        {
            this.owner = actor;
            //pattern = AttackPattern.GetPattern(Mathf.FloorToInt(Random.value * 3f), owner.domhandpos, owner.weapon.target.localRotation.eulerAngles);
            pattern = AttackPattern.GetPattern(AttackPattern.RIGHT_TO_LEFT, owner.domhandpos, owner.weapon.target.localRotation.eulerAngles); // Using this for now since others aren't done
        }

        public bool Update()
        {
            
            if(pattern.isDone()) {
                return true;
            }
            if(!owner.weapon.collisionHandler.inBlock) time += Time.fixedDeltaTime;
            Vector3 pos = pattern.GetExpectedPosition(time);
            Vector3 rot = pattern.GetExpectedRotation(time);

            owner.weapon.target.localRotation = Quaternion.Euler(rot);
            owner.domhandpos = pos;

            return false;
        }
    }
}
