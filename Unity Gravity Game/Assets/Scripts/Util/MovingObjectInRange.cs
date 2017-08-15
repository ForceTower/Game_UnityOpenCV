using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectInRange : MonoBehaviour {
    public Vector3 moveDirectionVector;
    private Vector3 moveDirectionVectorAbsolute;
    private Vector3 rangeBasedOnMove;
    private Vector3 maxRangeAllowed;
    private Vector3 minRangeAllowed;
    public float range;
    public float speed;
    private Vector3 startPosition;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        moveDirectionVectorAbsolute = new Vector3 (Mathf.Abs (moveDirectionVector.x), Mathf.Abs (moveDirectionVector.y), Mathf.Abs (moveDirectionVector.z));
        rangeBasedOnMove = moveDirectionVectorAbsolute * range;
        //rangeBasedOnMove = new Vector3(moveDirectionVectorAbsolute.x * range, moveDirectionVectorAbsolute.y * range, moveDirectionVectorAbsolute.z * range);
        maxRangeAllowed = startPosition + rangeBasedOnMove;
        minRangeAllowed = startPosition - rangeBasedOnMove;
    }

    // Update is called once per frame
    void Update () {
        float val = speed * Time.deltaTime;
        Vector3 v = new Vector3 (moveDirectionVector.x * val, moveDirectionVector.y * val, moveDirectionVector.z * val);
        transform.Translate (v);
    }

    void LateUpdate () {
        Vector3 currentPos = transform.position;

        // float s;
        if (!(currentPos.x <= maxRangeAllowed.x && currentPos.x >= minRangeAllowed.x))
            //s = 1;
            Destroy (gameObject);
        else if (!(currentPos.y <= maxRangeAllowed.y && currentPos.y >= minRangeAllowed.y))
            //s = 2;
            Destroy (gameObject);
        else if (!(currentPos.z <= maxRangeAllowed.z && currentPos.z >= minRangeAllowed.z))
            //s = 3;
            Destroy (gameObject);
    }
}