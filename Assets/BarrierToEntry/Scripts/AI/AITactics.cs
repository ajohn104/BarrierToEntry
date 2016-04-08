using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class AITactics
    {
        private NPC owner;
        private Tactic currentTactic;
        
        public AITactics(NPC actor)
        {
            this.owner = actor;
        }

        public void EvaluateNextMove()
        {
            DecideTactic();
            PerformTactic();
        }

        private void DecideTactic()
        {
            if (CheckForDanger())
            {
                if (!(currentTactic.GetType() != typeof(Defensive)))
                {
                    currentTactic = new Defensive(this.owner);
                }
            }
            else
            {
                if (!(currentTactic.GetType() != typeof(Defensive)))
                {
                    currentTactic = new Offensive(this.owner);
                }
            }
        }

        /// <summary>
        /// Checks to see if an enemy is attempting to attack.
        /// </summary>
        /// <returns>true if blocking is recommended</returns>
        private bool CheckForDanger()
        {
            // This'll be an uphill battle...
            return false;
        }

        /// <summary>
        /// Actually performs the actions dictated by the current tactic
        /// </summary>
        private void PerformTactic()
        {

        }
    }
}