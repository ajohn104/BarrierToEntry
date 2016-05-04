using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class LegManager : MonoBehaviour
    {
        public HipsManager Hips;

        void OnTriggerEnter(Collider col)
        {
            Hips.OnTriggerEnter(col);
        }
    }
}
