using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class AITactics
    {
        private NPC owner;
        private Tactic currentTactic;
        public Actor currentTarget;

        public const float MIN_DIST = 1.1f;
        private const float AttackDelay = 2f;
        private float lastAttackTime = 0f;
        private const float MAX_ATTACK_DIST = 1.4f;

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
            if (currentTactic != null && !currentTactic.CanReact()) return;

            DecideTarget();
            if (CheckForDanger())
            {
                if (currentTactic == null || !(currentTactic is Defensive))
                {
                    currentTactic = new Defensive(this.owner);
                }
            }
            else
            {
                if ((currentTactic == null || !(currentTactic is Offensive)) && lastAttackTime >= AttackDelay && TargetWithinRange)
                {
                    currentTactic = new Offensive(this.owner);
                    lastAttackTime = 0f;
                }
            }
            if(currentTactic == null)
            {
                // Go into ready stance, eventually
                currentTactic = new Defensive(this.owner);
                // but honestly this is fine for now.
            }
        }

        private bool TargetWithinRange
        {
            get { return Actor.Distance(owner, currentTarget) <= MAX_ATTACK_DIST; }
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
                //Vector3[] lastPos = currentTarget.observer.lastPositions; // currently not used. It doesn't really contribute enough to an attack to be worth checking

                Quaternion[] lastRot = currentTarget.observer.lastRotations;
                float angleSum = 0f;
                for (int j = 0; j < lastRot.Length - 1; j++)
                {
                    angleSum += Quaternion.Angle(lastRot[j], lastRot[j + 1]);
                }
                
                return angleSum > 50;
            }

            return false;
        }

        private void MoveTowardTarget()
        {
            if (Actor.Distance(owner, currentTarget) < MIN_DIST)
            {
                owner.LocalMoveSpeed = Vector2.zero;
                return;
            }
            owner.LocalMoveSpeed = Actor.GenerateMoveSpeed(owner.transform.InverseTransformPoint(currentTarget.transform.position), MIN_DIST);
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

            currentTarget = owner.FindClosestEnemy() ?? enemies[0];
            float closest = DistanceTo(currentTarget);
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
            MoveTowardTarget();
            currentTactic.Perform();
            if(currentTactic is Offensive)
            {
                if (((Offensive)currentTactic).isDone)
                {
                    currentTactic = null;
                }
            } else
            {
                lastAttackTime += Time.fixedDeltaTime;
            }
            
        }
    }
}