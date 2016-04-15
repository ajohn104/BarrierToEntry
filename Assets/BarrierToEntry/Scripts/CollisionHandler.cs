using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class CollisionHandler : MonoBehaviour
    {
        public Actor actor;
        public bool inBlock;
        public ParticleSystem sparks;

        void OnCollisionEnter(Collision collision)
        {
            actor.weapon.collisionPrevention = 0.01f;
            actor.weapon.saberCoM.transform.position = collision.contacts[0].point;
            //controls.rbSaber.centerOfMass = controls.rbSaber.transform.InverseTransformPoint(controls.saberCoM.position);
            inBlock = true;
            sparks.transform.position = collision.contacts[0].point;
            sparks.Play();
        }

        void OnCollisionExit(Collision collision)
        {
            actor.weapon.collisionPrevention = 1f;
            inBlock = false;
            sparks.Stop();
        }
    }
}