using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class NPC : Actor
    {

        // Use this for initialization
        void Start()
        {
            config = new ActorConfig(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void _UpdateDominantHand()
        {

        }

        protected override void _UpdateNonDominantHand()
        {

        }
    }
}