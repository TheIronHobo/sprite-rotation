using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class QuatIterator : MonoBehaviour {
    public float rotationSpeed = 1f;
    private List<Quaternion> quaternions = new List<Quaternion>();
    private int currentQuaternionIndex = 0;
    private float transitionTime;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start() {
        ReadQuaternionsFromFile("Resources/Quaternions.json");
        if (quaternions.Count > 0) {
            transform.rotation = quaternions[0]; // Initialize rotation
        }
    }

    // Update is called once per frame
    void Update() {
        if (quaternions.Count > 0) {
            currentTime += Time.deltaTime;
            if (currentTime >= transitionTime) {
                currentQuaternionIndex = (currentQuaternionIndex + 1) % quaternions.Count;
                currentTime = 0f;
            }

            Quaternion targetQuaternion = quaternions[currentQuaternionIndex];
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, currentTime / transitionTime);
        }
    }

    private void ReadQuaternionsFromFile(string fileName) {
        string path = Path.Combine(Application.dataPath, fileName);
        if (File.Exists(path)) {
            string jsonContents = File.ReadAllText(path);
            QuaternionsData data = JsonUtility.FromJson<QuaternionsData>(jsonContents);
            foreach (var quat in data.quaternions) {
                quaternions.Add(new Quaternion(quat.x, quat.y, quat.z, quat.w));
            }
            transitionTime = 1f / rotationSpeed; // Adjust based on rotation speed
        }
        else {
            Debug.LogError("File not found: " + path);
        }
    }

    [System.Serializable]
    public class QuaternionData {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    [System.Serializable]
    public class QuaternionsData {
        public List<QuaternionData> quaternions;
    }
}
