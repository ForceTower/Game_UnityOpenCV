using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public static Vector2 CameraResolution;

    public CubeMovementAndColor m_DefaultPlatform;
    private CubeMovementAndColor[] _DefaultPlatformInstances; 

    private bool _Ready = false;
    private int _CurrentState = 0;

    private int _BlackPlatforms;
    private int _RedPlatforms;
    private int _YellowPlatforms;
    private int _GreenPlatforms;
    private int _BluePlatforms;

    private LevelDetectionPipeline.LevelElement[] _BlackPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _RedPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _YellowPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _GreenPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _BluePlatformsElements;

    public bool _ShouldRestartPipeline;
    private bool _WaitingState;
    public float _WaitingTime;
    public float _TimeToWait;

    private int _IthPlatform;

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

        _DefaultPlatformInstances = new CubeMovementAndColor[3600];
        GenerateDefaultPlatformsInstances();
    }

    void GenerateDefaultPlatformsInstances () {
        for (uint i = 0; i < _DefaultPlatformInstances.Length; i++) {
            _DefaultPlatformInstances[i] = Instantiate<CubeMovementAndColor> (m_DefaultPlatform, new Vector3 (0, 0, 0), Quaternion.identity);
        }
    }
	
	// Update is called once per frame
    void Update () {
        if (!_Ready)
            return;
        int stageIn = -2;

        if (_CurrentState < 9) {
            LevelDetectionPipeline.DetectionPipeline (ref stageIn, ref _CurrentState);
        }
        else {
            LevelCreation ();
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
            SetupGravityPlatforms ();
        } else if (_CurrentState == 13) {
            GetGravityPlatforms ();
        } else if (_CurrentState == 14) {
            MountGravityPlatforms ();
        } else if (_CurrentState == 18) {
            SetupWaitingState ();
        } else if (_CurrentState == 19) {
            WaitingState ();
        } else if (_CurrentState == 20) {
            SetStage (1);
        }
    }

    void SetupDefaultPlatforms () {
        LevelDetectionPipeline.SetupBlackPlatforms(ref _BlackPlatforms);
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
            _DefaultPlatformInstances[i].ChangeToColorAndPosition (position, Color.black, 2);
        }

        _IthPlatform = i;

        for (int j = i; j < _DefaultPlatformInstances.Length; j++) {
            _DefaultPlatformInstances[j].gameObject.SetActive (false);
        }

        _CurrentState = 12;
    }

    void SetupGravityPlatforms () {
        LevelDetectionPipeline.SetupRedPlatforms (ref _RedPlatforms);
        LevelDetectionPipeline.SetupYellowPlatforms (ref _YellowPlatforms);
        LevelDetectionPipeline.SetupGreenPlatforms (ref _GreenPlatforms);
        LevelDetectionPipeline.SetupBluePlatforms (ref _BluePlatforms);

        _CurrentState = 13;
    }

    void GetGravityPlatforms () {

        _RedPlatformsElements = new LevelDetectionPipeline.LevelElement[_RedPlatforms];
        _YellowPlatformsElements = new LevelDetectionPipeline.LevelElement[_YellowPlatforms];
        _GreenPlatformsElements = new LevelDetectionPipeline.LevelElement[_GreenPlatforms];
        _BluePlatformsElements = new LevelDetectionPipeline.LevelElement[_BluePlatforms];

        unsafe
        {
            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _RedPlatformsElements) {
                LevelDetectionPipeline.GetRedPlatforms (arrayAddress, _RedPlatforms, ref _RedPlatforms);
            }

            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _YellowPlatformsElements) {
                LevelDetectionPipeline.GetYellowPlatforms (arrayAddress, _YellowPlatforms, ref _YellowPlatforms);
            }

            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _GreenPlatformsElements) {
                LevelDetectionPipeline.GetGreenPlatforms (arrayAddress, _GreenPlatforms, ref _GreenPlatforms);
            }

            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _BluePlatformsElements) {
                LevelDetectionPipeline.GetBluePlatforms (arrayAddress, _BluePlatforms, ref _BluePlatforms);
            }
        }

        _CurrentState = 14;
    }

    void MountGravityPlatforms() {
        int current;
        for (current = 0; current < _RedPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (-1 * _RedPlatformsElements[current].Y, -1 * _RedPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.red, 2);
        }

        _IthPlatform = current;
        for (current = 0; current < _YellowPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (-1 * _YellowPlatformsElements[current].Y, -1 * _YellowPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.yellow, 2);
        }

        _IthPlatform = current;
        for (current = 0; current < _GreenPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (-1 * _GreenPlatformsElements[current].Y, -1 * _GreenPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.green, 2);
        }

        _IthPlatform = current;
        for (current = 0; current < _BluePlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (-1 * _BluePlatformsElements[current].Y, -1 * _BluePlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.blue, 2);
        }

        _CurrentState = 18;
    }

    private void SetupWaitingState () {
        Debug.Log ("Detected " + (_BlackPlatforms + _RedPlatforms + _YellowPlatforms + _GreenPlatforms + _BluePlatforms) + " Platforms");
        if (_ShouldRestartPipeline) {
            _WaitingTime = 0;
            _CurrentState = 19;
        }
        else {
            LevelDetectionPipeline.Close ();
            gameObject.SetActive (false);
        }
    }

    private void WaitingState () {
        _WaitingTime += Time.deltaTime / _TimeToWait;

        if (_WaitingTime >= 1) {
            _WaitingTime = 0;
            _CurrentState = 20;
        }
    }

    void SetStage (int value) {
        LevelDetectionPipeline.SetState(value);
        _CurrentState = value;
    }
}
