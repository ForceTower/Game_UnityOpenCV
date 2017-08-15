using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance { get; set; }
    public int MaxHealth;
    public int CurrentHealth;

    private CharacterController character;
    private Vector3 m_Start_Position;

    private void Awake () {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        m_Start_Position = transform.position;
        character = GetComponent<CharacterController> ();
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void FixedUpdate () {
        PlayerInput ();
    }

    private void PlayerInput () {
        float x = Input.GetAxis ("Horizontal");
        float y = Input.GetAxis ("Vertical");

        bool jump = Input.GetButtonDown ("Jump");

        Vector3 move = new Vector3 (x, y, 0) * character.m_Speed;

        move *= Time.deltaTime;

        character.Move (move, jump);
    }

    public void TakeDamage (int ammount) {
        CurrentHealth -= ammount;
        if (CurrentHealth <= 0) {
            gameObject.SetActive (false);
        }
    }

    public void Reset () {
        transform.position = m_Start_Position;
        character.Stop ();
        CurrentHealth = MaxHealth;
    }
}
