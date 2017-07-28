using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public static Vector2 CameraResolution;

    public ControllerPlatform m_DefaultPlatform;

    private ControllerPlatform[] _DefaultPlatformInstances; 

    private bool _Ready = false;
    private int _CurrentState = 0;
    private int _BlackPlatforms;
    private LevelDetectionPipeline.LevelElement[] _BlackPlatformsElements;

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

        _DefaultPlatformInstances = new ControllerPlatform[3600];
        GenerateDefaultPlatformsInstances();
    }

    void GenerateDefaultPlatformsInstances () {
        for (uint i = 0; i < _DefaultPlatformInstances.Length; i++) {
            _DefaultPlatformInstances[i] = Instantiate<ControllerPlatform> (m_DefaultPlatform, new Vector3 (0, 0, 0), Quaternion.identity);
        }
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
    }

    void OnApplicationQuit () {
        if (_Ready)
            LevelDetectionPipeline.Close();
    }

    void LevelCreation () {
        if (_CurrentState == 9) {
            SetupDefaultPlatforms ();
        } else if (_CurrentState == 10) {
            GetDefaultPlatforms ();
        } else if (_CurrentState == 11) {
            MountDefaultPlatforms ();
        } else if (_CurrentState == 12) {
            SetStage (1);
        }
    }

    void SetupDefaultPlatforms () {
        LevelDetectionPipeline.SetupBlackPlatforms(ref _BlackPlatforms);
        Debug.Log("Detected " + _BlackPlatforms + " Platforms");
        _CurrentState = 10;
    }

    void GetDefaultPlatforms () {
        _BlackPlatformsElements = new LevelDetectionPipeline.LevelElement[_BlackPlatforms];

        unsafe {
            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _BlackPlatformsElements) {
                LevelDetectionPipeline.GetBlackPlatforms(arrayAddress, _BlackPlatforms, ref _BlackPlatforms);
            }
        }

        _CurrentState = 11;
    }

    void MountDefaultPlatforms () {
        int i;
        for (i = 0; i < _BlackPlatformsElements.Length; i++) {
            Vector3 position = new Vector3 (-1*_BlackPlatformsElements[i].Y, -1*_BlackPlatformsElements[i].X, 0);
            _DefaultPlatformInstances[i].gameObject.SetActive (true);
            _DefaultPlatformInstances[i].Go (position, Color.blue, 60);
        }

        //Debug.Log ("I: " + i + " Len: " + _BlackPlatformsElements.Length);

        for (int j = i; j < _DefaultPlatformInstances.Length; j++) {
            _DefaultPlatformInstances[j].gameObject.SetActive (false);
        }

        _CurrentState = 17;
    }

    void SetStage (int value) {
        LevelDetectionPipeline.SetState(value);
        _CurrentState = value;
    }
}
