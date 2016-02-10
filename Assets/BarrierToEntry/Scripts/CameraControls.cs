using UnityEngine;
using System.Collections;
using Coliseo;

public class CameraControls : MonoBehaviour {

    public Transform camTrans;

    // Mouse sensitivity
    public float horizontalSpeed = 4.0F;
    public float verticalSpeed = 4.0F;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");

        float rotationX = h*Time.deltaTime;
        float rotationY = v*Time.deltaTime;

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            camTrans.Rotate(-rotationY, rotationX, 0);
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            camTrans.Rotate(-rotationY/verticalSpeed*40f, rotationX / horizontalSpeed * 40f, 0);
        }
    }
}
