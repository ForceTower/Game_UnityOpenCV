using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {
    public static GravityController Instance { get; set; }

    public enum GravityState { Up, Down, Left, Right, Push, Pull, None };
    public GravityState m_CurrentState = GravityState.Down;

    private void Start () {
        ChangeGravity (m_CurrentState);
    }

    void Awake () {
        Instance = this;
    }

    void Update () {
#if UNITY_EDITOR
        Debug.Log ("Gravity: " + Physics.gravity);
#endif
    }

    public void ChangeGravity (GravityState newState) {
        m_CurrentState = newState;
        switch (m_CurrentState) {
            case GravityState.Down:
                Physics.gravity = new Vector3 (0, -9.8f, 0);
                break;
            case GravityState.Up:
                Physics.gravity = new Vector3 (0, 9.8f, 0);
                break;
            case GravityState.Left:
                Physics.gravity = new Vector3 (-9.8f, 0, 0);
                break;
            case GravityState.Right:
                Physics.gravity = new Vector3 (9.8f, 0, 0);
                break;
            case GravityState.Pull:
                Physics.gravity = new Vector3 (0, 0, -9.8f);
                break;
            case GravityState.Push:
                Physics.gravity = new Vector3 (0, 0, 9.8f);
                break;
        }
        Camera.main.GetComponent<CameraSide> ().ChangeLook ();
    }

    public Vector3 GetJumpVector (float jumpPower, Vector3 velocity) {
        Vector3 gravity = Physics.gravity;

        Vector3 jumpVector = new Vector3 (
            gravity.x != 0 ? gravity.x < 0 ? jumpPower : -jumpPower : velocity.x,
            gravity.y != 0 ? gravity.y < 0 ? jumpPower : -jumpPower : velocity.y,
            gravity.z != 0 ? gravity.z < 0 ? jumpPower : -jumpPower : velocity.z);

        return jumpVector;
    }
}
