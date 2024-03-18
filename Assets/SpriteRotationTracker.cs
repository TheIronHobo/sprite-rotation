using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotationTracker : MonoBehaviour {

    Sprite3DRotator sprite3DRotator;
    // Start is called before the first frame update
    void Awake() {
        sprite3DRotator = transform.root.GetComponentInChildren<Sprite3DRotator>();
    }

    // Update is called once per frame
    void Update() {
        transform.rotation = sprite3DRotator.fittestXYZRotation;
    }
}
