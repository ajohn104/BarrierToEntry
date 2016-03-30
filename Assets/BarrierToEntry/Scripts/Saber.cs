﻿using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class Saber : MonoBehaviour
    {
        public Actor owner;
        public Rigidbody rb;
        public Transform target;

        private readonly Vector3 saberHandGripRotOffset = new Vector3(-90, 180, 0);
        private readonly Vector3 handRotOffset = new Vector3(180, 90, 90);

        public float collisionPrevention = 1f;
        public Transform saberCoM;

        public float rumbleCutoff = 10f;
        public float rumbleRange = 15f;


        private float _saberErrorDist = 0f;
        public float saberErrorDist { get { return _saberErrorDist; } }

        /* TODO: Add a position and rotation property in here for ease of access. It would make more sense, too. When reading certain lines of code. */

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}