using UnityEngine;
using System;  // Needed for Math
using BarrierToEntry;

namespace BarrierToEntry
{

    public class SaberWaver : MonoBehaviour
    {
        // un-optimized version
        public float scale = 2f;
        public Actor actor;
        public new AudioSource audio;

        private int time = 0;

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (actor.observer == null) return;
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= Mathf.Lerp(1f, 5f, actor.observer.AverageRotationalSpeed * scale);
            }
        }
    }
}