using UnityEngine;
using System.Collections;
namespace BarrierToEntry
{
    public class LowerArmManager : MonoBehaviour
    {
        public UpperArmManager UpperArm;
        public bool LowerArmRemoved = false;

        private HumanBodyBones ArmRemovalPoint
        {
            get { return UpperArm.Arm == Hand.Left ? HumanBodyBones.LeftLowerArm : HumanBodyBones.RightLowerArm; }


        }

        public void OnTriggerEnter(Collider col)
        {
            if (UpperArm.ArmRemoved || LowerArmRemoved) return;

            GameObject other = col.gameObject;
            Saber otherWeapon = other.GetComponentInParent<Saber>();

            if (!RagdollSpawner.DebugEnabled)
            {
                if (otherWeapon == null) { return; }
                if (Team.isSameTeam(UpperArm.owner, otherWeapon.owner) || UpperArm.owner == otherWeapon.owner) { return; }
            }
            else
            {
                if (otherWeapon != null) { return; }
            }

            LowerArmRemoved = true;

            UpperArm.owner.anim.GetBoneTransform(ArmRemovalPoint).localScale = RagdollSpawner.ScaleDown * Vector3.one;

            UpperArm.DisableTool();
        }
    }
}