using UnityEngine;
using System.Collections;
using System;

namespace BarrierToEntry {
    public class Observer {
        private Actor owner;
        private Vector3[] _lastPositions;
        private Quaternion[] _lastRotations;
        public int trackAmount = 5;
        public Observer(Actor actor)
        {
            this.owner = actor;
            _lastPositions = new Vector3[trackAmount];
            _lastRotations = new Quaternion[trackAmount];
        }

        public void observe()
        {
            Vector3[] newLastPos = new Vector3[trackAmount];
            Array.ConstrainedCopy(_lastPositions, 0, newLastPos, 1, trackAmount - 1);
            newLastPos[0] = owner.weapon.transform.position;
            _lastPositions = newLastPos;

            Quaternion[] newLastRot = new Quaternion[trackAmount];
            Array.ConstrainedCopy(_lastRotations, 0, newLastRot, 1, trackAmount - 1);
            newLastRot[0] = owner.weapon.transform.rotation;
            _lastRotations = newLastRot;
        }

        /// <summary>
        /// Provides the last few recorded positions of the weapon, 0 as most recent.
        /// </summary>
        public Vector3[] lastPositions
        {
            get { return _lastPositions; }
        }

        public float AverageSpeed
        {
            get
            {
                float sum = 0f;
                for(int i = 0; i < _lastPositions.Length-1; i++)
                {
                    sum += Vector3.Distance(_lastPositions[i], _lastPositions[i + 1]) / 0.005f;
                }
                return sum / _lastPositions.Length;
            }
        }

        public float AverageRotationalSpeed
        {
            get
            {
                float sum = 0f;
                for(int i = 0; i < _lastRotations.Length-1; i++)
                {
                    sum += Quaternion.Angle(_lastRotations[i], _lastRotations[i + 1]);
                }
                return sum / _lastRotations.Length;
            }
        }

        public Quaternion IdealBlockRotation
        {
            get
            {
                return Quaternion.LookRotation(owner.weapon.transform.up, -owner.weapon.transform.right);
            }
        }

        /// <summary>
        /// Provides the last few recorded rotations, 0 as most recent.
        /// </summary>
        public Quaternion[] lastRotations
        {
            get { return _lastRotations; }
        }
    }
}
