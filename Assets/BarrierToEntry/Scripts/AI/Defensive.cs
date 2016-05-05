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

            float oldZ = owner.weapon.target.localEulerAngles.z;
            owner.weapon.target.localRotation = Quaternion.Inverse(owner.transform.rotation)*enemy.observer.IdealBlockRotation;//enemy.weapon.target.transform.localRotation;
            Vector3 centerBeamPosFar = enemy.observer.IdealBlockPosition;
            Vector3 shoulderOffset = centerBeamPosFar - owner.anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;

            shoulderOffset = shoulderOffset.normalized * Mathf.Clamp(shoulderOffset.magnitude, 0f, owner.config.ArmLength/2);
            Vector3 finalPos = shoulderOffset + owner.anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;

            float offset = Vector3.Distance(owner.weapon.transform.position, owner.observer.IdealBlockPosition);
            Vector3 posOffset = owner.weapon.target.rotation * (offset * Vector3.forward);
            //finalPos -= posOffset;
            owner.domhandpos = owner.transform.InverseTransformPoint(finalPos);

        }
    }
}