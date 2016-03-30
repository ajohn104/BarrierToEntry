using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class ActorConfig
    {
        protected Actor owner;

        protected float armLength = 0f;
        private readonly HumanBodyBones[] rightArmBones = new HumanBodyBones[] {
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand
        };

        protected void GenerateArmLength()
        {
            for (int i = 0; i < rightArmBones.Length - 1; i++)
            {
                Vector3 bone1Position = owner.anim.GetBoneTransform(rightArmBones[i]).position;
                Vector3 bone2Position = owner.anim.GetBoneTransform(rightArmBones[i + 1]).position;
                armLength += Vector3.Distance(bone1Position, bone2Position);
            }
        }

        public ActorConfig(Actor owner)
        {
            this.owner = owner;
        }
    }
}