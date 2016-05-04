using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class CollisionHandler : MonoBehaviour
    {
        public Actor owner;
        public bool inBlock;
        public ParticleSystem sparks;

        void OnCollisionEnter(Collision collision)
        {
            owner.weapon.collisionPrevention = 0.01f;
            owner.weapon.saberCoM.transform.position = collision.contacts[0].point;
            inBlock = true;
            GameObject other = collision.gameObject;
            Saber otherWeapon = other.GetComponentInParent<Saber>();
            if (otherWeapon == null) { return; }
            if (Team.isSameTeam(owner, otherWeapon.owner)) { return; }
            sparks.transform.position = collision.contacts[0].point;
            sparks.Play();
        }

        void OnCollisionExit(Collision collision)
        {
            owner.weapon.collisionPrevention = 1f;
            inBlock = false;
            sparks.Stop();
        }
    }
}