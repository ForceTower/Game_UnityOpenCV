using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance { get; set; }
    public float dampTime = 0.15f;
    public float cameraDistance = 7.5f;
    public float cameraDistanceUp = 7.5f;

    private Vector3 velocity = Vector3.zero;

    public Transform target;

    private bool lookingAndMoving = false;
    private Transform lookAndMoveTarget = null;

    // Use this for initialization
    void Awake () {
        Instance = this;
    }

    private void FixedUpdate () {
        if (target) {
            Vector3 t_Position = target.position;
            Vector3 delta = t_Position - GetComponent<Camera> ().ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, cameraDistanceUp));
            Vector3 dest = transform.position + delta;

            transform.position = Vector3.SmoothDamp (transform.position, dest, ref velocity, dampTime);

            if (lookingAndMoving)
                transform.LookAt (lookAndMoveTarget);
        }
    }

    public void SetTarget (Transform target) {
        this.target = target;
    }
}
