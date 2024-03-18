using System.Collections.Generic;
using UnityEngine;
using MyUtilities;

public class Sprite3DRotator : MonoBehaviour {
    public GameObject target;
    public string quaternionFilePath = "Assets/Resources/Quaternions.json";
    private Renderer myRenderer;

    private List<Quaternion> quaternions;
    [HideInInspector]
    public Quaternion fittestXYZRotation;


    void Start() {
        myRenderer = GetComponent<Renderer>();
        quaternions = CommonFunctions.LoadQuaternionsFromJSON(quaternionFilePath);
    }

    void Update() {
        Quaternion targetObjectRotation = target.transform.rotation;
        float targetNoseVectorAngle = UpVectorAngleAroundXYAxis(targetObjectRotation);
        Quaternion zRotation = Quaternion.Euler(0f, 0f, targetNoseVectorAngle);
        Quaternion targetXYRotation = Quaternion.Inverse(zRotation) * targetObjectRotation;

        (int frame, Quaternion bestXYRotation) = FindClosestQuaternionFrame(targetXYRotation);

        PublishFrame(frame, zRotation);
    }

    private void PublishFrame(int frameIndex, Quaternion zTransform) {
        transform.rotation = zTransform;
        myRenderer.material.SetFloat("_Tile", frameIndex);
    }

    private (int, Quaternion) FindClosestQuaternionFrame(Quaternion targetRotation) {
        float smallestAngle = Mathf.Infinity;
        int closestFrame = 0;

        for (int i = 0; i < quaternions.Count; i++) {
            float angle = Quaternion.Angle(targetRotation, quaternions[i]);
            if (angle < smallestAngle) {
                closestFrame = i;
                smallestAngle = angle;
            }
        }
        return (closestFrame, quaternions[closestFrame]);
    }

    private float UpVectorAngleAroundXYAxis(Quaternion rotation) {
        Vector3 localUp = rotation * Vector3.up;
        Vector3 projectedNoseVector = Vector3.ProjectOnPlane(localUp, Vector3.forward);
        return Mathf.Atan2(projectedNoseVector.y, projectedNoseVector.x) * Mathf.Rad2Deg;
    }

    private void OnDrawGizmos() {

    }

}
