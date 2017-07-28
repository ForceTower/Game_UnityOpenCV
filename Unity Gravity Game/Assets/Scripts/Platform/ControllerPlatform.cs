using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlatform : MonoBehaviour {
    public Move scriptMove;
    public ChangeColor scriptColor;
    private bool endMove, endColor;

    //to debug
    public Color c;
    public Vector3 v;
    public float t;
    public bool b;


	// Use this for initialization
	void Start () {
        if (!scriptMove)
            scriptMove = GetComponent<Move>();
        if (!scriptColor)
            scriptColor = GetComponent<ChangeColor>();
	}
	
	// Update is called once per frame
	void Update () {
        if (b) {
            go(v, c, t);
            b = !b;
        }
        if (scriptMove.finished && scriptColor.finished)
            finished();
	}

    public void go(Vector3 target, Color color, float factor) {
        scriptMove.start(target, factor);
        scriptColor.start(color, factor);
    }

    private void finished() {
    }
}
