using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour {
    public float speed = 10f; // Speed of the rotation
    public Vector3 rotationAxis = Vector3.up; // Default rotation axis is up (Y-axis)
    public bool useParentSpace = false; // Whether to rotate in parent's space or local space

    void Update() {
        if (useParentSpace && transform.parent) {
            Vector3 globalRotationAxis = transform.parent.TransformDirection(rotationAxis);
            transform.RotateAround(transform.position, globalRotationAxis, speed * Time.deltaTime);
        }
        else {
            transform.Rotate(rotationAxis, speed * Time.deltaTime, Space.Self);
        }
    }
}
