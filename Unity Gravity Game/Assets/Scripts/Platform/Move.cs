using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public Vector3 endMarker;
    public float speed = 1.0F;
    public float margin;
    public bool go;
    public bool finished;

    private Vector3 startMarker;
    private float startTime;
    private float journeyLength;
    private bool startGo;

    void Start() {

    }

    void Update() {
         if (go || startGo)
            MoveComponent();
    }

    public void StartMove(Vector3 target, float speed) {
        endMarker = target;
        this.speed = speed;
        go = true;
    }

    public void MoveComponent() {
        if (go) {
            startMarker = transform.position;
            startTime = Time.time;
            journeyLength = Vector3.Distance(startMarker, endMarker);
            go = !go;
            startGo = true;
            finished = false;
        }
        if(!finished)
            MoveComponent(endMarker);
    }

    public void MoveComponent(Vector3 endMarker) {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
        GetComponent<ChangeColor> ().timeToColor = fracJourney;
        // verifying arrived at destiny
        float actualDistance = Vector3.Distance(transform.position, endMarker);

        if (actualDistance < margin)
            ArrivedDestiny();
    }

    public bool ArrivedDestiny() {
        finished = true;
        startGo = false;
        return finished;
    }

    public void MoveComponent(float x, float y, float z, float speed) {
        this.speed = speed;
        endMarker.x = x;
        endMarker.y = y;
        endMarker.z = z;
        MoveComponent();
    }
}
