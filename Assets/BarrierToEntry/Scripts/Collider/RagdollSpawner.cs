using UnityEngine;
using System.Collections;

namespace BarrierToEntry
{
    public class RagdollSpawner : MonoBehaviour
    {
        public GameObject ragdoll;
        public static GameObject rd;
        public static float ScaleDown = 0.001f;
        public static float ScaleUp = 1f / ScaleDown;
        public static bool SpawnLimbRagdolls = false;
        public static bool DebugEnabled = false;

        void Start()
        {
            rd = ragdoll;
        }

        public static GameObject SpawnRagdoll(Actor actor, HumanBodyBones originPart, HumanBodyBones keptPart)
        {
            GameObject obj = Object.Instantiate(RagdollSpawner.rd);
            Animator anim = obj.GetComponent<Animator>();
            SyncBones(actor.transform.Find("ScientistSkeleton"), obj.transform.Find("ScientistSkeleton"));
            anim.GetBoneTransform(originPart).localScale = ScaleDown * Vector3.one;
            anim.GetBoneTransform(keptPart).localScale = ScaleUp * Vector3.one;

            return obj;
        }

        public static void SyncBones(Transform source, Transform dest)
        {
            for(int i = 0; i < dest.childCount; i++)
            {
                Transform destChild = dest.GetChild(i);
                Transform sourceChild = source.GetChild(i);
                destChild.position = sourceChild.position;
                destChild.rotation = sourceChild.rotation;
                SyncBones(sourceChild, destChild);
            }
        }
        
    }
}