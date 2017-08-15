using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoundary : MonoBehaviour {
    public float m_TimeOut = 1;
    private bool m_Out = false;
    private float m_CurrentTimeOut;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (m_Out) {
            m_CurrentTimeOut += Time.deltaTime;

            if (m_CurrentTimeOut > m_TimeOut) {
                GameController.Instance.ResetPlayer ();
            }
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            m_Out = false;
            m_CurrentTimeOut = 0;
        }
    }

    private void OnTriggerExit (Collider other) {
        if (other.tag == "Player") {
            m_Out = true;
            m_CurrentTimeOut = 0;
        }
    }
}
