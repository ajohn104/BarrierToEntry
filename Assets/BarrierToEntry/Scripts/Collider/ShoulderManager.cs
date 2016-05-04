using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class ShoulderManager : MonoBehaviour
    {
        public UpperArmManager UpperArm;

        public void OnTriggerEnter(Collider col)
        {
            if (UpperArm.ArmRemoved) return;

            UpperArm.OnTriggerEnter(col);
        }
    }
}