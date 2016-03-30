using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class CollisionHandler : MonoBehaviour
    {
        public PlayerControls controls;
        public bool inBlock;

        void OnCollisionEnter(Collision collision)
        {
            controls.collisionPrevention = 0.01f;
            controls.saberCoM.transform.position = collision.contacts[0].point;
            //controls.rbSaber.centerOfMass = controls.rbSaber.transform.InverseTransformPoint(controls.saberCoM.position);
            inBlock = true;
        }

        void OnCollisionExit(Collision collision)
        {
            controls.collisionPrevention = 1f;
            inBlock = false;
        }
    }
}