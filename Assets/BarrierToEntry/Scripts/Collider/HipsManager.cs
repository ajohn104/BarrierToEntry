using UnityEngine;
using System.Collections;

namespace BarrierToEntry {
    /// <summary>
    /// Used to manage the dismemberment of the legs. If either is hit, both are "removed"
    /// </summary>
    public class HipsManager : MonoBehaviour {
        public Actor owner;
        bool LegsRemoved = false;
        

        public void OnTriggerEnter(Collider col)
        {
            if (LegsRemoved) { return; }

            GameObject other = col.gameObject;
            Saber otherWeapon = other.GetComponentInParent<Saber>();

            if (!RagdollSpawner.DebugEnabled)
            {
                if (otherWeapon == null) { return; }
                if (Team.isSameTeam(owner, otherWeapon.owner)) { return; }
            } else
            {
                if (otherWeapon != null) { return; }
            }

            LegsRemoved = true;

            if (RagdollSpawner.SpawnLimbRagdolls)
            {
                Transform leftLeg = RagdollSpawner.SpawnRagdoll(owner, HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg).transform;
                Transform rightLeg = RagdollSpawner.SpawnRagdoll(owner, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg).transform;

                leftLeg.position = owner.transform.position;

                leftLeg.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperLeg).position = owner.anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
                PushToPosition(leftLeg.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).position, leftLeg, "LeftUpLeg");
                rightLeg.position = owner.transform.position;

                rightLeg.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightUpperLeg).position = owner.anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
                PushToPosition(rightLeg.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).position, rightLeg, "RightUpLeg");
            }

            transform.localScale = RagdollSpawner.ScaleDown * Vector3.one;
            owner.anim.GetBoneTransform(HumanBodyBones.Spine).localScale = RagdollSpawner.ScaleUp * Vector3.one;

            owner.GetComponent<BoxCollider>().center = owner.transform.InverseTransformPoint(owner.anim.GetBoneTransform(HumanBodyBones.Hips).transform.position);
        }


        // Currently not in use as limb ragdolling is experimental
        private void PushToPosition(Vector3 pos, Transform obj, string reject)
        {
            if (obj.name.Equals(reject)) return;

            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                if (child.name.Equals(reject)) continue;
                if(!child.name.Equals("Hips")) child.position = pos;
                
                Component[] components = child.GetComponents<Component>();
                
                for (int j = 0; j < components.Length; j++)
                {
                    if (!(components[j] is Transform) && !(components[j] is Rigidbody) && !(components[j] is SkinnedMeshRenderer))
                    {
                        if (!(child.name.Equals("Hips") && (components[j] is Rigidbody || components[j] is BoxCollider)))
                        {
                            Destroy(components[j]);
                        }
                    }
                }

                for (int j = 0; j < components.Length; j++)
                {

                    if (!(components[j] is Transform) && !(components[j] is SkinnedMeshRenderer))
                    {
                        if (!(child.name.Equals("Hips") && (components[j] is Rigidbody || components[j] is BoxCollider)))
                        {
                            Destroy(components[j]);
                        }
                        if ((child.name.Equals("Hips") && components[j] is BoxCollider))
                        {
                            ((BoxCollider)components[j]).isTrigger = true;
                        }
                    }
                }
                PushToPosition(pos, child, reject);
            }
        }
    }
}