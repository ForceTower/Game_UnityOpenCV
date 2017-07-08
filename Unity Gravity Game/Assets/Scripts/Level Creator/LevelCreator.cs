using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public static Vector2 CameraResolution;

    private bool _Ready = false;
    private int _CurrentState = 0;
    private int _BlackPlatforms;

    // Use this for initialization
    void Start () {
        int cameraWidth = 0;
        int cameraHeight = 0;

        int result = LevelDetectionPipeline.Init(ref cameraWidth, ref cameraHeight);

        if (result < 0) {
            Debug.LogWarning ("Failed to Open Camera");
            _CurrentState = -1;
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

        if (_CurrentState < 9) {
            LevelDetectionPipeline.DetectionPipeline(ref stageIn, ref _CurrentState);
        } else {
            LevelCreation();
        }

        Debug.Log("Stage [In: " + stageIn + "] .. [Out: " + _CurrentState + "]");
    }

    void OnApplicationQuit () {
        if (_Ready)
            LevelDetectionPipeline.Close();
    }

    void LevelCreation () {
        if (_CurrentState == 9) {
            LevelDetectionPipeline.SetupBlackPlatforms(ref _BlackPlatforms);
            Debug.Log("Detected " + _BlackPlatforms + " Platforms");
            SetStage(1);
        }
    }

    void SetStage(int value) {
        LevelDetectionPipeline.SetState(value);
        _CurrentState = value;
    }
}
