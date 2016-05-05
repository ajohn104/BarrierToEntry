using UnityEngine;
using System;
using System.Collections;

namespace BarrierToEntry
{
    public abstract class Actor : MonoBehaviour
    {
        public Transform hand;
        public Animator anim;
        public Rigidbody rb;        // TODO: USE THIS. Maybe. I don't knpw for sure that anim responds to physics.
        public Saber weapon;
        public new Collider collider;
        public Team team = Team.NONE;
        public Team enemyTeam = Team.NONE;
        public virtual ActorConfig config
        {
            get; set;
        }

        protected float MoveSpeedForward;
        protected float MoveSpeedStrafe;
        protected const float MaxMoveSpeed = 5f;
        protected float RotationSpeedHoriz;
        protected const float MaxTurnSpeed = 100f;      // Initial acception. Pending user approval.
        public bool CanMove = true;

        protected Vector3 DominantHandPos;
        public Vector3 domhandpos
        {
            set { DominantHandPos = value; }
            get { return DominantHandPos; }
        }
        protected Vector3 NonDominantHandPos;
        protected Quaternion DominantHandRot;   // Currently not used. weapon.target.transform is used instead
        protected Quaternion NonDominantHandRot;

        protected readonly Vector3 HandGripIKOffset = new Vector3(0, -180, -90);
        protected readonly Vector3 handIKOffset = new Vector3(-90, 180, 0);

        public ModelDesigner modelDesign;

        protected Observer _observer;
        public Observer observer
        {
            get { return _observer; }
        }
        
        public GameObject DominantIK;
        public GameObject NonDominantIK;

        public bool DominantHandAttached = true;
        protected bool NonDominantHandAttached = true;

        public Vector2 LocalMoveSpeed
        {
            set {
                MoveSpeedStrafe = value.x;
                MoveSpeedForward = value.y;
            }
        }

        public bool Alive = true;

        public static Vector2 GenerateMoveSpeed(Vector3 localOffset, float minDist)
        {
            Vector2 offset = new Vector2(localOffset.x, localOffset.z);
            float dist = offset.magnitude;
            
            float altDist = Mathf.Clamp(dist - minDist, 0f, dist);
            float moveSpeed = altDist < MaxMoveSpeed * Time.fixedDeltaTime ? altDist / Time.fixedDeltaTime : MaxMoveSpeed ;
            moveSpeed = Mathf.Clamp01(moveSpeed);
            moveSpeed = dist < minDist ? 0f : moveSpeed;
            return offset.normalized * moveSpeed;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!Alive) return;
            _observer.observe();
            Think();
            Act();
        }

        void Start()
        {
            config = new ActorConfig(this);
            config.GenerateArmLength();
            
            weapon.rb.centerOfMass = weapon.rb.transform.InverseTransformPoint(weapon.saberCoM.position);
            // Physics.IgnoreCollision(collider, weapon.collider); // Instead, colliders will all be triggers, aside from the bottom collider that keeps them from falling through the floor.

            modelDesign.Prepare();
            _observer = new Observer(this);
            DominantIK = weapon.gameObject;
        }

        protected abstract void Think();

        private void Act()
        {
            if (CanMove)
            {
                anim.SetFloat("Forward", MoveSpeedForward);
                anim.SetFloat("Strafe", MoveSpeedStrafe);
                rb.MovePosition(transform.TransformPoint(new Vector3(MoveSpeedStrafe, 0f, MoveSpeedForward) * MaxMoveSpeed * Time.fixedDeltaTime));
                transform.Rotate(new Vector3(0, RotationSpeedHoriz * MaxTurnSpeed * Time.fixedDeltaTime, 0));        // TODO: Make this better. I think this is geared for a thumbstick
            }

            MoveDominantHand();
            MoveNonDominantHand();
        }

        public void Die()
        {
            if (!Alive) return;
            rb.constraints = RigidbodyConstraints.None;
            rb.angularDrag = 0f;
            anim.enabled = false;
            DisableHand(Hand.Left);
            DisableHand(Hand.Right);
            Ragdoll();
            Alive = false;
            
            // TODO: Fully ragdoll the body, then also add a game over state for player (or just let them respawn because who cares).
        }

        private void Ragdoll()
        {
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.isTrigger = false;
            }
            collider.enabled = false;
        }

        public void DisableHand(Hand side)
        {
            // TODO: Implement. It'll need some stuff to be messed with elsewhere, probably.
            if(side == config.DominantHand )
            {
                if (!DominantHandAttached) return;
                DominantHandAttached = false;
                weapon.rb.useGravity = true;
                DominantIK = weapon.target.gameObject;
            } else
            {
                if (!NonDominantHandAttached) return;
                NonDominantHandAttached = false;
                weapon.NonDomHand = anim.GetBoneTransform(side == Hand.Right ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand); // Eh, it'll get the job done at least.
            }
            // That might be all I need to do, honestly.
        }

        void MoveDominantHand()
        {

            Vector3 calculatedGripPosition = transform.TransformPoint(DominantHandPos);
            Vector3 rightArmPos = anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Vector3 rightArmOffset = calculatedGripPosition - rightArmPos;

            if (rightArmOffset.magnitude > config.ArmLength)       // Really only intended for the player but whatever
            {
                calculatedGripPosition = rightArmPos + (config.ArmLength / rightArmOffset.magnitude) * rightArmOffset;
            }

            if (!DominantHandAttached)
            {
                weapon.target.position = calculatedGripPosition;
                return;
            }

            Vector3 moveOffset = calculatedGripPosition - weapon.rb.position;
            Vector3 partialMove = Vector3.Lerp(weapon.rb.position, calculatedGripPosition, weapon.collisionPrevention);
            weapon.rb.MovePosition(partialMove);

            weapon.rb.MoveRotation(Quaternion.Lerp(weapon.rb.rotation, weapon.target.rotation, weapon.collisionPrevention));

            Feedback(moveOffset.magnitude, Mathf.Abs(Quaternion.Angle(weapon.rb.rotation, weapon.target.rotation)));
        }

        void MoveNonDominantHand()
        {
            //if (!NonDominantHandAttached) return; // Not sure I need it... we'll see what IK tries to do if the hand is disabled.
            hand.position = transform.rotation * (NonDominantHandPos) + transform.position;

            hand.localRotation = NonDominantHandRot;
        }

        protected abstract void Feedback(float errorOffset, float errorAngle);

        /// <summary>Turns the actor in the y axis towards theta degrees.</summary>
        protected void Turn(float theta)
        {
            throw new NotSupportedException("TODO: Implement Actor.Turn better than Actor.Act does that meshes well with it");
        }

        /// <summary>Moves the actor towards posX in the local x axis and posZ in the local z axis.</summary>
        protected void Move(float posX, float posZ)
        {
            throw new NotSupportedException("TODO: Implement Actor.Move better than Actor.Act does that meshes well with it");
        }

        protected Actor FindClosestEnemy()
        {
            if (enemyTeam.members.Length == 0) return null;
            Actor closest = enemyTeam.members[0];
            float closestDist = Vector3.Distance(closest.transform.position, this.transform.position);
            foreach(Actor enemy in enemyTeam.members)
            {
                float dist = Vector3.Distance(enemy.transform.position, this.transform.position);
                if (dist < closestDist)
                {
                    closest = enemy;
                    closestDist = dist;
                }
            }
            return closest;
        }

        public static float Distance(Actor a, Actor b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        // Note, the extra script space here is in case of extra necessary massaging, not to do what Actor.Move* do.
        protected void UpdateDominantHand()
        {

            _UpdateDominantHand();
        }

        /// <summary>
        /// Used to update the desired orientation and position of the dominant hand
        /// </summary>
        protected abstract void _UpdateDominantHand();

        
        protected void UpdateNonDominantHand()
        {

            _UpdateNonDominantHand();
        }

        /// <summary>
        /// Used to update the desired orientation and position of the nondominant hand
        /// </summary>
        protected abstract void _UpdateNonDominantHand();

        
    }
}