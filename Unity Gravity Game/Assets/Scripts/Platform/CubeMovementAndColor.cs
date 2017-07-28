using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovementAndColor : MonoBehaviour {
    private float _DeltaTime;
    private bool _Ready;

    private Vector3 _StartPosition;
    private Vector3 _EndPosition;
    private Color _StartColor;
    private Color _EndColor;
    private float _ExpectedTime;

    private PlayerController.GravityModifier _CurrentGravity;

    // Use this for initialization
    void Start () {
        _DeltaTime = 0;
        _Ready = false;
    }
	
    // Update is called once per frame
    void Update () {
        if (_Ready)
            ChangeColorAndPosition ();
    }

    public void ChangeToColorAndPosition(Vector3 position, Color color, float time) {
        _Ready = true;
        _DeltaTime = 0;
        _StartPosition = transform.position;
        _StartColor = GetComponent<MeshRenderer> ().material.color;

        _EndPosition = position;
        _EndColor = color;
        _ExpectedTime = time;
    }

    private void ChangeColorAndPosition () {
        if (_DeltaTime >= 1) {
            _Ready = false;
            OnFinish ();
            _DeltaTime = 0;
        } else {
            _DeltaTime += Time.deltaTime / _ExpectedTime;
            transform.position = Vector3.Lerp (_StartPosition, _EndPosition, _DeltaTime);
            GetComponent<MeshRenderer> ().material.color = Color.Lerp (_StartColor, _EndColor, _DeltaTime);
        }
    }

    void OnFinish () {
        if (_EndColor == Color.red)
            _CurrentGravity = PlayerController.GravityModifier.UP;
        else if (_EndColor == Color.blue)
            _CurrentGravity = PlayerController.GravityModifier.DOWN;
        else if (_EndColor == Color.green)
            _CurrentGravity = PlayerController.GravityModifier.LEFT;
        else if (_EndColor == Color.yellow)
            _CurrentGravity = PlayerController.GravityModifier.RIGHT;
        else if (_EndColor == Color.black)
            _CurrentGravity = PlayerController.GravityModifier.NONE;

        gameObject.tag = "Gravity Type " + _CurrentGravity;
    }
}
