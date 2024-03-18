using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuatMath : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Quaternion a;
        Quaternion b;

        // A {"x":-0.7071067690849304,"y":0.0,"z":0.0,"w":0.7071067690849304},

        // b    "quaternions": [
        /*        {
                    "x": 0.0,
                    "y": 0.0,
                    "z": 0.0,
                    "w": 1.0
                },*/

        a.x = -0.7071067690849304f;
        a.y = 0.0f;
        a.z = 0.0f;
        a.w = 0.7071067690849304f;


        b.x = 0.0f;
        b.y = 0.0f;
        b.z = 0.0f;
        b.w = 1f;

        a = MyUtilities.CommonFunctions.StandardizeQuaternion(a);
        b = MyUtilities.CommonFunctions.StandardizeQuaternion(b);

        Quaternion rotationFromBToA = a * Quaternion.Inverse(b);

        Quaternion result = MyUtilities.CommonFunctions.StandardizeQuaternion(b * rotationFromBToA);

        Debug.Log(" w " + rotationFromBToA.w + " x " + rotationFromBToA.x + " y " + rotationFromBToA.y + " z " + rotationFromBToA.z);

    }

    // Update is called once per frame
    void Update() {

    }
}
