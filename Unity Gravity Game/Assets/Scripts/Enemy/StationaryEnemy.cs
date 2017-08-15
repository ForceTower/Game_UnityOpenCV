using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemy : MonoBehaviour {
    public Transform LeftShoot;
    public Transform RightShoot;
    public bool Shooting;

    public float ReloadTime;
    private Transform left;
    private Transform positions;
    private Transform right;

    // Use this for initialization
    void Start () {
        
        Shooting = true;

        positions = transform.GetChild (0);
        right = positions.GetChild (0);
        left = positions.GetChild (1);

        StartCoroutine (Shoot ());
    }

    public IEnumerator Shoot() {
        while (Shooting) {
            Instantiate (LeftShoot, left.position, Quaternion.identity);
            Instantiate (RightShoot, right.position, Quaternion.identity);
            yield return new WaitForSeconds (ReloadTime);
        }
    }
}
