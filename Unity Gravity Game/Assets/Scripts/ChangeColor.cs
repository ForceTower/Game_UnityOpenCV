using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {
    public Color endColor;
    public float timeToColor = 1.0F;
    public bool go;

    private Material myMaterial;
    private Color startColor;
    private float actualTime;
    private float endTime;
    private bool startGo;
    private bool end;

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

    public void colorize(uint r, uint g, uint b) {    
        endColor.r = r;
        endColor.g = g;
        endColor.b = b;
        colorize();
    }

    public void colorize() {
        if (go) {
            startColor = myMaterial.color;
            go = false;
            startGo = true;
            end = false;
        }

        if (!end)
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
        end = true;
        startGo = false;
        return end;
    }
}
