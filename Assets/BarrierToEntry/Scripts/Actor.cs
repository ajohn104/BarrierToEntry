using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public abstract class Actor : MonoBehaviour
    {
        public Transform hand;
        public Animator anim;
        public Rigidbody rb;        // TODO: USE THIS. Maybe. I don't knpw for sure that anim responds to physics.
        public Saber saber;
        public ActorConfig config;

        // Use this for initialization
        void Start()
        {
            config = new ActorConfig(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void UpdateDominantHand()
        {

            _UpdateDominantHand();
        }

        protected abstract void _UpdateDominantHand();

        protected void UpdateNonDominantHand()
        {

            _UpdateNonDominantHand();
        }

        protected abstract void _UpdateNonDominantHand();


    }
}