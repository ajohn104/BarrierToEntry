using UnityEngine;
using System.Collections;
using BarrierToEntry;

public class CollisionHandler : MonoBehaviour {
    public PlayerControls controls;

    void OnCollisionEnter(Collision collision)
    {
        controls.collisionPrevention = 0.01f;
    }
    
    void OnCollisionExit(Collision collision)
    {
        controls.collisionPrevention = 1f;
    }
}
