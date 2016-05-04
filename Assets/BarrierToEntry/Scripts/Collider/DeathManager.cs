using UnityEngine;
using System.Collections;
namespace BarrierToEntry
{
    public class DeathManager : MonoBehaviour
    {
        public Actor owner;

        public void OnTriggerEnter(Collider col)
        {
            if (!owner.Alive) return;

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

            owner.Die();
        }
    }
}