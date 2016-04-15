using UnityEngine;
using System;  // Needed for Math
using BarrierToEntry;

namespace BarrierToEntry
{

    public class SaberWaver : MonoBehaviour
    {
        // un-optimized version
        public float scale = 2f;
        public Player player;

        private int time = 0;

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (player.observer == null) return;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= Mathf.Lerp(1f, 5f, player.observer.AverageRotationalSpeed * scale);
            }
        }
    }
}