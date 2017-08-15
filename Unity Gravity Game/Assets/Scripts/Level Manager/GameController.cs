using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController Instance { get; set; }

    private void Awake () {
        Instance = this;
    }

    public void ResetPlayer () {
        PlayerController.Instance.Reset ();
        GravityController.Instance.ChangeGravity (GravityController.GravityState.Down);
        Camera.main.GetComponent<CameraSide> ().ChangeState (CameraSide.CameraState.Normal);
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.R))
            ResetPlayer ();
    }
}
