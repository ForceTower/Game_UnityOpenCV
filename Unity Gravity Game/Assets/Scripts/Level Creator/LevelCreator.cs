using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour {
    public static Vector2 CameraResolution;

    public CubeMovementAndColor m_DefaultPlatform;
    public StationaryEnemy m_DefaultEnemy;
    public LevelStart m_StartTransform;
    public LevelFinish m_FinishTransform;
    public Transform m_Player;

    public int m_MaxEnemiesInLevel;

    private CubeMovementAndColor[] _DefaultPlatformInstances;
    private StationaryEnemy[] _DefaultEnemyInstances;

    private bool _Ready = false;
    private int _CurrentState = 0;

    private bool _GameStarted = false;

    private int _BlackPlatforms;
    private int _RedPlatforms;
    private int _YellowPlatforms;
    private int _GreenPlatforms;
    private int _BluePlatforms;
    private int _BlueEnemies;

    private LevelDetectionPipeline.LevelElement[] _BlackPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _RedPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _YellowPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _GreenPlatformsElements;
    private LevelDetectionPipeline.LevelElement[] _BluePlatformsElements;

    private LevelDetectionPipeline.LevelElement[] _BlueEnemiesElements;

    public bool _ShouldRestartPipeline;
    private bool _WaitingState;
    public float _WaitingTime;
    public float _TimeToWait;

    private int _IthPlatform;

    private bool m_StartOnNext;
    private Vector3 m_StartPos;

    // Use this for initialization
    void Start () {
        int cameraWidth = 0;
        int cameraHeight = 0;

        int result = LevelDetectionPipeline.Init(ref cameraWidth, ref cameraHeight, 0);

        if (result < 0) {
            Debug.LogWarning ("Failed to Open Camera");
            _CurrentState = -1;
            return;
        }

        CameraResolution = new Vector2(cameraWidth, cameraHeight);
        _Ready = true;

        _DefaultPlatformInstances = new CubeMovementAndColor[14400];
        _DefaultEnemyInstances = new StationaryEnemy[m_MaxEnemiesInLevel];

        GenerateDefaultPlatformsInstances ();
        GenerateDefaultEnemies ();
    }

    private void GenerateDefaultPlatformsInstances () {
        for (uint i = 0; i < _DefaultPlatformInstances.Length; i++) {
            _DefaultPlatformInstances[i] = Instantiate<CubeMovementAndColor> (m_DefaultPlatform, new Vector3 (0, 0, 0), Quaternion.identity);
            _DefaultPlatformInstances[i].gameObject.SetActive (false);
        }
    }

    private void GenerateDefaultEnemies () {
        for (uint i = 0; i < m_MaxEnemiesInLevel; i++) {
            _DefaultEnemyInstances[i] = Instantiate<StationaryEnemy> (m_DefaultEnemy, new Vector3 (0, 0, 0), Quaternion.identity);
            _DefaultEnemyInstances[i].gameObject.SetActive (false);
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

        if (Input.GetKeyDown (KeyCode.F)) {
            m_StartOnNext = true;
            m_StartPos = new Vector3 (0, 0, 0);
        }
    }

    void OnApplicationQuit () {
        if (_Ready)
            LevelDetectionPipeline.Close();
    }

    private void LevelCreation () {
        if (_CurrentState == 9) {
            SetupDefaultPlatforms ();
        }
        else if (_CurrentState == 10) {
            GetDefaultPlatforms ();
        }
        else if (_CurrentState == 11) {
            MountDefaultPlatforms ();
        }
        else if (_CurrentState == 12) {
            SetupGravityPlatforms ();
        }
        else if (_CurrentState == 13) {
            GetGravityPlatforms ();
        }
        else if (_CurrentState == 14) {
            MountGravityPlatforms ();
        }
        else if (_CurrentState == 15) {
            GetBlueEnemies ();
        }
        else if (_CurrentState == 16) {
            MountBlueEnemies ();
        }
        else if (_CurrentState == 17) {
            if (!_GameStarted)
                SetupStartEnd ();
        }
        else if (_CurrentState == 18) {
            SetupWaitingState ();
        }
        else if (_CurrentState == 19) {
            WaitingState ();
        }
        else if (_CurrentState == 20) {
            SetStage (1);
        }
    }

    private void SetupDefaultPlatforms () {
        LevelDetectionPipeline.SetupBlackPlatforms(ref _BlackPlatforms);
        _CurrentState = 10;
    }

    private void GetDefaultPlatforms () {
        _BlackPlatformsElements = new LevelDetectionPipeline.LevelElement[_BlackPlatforms];

        unsafe {
            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _BlackPlatformsElements) {
                LevelDetectionPipeline.GetBlackPlatforms(arrayAddress, _BlackPlatforms, ref _BlackPlatforms);
            }
        }

        _CurrentState = 11;
    }

    private void MountDefaultPlatforms () {
        int i;
        for (i = 0; i < _BlackPlatformsElements.Length; i++) {
            Vector3 position = new Vector3 (_BlackPlatformsElements[i].Y, _BlackPlatformsElements[i].X*-1, 0);
            _DefaultPlatformInstances[i].gameObject.SetActive (true);
            _DefaultPlatformInstances[i].ChangeToColorAndPosition (position, Color.black, 2);
        }

        _IthPlatform = i;

        _CurrentState = 12;
    }

    private void SetupGravityPlatforms () {
        LevelDetectionPipeline.SetupRedPlatforms (ref _RedPlatforms);
        LevelDetectionPipeline.SetupYellowPlatforms (ref _YellowPlatforms);
        LevelDetectionPipeline.SetupGreenPlatforms (ref _GreenPlatforms);
        LevelDetectionPipeline.SetupBluePlatforms (ref _BluePlatforms);

        Debug.Log ("Red: " + _RedPlatforms);
        Debug.Log ("Yellow: " + _YellowPlatforms);
        Debug.Log ("Green: " + _GreenPlatforms);
        Debug.Log ("Blue: " + _BluePlatforms);


        _CurrentState = 13;
    }

    private void GetGravityPlatforms () {
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

    private void MountGravityPlatforms() {
       int current;
       for (current = 0; current < _RedPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (_RedPlatformsElements[current].Y, -1 * _RedPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.red, 2);
        }
        
        _IthPlatform += current;
        for (current = 0; current < _YellowPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (_YellowPlatformsElements[current].Y, -1 * _YellowPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.yellow, 2);
        }
        
        _IthPlatform += current;
        for (current = 0; current < _GreenPlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (_GreenPlatformsElements[current].Y, -1 * _GreenPlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.green, 2);
        }

        _IthPlatform += current;
        for (current = 0; current < _BluePlatformsElements.Length; current++) {
            Vector3 position = new Vector3 (_BluePlatformsElements[current].Y, -1 * _BluePlatformsElements[current].X, 0);
            _DefaultPlatformInstances[current + _IthPlatform].gameObject.SetActive (true);
            _DefaultPlatformInstances[current + _IthPlatform].ChangeToColorAndPosition (position, Color.blue, 2);
        }

        _IthPlatform += current;
        for (int j = _IthPlatform; j < _DefaultPlatformInstances.Length; j++) {
            _DefaultPlatformInstances[j].gameObject.SetActive (false);
        }

        _CurrentState = 15;
    }

    private void GetBlueEnemies () {
        LevelDetectionPipeline.NumberOfBlueEnemies (ref _BlueEnemies);
        _BlueEnemiesElements = new LevelDetectionPipeline.LevelElement[_BlueEnemies];
        unsafe {
            fixed (LevelDetectionPipeline.LevelElement* arrayAddress = _BlueEnemiesElements) {
                LevelDetectionPipeline.GetBlueEnemies (arrayAddress, m_MaxEnemiesInLevel, ref _BlueEnemies);
            }
        }

        _CurrentState = 16;
    }

    private void MountBlueEnemies () {
        int i;
        for (i = 0; i < m_MaxEnemiesInLevel && i < _BlueEnemies; i++) {
            Vector3 position = new Vector3 (_BlueEnemiesElements[i].X, _BlueEnemiesElements[i].Y *-1, 0);
            _DefaultEnemyInstances[i].gameObject.SetActive (true);
            _DefaultEnemyInstances[i].transform.position = position;
        }

        for (; i < _DefaultEnemyInstances.Length; i++) {
            _DefaultEnemyInstances[i].gameObject.SetActive (false);
        }

        _CurrentState = 17;
    }

    private void SetupStartEnd () {
        LevelDetectionPipeline.LevelElement[] startEnd = new LevelDetectionPipeline.LevelElement[2];
        bool ready = false;

        unsafe {
            fixed (LevelDetectionPipeline.LevelElement* startEndPointer = startEnd) {
                ready = LevelDetectionPipeline.GetPlayerStartEnd (startEndPointer);
            }
        }

       
        if (ready) {
            Debug.Log ("Game is Ready");
            Vector3 startPos = new Vector3 (startEnd[0].X, startEnd[0].Y * -1, 0);
            Instantiate (m_StartTransform, startPos, Quaternion.identity);
            Vector3 endPos = new Vector3 (startEnd[1].X, startEnd[1].Y *-1, 0);
            Instantiate (m_FinishTransform, endPos, Quaternion.identity);

            if (m_StartOnNext && !_GameStarted) {
                m_StartPos.z = 0;
                Transform player = Instantiate (m_Player, m_StartPos, Quaternion.identity);
                _GameStarted = true;
                CameraFollow.Instance.SetTarget (player);
            }
            m_StartOnNext = true;
            m_StartPos = startPos;
        } else {
            Debug.Log ("Game is not ready");
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

            if (!_ShouldRestartPipeline) {
                LevelDetectionPipeline.Close ();
                gameObject.SetActive (false);
            }

        }
    }

    void SetStage (int value) {
        LevelDetectionPipeline.SetState(value);
        _CurrentState = value;
    }
}
