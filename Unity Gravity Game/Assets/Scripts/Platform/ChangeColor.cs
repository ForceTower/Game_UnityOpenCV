using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {
    public Color endColor;
    public float timeToColor = 1.0F;
    public bool go;
    public bool finished;

    private Material myMaterial;
    private Color startColor;
    private float actualTime;
    private float endTime;
    private bool startGo;

    private MeshRenderer myMesh;

    // Use this for initialization
    void Start () {
        myMesh = GetComponent<MeshRenderer>();
        if(myMaterial == null)
            myMaterial = myMesh.materials [0];
    }

    void Update() {
        if (go || startGo)
            colorize();
    }

    public void start(Color color, float time) {
        timeToColor = time;
        endColor = color;
        go = true;
    }

    public void colorize() {
        if (go) {
            startColor = myMaterial.color;
            go = false;
            startGo = true;
            finished = false;
        }

        if (!finished)
            colorize(endColor);
    }

    public void colorize(Color endColor) {
        actualTime += Time.deltaTime;
        float fracTime = actualTime / timeToColor;
        myMaterial.color = Color.Lerp(startColor, endColor, fracTime);

        if (actualTime >= timeToColor)
            arrivedColor();
    }

    public bool arrivedColor() {
        actualTime = 0;
        finished = true;
        startGo = false;
        return finished;
    }

    public void colorize(uint r, uint g, uint b, float time) {
        timeToColor = time;
        endColor.r = r;
        endColor.g = g;
        endColor.b = b;
        colorize();
    }
}
