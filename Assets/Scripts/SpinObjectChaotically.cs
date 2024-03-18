using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObjectChaotically : MonoBehaviour {
    private Quaternion initialRotation;

    public float xSpeed;

    public float ySpeed;

    public float zSpeed;

    private float smallifier = 10f;
    // Start is called before the first frame update
    void Start() {
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate() {
        transform.rotation = initialRotation * Quaternion.Euler(smallifier * (Time.time * xSpeed), smallifier * (Time.time * ySpeed), smallifier * (Time.time * zSpeed));
    }
}
