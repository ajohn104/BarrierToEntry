using UnityEngine;
using System.Collections;
using XInputDotNetPure;

namespace BarrierToEntry
{

    public class HapticsManager : MonoBehaviour
    {
        public CollisionHandler blockManager;
        public PlayerControls player;

        bool playerIndexSet = false;
        PlayerIndex playerIndex;
        GamePadState state;
        GamePadState prevState;

        bool vibeEnabled = false;
       
        const float saberErrorMax = 0.5f;
        float _blockWeight { get { return blockManager.inBlock?1f:0f; } }
        float _blockErrorWeight {
            get {
                float errorDist = blockManager.inBlock ? player.saberErrorDist : 0f;
                float rumbleMag =  Mathf.Clamp(errorDist, 0f, saberErrorMax)*2f;
                return rumbleMag;
            }
        }
        
        void Update()
        {
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (vibeEnabled) GamePad.SetVibration(playerIndex, _blockErrorWeight, _blockWeight);
            else GamePad.SetVibration(playerIndex, 0f, 0f);
        }
    }

}