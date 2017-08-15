using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour {
    public static GameConstants Instance { get; set; }

    //Gravity Colors
    public Color gravityDownColor;
    public Color gravityUpColor;
    public Color gravityRightColor;
    public Color gravityLeftColor;
    public Color gravityPushColor;
    public Color gravityPullColor;

    //Camera Colors/Materials
    public Color normalCameraColor;
    public Color sidewayCameraColor;
    public Color upsideCameraColor;

    public Color startTeleportGate;
    public Color endTeleportGate;

    public Mesh sphereMesh;

    void Awake () {
        Instance = this;
    }
}
