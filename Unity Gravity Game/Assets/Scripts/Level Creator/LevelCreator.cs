using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public static Vector2 CameraResolution;

    private bool _Ready = false;

	// Use this for initialization
	void Start () {
        int cameraWidth = 0;
        int cameraHeight = 0;

        int result = LevelDetectionPipeline.Init(ref cameraWidth, ref cameraHeight);

        if (result < 0) {
            Debug.LogWarning ("Failed to Open Camera");
            return;
        }

        CameraResolution = new Vector2(cameraWidth, cameraHeight);
        _Ready = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!_Ready)
            return;
        int stageIn = -2;
        int stageOut = -2;

        LevelDetectionPipeline.DetectionPipeline(ref stageIn, ref stageOut);

        Debug.Log("Stage [In: " + stageIn + "] .. [Out: " + stageOut + "]");
	}

    void OnApplicationQuit() {
        if (_Ready)
            LevelDetectionPipeline.Close();
    }
}
