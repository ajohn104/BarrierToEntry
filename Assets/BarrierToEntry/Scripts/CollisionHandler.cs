using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class CollisionHandler : MonoBehaviour
    {
        public Actor actor;
        public bool inBlock;

        void OnCollisionEnter(Collision collision)
        {
            actor.weapon.collisionPrevention = 0.01f;
            actor.weapon.saberCoM.transform.position = collision.contacts[0].point;
            //controls.rbSaber.centerOfMass = controls.rbSaber.transform.InverseTransformPoint(controls.saberCoM.position);
            inBlock = true;
        }

        void OnCollisionExit(Collision collision)
        {
            actor.weapon.collisionPrevention = 1f;
            inBlock = false;
        }
    }
}