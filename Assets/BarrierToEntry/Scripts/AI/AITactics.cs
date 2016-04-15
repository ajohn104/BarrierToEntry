using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class AITactics
    {
        private NPC owner;
        private Tactic currentTactic;
        public Actor currentTarget;
        
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
            DecideTarget();
            if (CheckForDanger())
            {
                if (currentTactic == null || (currentTactic.GetType() != typeof(Defensive)))
                {
                    currentTactic = new Defensive(this.owner);
                }
            }
            else
            {
                if (currentTactic == null || (currentTactic.GetType() != typeof(Defensive)))
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
            if (currentTarget == null) return false;

            if (currentTarget.GetType() == typeof(Player))
            {
                Vector3[] lastPos = currentTarget.observer.lastPositions;
                string s = "";
                for (int i = 0; i < lastPos.Length - 1; i++ )
                {
                    s += ", " + (lastPos[i] - lastPos[i + 1]);
                }
                //Debug.Log(s);

                Quaternion[] lastRot = currentTarget.observer.lastRotations;
                float angleSum = 0f;
                string s2 = "";
                for (int j = 0; j < lastPos.Length - 1; j++)
                {
                    s2 += ", " + (lastRot[j].eulerAngles - lastRot[j + 1].eulerAngles);
                    angleSum += Quaternion.Angle(lastRot[j], lastRot[j + 1]);
                }
                //Debug.Log(s2 + " : " + angleSum);
                //if (angleSum > 50) Debug.Log(angleSum); // In a really basic sense, this could be my danger check. For now. TODO: Make a less dumb dangerCheck
                return angleSum > 50;
            }

            return false;
        }

        /// <summary>
        /// Reevaluates the current target to be the closest enemy to the owner
        /// </summary>
        private void DecideTarget()
        {
            if(owner.enemyTeam.members.Length == 0)
            {
                Debug.Log("No enemies found.");
                return;
            }
            Actor[] enemies = owner.enemyTeam.members;
            currentTarget = currentTarget ?? enemies[0];        // Doesn't deal with dead enemies, obv. TODO: Needs to.
            float closest = DistanceTo(currentTarget);
            
            foreach (Actor enemy in enemies)
            {
                float dist = DistanceTo(enemy);
                if (dist < closest)
                {
                    closest = dist;
                    currentTarget = enemy; 
                }
            }
        }

        /// <summary>
        /// Calculates the distance of the owner to a specified actor.
        /// </summary>
        /// <param name="actor">the enemy to measure against</param>
        /// <returns>the distance between owner and the actor</returns>
        public float DistanceTo(Actor actor)
        {
            return Vector3.Distance(owner.transform.position, actor.transform.position);
        }

        /// <summary>
        /// Actually performs the actions dictated by the current tactic.
        /// </summary>
        private void PerformTactic()
        {
            currentTactic.Perform();
        }
    }
}