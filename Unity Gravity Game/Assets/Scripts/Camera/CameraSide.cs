using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CameraFollow))]
[RequireComponent (typeof (CameraRotation))]
public class CameraSide : MonoBehaviour {
    public enum CameraState { Normal, Sideway, Upside }
    public enum CameraOption { Normal, Inverted }

    private CameraFollow m_CameraFollow;
    private CameraRotation m_CameraRotation;

    public CameraState state;
    public CameraOption option;

    private void Awake () {
        m_CameraFollow = GetComponent<CameraFollow> ();
        m_CameraRotation = GetComponent<CameraRotation> ();
    }
    // Use this for initialization
    void Start () {
        ChangeLook ();
    }

    // Update is called once per frame
    void Update () {

    }

    public void ChangeState (CameraState nState) {
        state = nState;
        ChangeLook ();
    }

    public void ChangeOption (CameraOption nOption) {
        option = nOption;
        ChangeLook ();
    }

    public void ChangeLook () {
        Quaternion quat = Quaternion.Euler (20, 0, 0);
        m_CameraFollow.cameraDistanceUp = m_CameraFollow.cameraDistance * 1f;

        if (state == CameraState.Sideway) {
            quat = Quaternion.Euler (20, 270, 0);
            m_CameraFollow.cameraDistanceUp = m_CameraFollow.cameraDistance * 1.2f;
        }
        else if (state == CameraState.Upside) {
            quat = Quaternion.Euler (90, 0, 0);
            m_CameraFollow.cameraDistanceUp = m_CameraFollow.cameraDistance * 1.5f;
        }

        //m_CameraRotation.RotateTo (quat);

        ChangeSide ();
    }

    public void ChangeSide () {
        GravityController.GravityState grav = GravityController.Instance.m_CurrentState;

        Quaternion quat;

        if (option == CameraOption.Inverted) {
            if (state == CameraState.Normal) {
                if (grav == GravityController.GravityState.Up)
                    quat = Quaternion.Euler (-20, 180, 0);
                else
                    quat = Quaternion.Euler (20, 180, 0);
            }
            else if (state == CameraState.Sideway) {
                if (grav == GravityController.GravityState.Up || grav == GravityController.GravityState.Left)
                    quat = Quaternion.Euler (-20, 90, 0);
                else
                    quat = Quaternion.Euler (20, 90, 0);
            }
            else
                quat = Quaternion.Euler (270, 0, 0);
        }
        else {
            if (state == CameraState.Normal) {
                if (grav == GravityController.GravityState.Up)
                    quat = Quaternion.Euler (-20, 0, 0);
                else
                    quat = Quaternion.Euler (20, 0, 0);
            }
            else if (state == CameraState.Sideway) {
                if (grav == GravityController.GravityState.Up || grav == GravityController.GravityState.Left)
                    quat = Quaternion.Euler (-20, 270, 0);
                else
                    quat = Quaternion.Euler (20, 270, 0);
            }
            else
                quat = Quaternion.Euler (90, 0, 0);
        }

        m_CameraRotation.RotateTo (quat);
    }
}
