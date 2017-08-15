using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlatform : MonoBehaviour {
    public GravityController.GravityState m_DefaultAction;
    private GravityController.GravityState m_StartAction;
    private GravityController.GravityState m_CurrentAction;

    void Start () {
        m_StartAction = m_DefaultAction;
        m_CurrentAction = m_DefaultAction;
    }

    public void SetCurrentAction (GravityController.GravityState g) {
        m_CurrentAction = g;
    }

    public void Reset () {
        m_CurrentAction = m_StartAction;
    }

    private void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "Player") {
            GravityController.Instance.ChangeGravity (m_CurrentAction);
        }
    }
}
