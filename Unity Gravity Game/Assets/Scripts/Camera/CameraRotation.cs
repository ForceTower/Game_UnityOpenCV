using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    private enum CameraRotate { START, ROTATING, FINISHED }

    private CameraRotate rotate;
    public float rotateTime = 2;
    private Quaternion s_quat;
    private Quaternion d_quat;
    private float dRotate;

    // Use this for initialization
    void Start () {
        rotate = CameraRotate.FINISHED;
    }

    // Update is called once per frame
    void FixedUpdate () {
        ComputeRotate ();
    }

    public void RotateTo (Quaternion quat) {
        s_quat = transform.rotation;
        d_quat = quat;
        rotate = CameraRotate.ROTATING;
        dRotate = 0f;
    }

    private void ComputeRotate () {
        if (rotate != CameraRotate.FINISHED) {
            dRotate += Time.deltaTime / rotateTime;

            if (dRotate >= 1) {
                dRotate = 1;
                rotate = CameraRotate.FINISHED;
            }

            Quaternion c_quat = Quaternion.Lerp (s_quat, d_quat, dRotate);
            transform.rotation = Quaternion.Slerp (transform.rotation, c_quat, 1);
        }
    }
}
