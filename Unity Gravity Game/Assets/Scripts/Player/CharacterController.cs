using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Rigidbody))]
public class CharacterController : MonoBehaviour {
    private Rigidbody m_RigidBody;
    public float m_JumpPower;
    public float m_Speed;

    public bool m_IsGrounded;
    public float m_GroundCheckDistance = 0.1f;
    public Vector3 m_GroundNormal;

    public bool m_DoubleJump;
    public bool m_DJOffset;

    private Color m_CurrentColor;

    // Use this for initialization
    void Start () {
        m_DoubleJump = false;
        m_DJOffset = true;
        m_RigidBody = GetComponent<Rigidbody> ();
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
    }

    // Update is called once per frame
    void Update () {
        ChangeColor ();
    }

    public void Move (Vector3 moveVector, bool jump) {
        CheckGround ();

        if (m_IsGrounded)
            GroundedCommands (moveVector, jump);
        else
            OnAirCommands (jump);
    }

    private void GroundedCommands (Vector3 moveVector, bool jump) {
        moveVector = ModifiedMoveVector (moveVector);
        m_RigidBody.velocity = moveVector;

        if (jump) {
            Jump (GravityController.Instance.GetJumpVector (m_JumpPower, m_RigidBody.velocity));
            m_DJOffset = true;
        }
    }

    private void OnAirCommands (bool jump) {
        if (m_DoubleJump && jump) {
            Jump (GravityController.Instance.GetJumpVector (m_JumpPower, m_RigidBody.velocity));
            m_DoubleJump = false;
        }
    }

    private Vector3 ModifiedMoveVector (Vector3 vector) {
        CameraSide.CameraState state = Camera.main.GetComponent<CameraSide> ().state;
        CameraSide.CameraOption option = Camera.main.GetComponent<CameraSide> ().option;
        GravityController.GravityState gravity = GravityController.Instance.m_CurrentState;
        if (gravity == GravityController.GravityState.Down || gravity == GravityController.GravityState.Up) {
            switch (state) {
                case CameraSide.CameraState.Normal:
                    vector.y = 0;
                    vector.z = 0;
                    break;
                case CameraSide.CameraState.Sideway:
                    vector.z = vector.x;
                    vector.y = 0;
                    vector.x = 0;
                    break;
                case CameraSide.CameraState.Upside:
                    vector.z = vector.y;
                    vector.y = 0;
                    break;
            }
        }

        else if (gravity == GravityController.GravityState.Left || gravity == GravityController.GravityState.Right) {
            switch (state) {
                case CameraSide.CameraState.Normal:
                    vector.x = 0;
                    vector.z = 0;
                    break;
                case CameraSide.CameraState.Sideway:
                    vector.z = vector.x;
                    vector.x = 0;
                    break;
                case CameraSide.CameraState.Upside:
                    vector.z = vector.y;
                    vector.x = 0;
                    vector.y = 0;
                    break;
            }
        }

        return vector;
    }

    void Jump (Vector3 jumpPower) {
        m_IsGrounded = false;
        m_RigidBody.velocity = jumpPower;

        if (!m_DoubleJump) m_DoubleJump = true;
    }

    public void Stop () {
        m_RigidBody.velocity = Vector3.zero;
    }

    
    void CheckGround () {
        RaycastHit hitInfo;

        Vector3 down = GravityController.Instance.GetJumpVector (-1, Vector3.zero);
        Vector3 up = down * -1;

        if (Physics.Raycast (transform.position + (up * 0.1f), down, out hitInfo, m_GroundCheckDistance)) {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            m_DoubleJump = false;
            m_DJOffset = false;
        }
        else {
            if (!m_DJOffset) m_DoubleJump = true;
            m_DJOffset = true;
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
        }
    }

    /*
    public void OnCollisionEnter (Collision collision) {
        m_IsGrounded = true;
        m_IsGrounded = true;
        m_DoubleJump = false;
        m_DJOffset = false;
    }

    public void OnCollisionExit (Collision collision) {
        m_IsGrounded = false;
        if (!m_DJOffset) m_DoubleJump = true;
        m_DJOffset = true;
        m_IsGrounded = false;
        m_GroundNormal = Vector3.up;
    }
    */

    public void ChangeColor () {
        if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Down)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityDownColor;
        else if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Up)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityUpColor;
        else if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Left)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityLeftColor;
        else if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Right)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityRightColor;
        else if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Push)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityPushColor;
        else if (GravityController.Instance.m_CurrentState == GravityController.GravityState.Pull)
            GetComponent<MeshRenderer> ().material.color = GameConstants.Instance.gravityPullColor;

        m_CurrentColor = GetComponent<MeshRenderer> ().material.color;
    }
}
