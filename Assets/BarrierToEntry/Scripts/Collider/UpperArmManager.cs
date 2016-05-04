using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class UpperArmManager : MonoBehaviour
    {
        public Actor owner;
        public bool ArmRemoved = false;
        public Hand Arm;
        private bool ToolDisabled = false;

        public void OnTriggerEnter(Collider col)
        {
            if (ArmRemoved) { return; }

            GameObject other = col.gameObject;
            Saber otherWeapon = other.GetComponentInParent<Saber>();

            if (!RagdollSpawner.DebugEnabled)
            {
                if (otherWeapon == null) { return; }
                if (Team.isSameTeam(owner, otherWeapon.owner)) { return; }
            }
            else
            {
                if (otherWeapon != null) { return; }
            }

            ArmRemoved = true;

            transform.localScale = RagdollSpawner.ScaleDown * Vector3.one;

            DisableTool();
        }
        
        // Like many of these limb things, it could use a little visual/audio (preferably visual) feedback too...
        public void DisableTool()
        {
            if (ToolDisabled) return;
            owner.DisableHand(Arm);
            ToolDisabled = true;
        }
    }
}