using UnityEngine;
using System.Collections;

public class OffsetHand : MonoBehaviour {

    public Animator anim;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnPreCull()
    {
        Debug.Log("running");
        anim.GetBoneTransform(HumanBodyBones.RightHand).Rotate(new Vector3(-270f, 0, 270));
    }
}
