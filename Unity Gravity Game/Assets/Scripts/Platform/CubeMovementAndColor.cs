using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovementAndColor : MonoBehaviour {
    private float deltaTime;
    private bool ready;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Color startColor;
    private Color endColor;
    private float expectedTime;

    // Use this for initialization
    void Start () {
        deltaTime = 0;
        ready = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (ready)
            ChangeColorAndPosition ();
	}

    public void ChangeToColorAndPosition(Vector3 position, Color color, float time) {
        ready = true;
        deltaTime = 0;
        startPosition = transform.position;
        startColor = GetComponent<MeshRenderer> ().material.color;

        endPosition = position;
        endColor = color;
        expectedTime = time;
    }

    private void ChangeColorAndPosition () {
        if (deltaTime >= 1) {
            ready = false;
            OnFinish ();
            deltaTime = 0;
        } else {
            deltaTime += Time.deltaTime / expectedTime;
            transform.position = Vector3.Lerp (startPosition, endPosition, deltaTime);
            GetComponent<MeshRenderer> ().material.color = Color.Lerp (startColor, endColor, deltaTime);
        }
    }

    void OnFinish () {

    }
}
