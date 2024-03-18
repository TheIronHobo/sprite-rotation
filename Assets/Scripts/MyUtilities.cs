using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace MyUtilities {


    public static class VectorExtensions {
        public static Vector2 Flatten(this Vector3 v) {
            return new Vector2(v.x, v.y);
        }
    }

    public class CommonFunctions {
        public static float dpThreshhold = 0.65f;
        public static float RB_PIXEL_TO_MASS_MODIFIER = 1.0f;


        public static float[][] SampleQuaternionData(List<Quaternion> inputQuats) {
            var data = new List<float[]>();

            for (int i = 0; i < inputQuats.Count; i++) {
                Quaternion quat = inputQuats[i];
                data.Add(new float[] { quat.x, quat.y, quat.z, quat.w });
            }
            return data.ToArray();
        }

        public static Quaternion StandardizeQuaternion(Quaternion input, int i = 0) {
            if (i == 4) {
                UnityEngine.Debug.LogError("Invalid quaternion");
                return input;
            }
            if (input[i] < 0) {
                Quaternion temp = Quaternion.identity;
                temp.Set(input[0] * -1f, input[1] * -1f, input[2] * -1f, input[3] * -1f);
                return temp;
            }
            else if (input[i] == 0) {
                return StandardizeQuaternion(input, i + 1);
            }
            else {
                return input;
            }
        }


        public static List<Texture2D> GenerateFragmentList(Texture2D inputTex) {
            List<Texture2D> fragments = new List<Texture2D>();

            while (ContainsBlackPixel(inputTex)) {
                (Texture2D filled, Texture2D fragment) = FloodFillBlackPixel(inputTex);

                // Add the fragment to the list of fragments
                fragments.Add(fragment);

                // Update inputTex to be the filled texture, so next iteration will find next fragment
                inputTex = filled;
            }
            return fragments;
        }

        public static bool ContainsBlackPixel(Texture2D tex) {
            for (int y = 0; y < tex.height; y++) {
                for (int x = 0; x < tex.width; x++) {
                    if (tex.GetPixel(x, y) == Color.black) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static (Texture2D, Texture2D) FloodFillBlackPixel(Texture2D original) {
            Texture2D filledTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
            filledTexture.SetPixels(original.GetPixels());
            filledTexture.filterMode = FilterMode.Point;

            Texture2D filledAreaTexture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
            Color[] whitePixels = new Color[original.width * original.height];
            for (int i = 0; i < whitePixels.Length; i++) {
                whitePixels[i] = Color.white;
            }
            filledAreaTexture.SetPixels(whitePixels);
            filledAreaTexture.filterMode = FilterMode.Point;

            Color black = Color.black;
            Color fillCol = Color.white;  // You can choose any distinct color for filling.

            for (int y = 0; y < filledTexture.height; y++) {
                for (int x = 0; x < filledTexture.width; x++) {
                    if (filledTexture.GetPixel(x, y) == black) {
                        // Perform flood fill
                        RecursiveFill(filledTexture, filledAreaTexture, x, y, black, fillCol);
                        filledTexture.Apply();  // Apply changes after flood fill
                        filledAreaTexture.Apply();
                        return (filledTexture, filledAreaTexture); // Return both textures after first flood fill
                    }
                }
            }
            filledTexture.Apply();  // Apply any other changes, just to be sure.
            filledAreaTexture.Apply();
            return (filledTexture, filledAreaTexture); // In case no black pixel is found, return both textures.
        }

        public static void RecursiveFill(Texture2D tex, Texture2D filledAreaTex, int x, int y, Color targetColor, Color fillColor) {
            if (x < 0 || x >= tex.width || y < 0 || y >= tex.height)
                return;

            if (tex.GetPixel(x, y) != targetColor)
                return;

            tex.SetPixel(x, y, fillColor);
            filledAreaTex.SetPixel(x, y, targetColor);  // Set the filled pixel on the filled area texture to black

            // Recursively call surrounding pixels
            RecursiveFill(tex, filledAreaTex, x + 1, y, targetColor, fillColor);
            RecursiveFill(tex, filledAreaTex, x - 1, y, targetColor, fillColor);
            RecursiveFill(tex, filledAreaTex, x, y + 1, targetColor, fillColor);
            RecursiveFill(tex, filledAreaTex, x, y - 1, targetColor, fillColor);
        }

        public static int CountPixelsOfColor(Texture2D texture, Color targetColor, float tolerance = 0.05f) {
            if (texture == null)
                throw new System.ArgumentNullException("Texture cannot be null.");

            int count = 0;

            for (int i = 0; i < texture.width; i++) {
                for (int j = 0; j < texture.height; j++) {
                    Color pixelColor = texture.GetPixel(i, j);

                    // Calculate the difference between the pixelColor and targetColor for each component
                    float rDiff = Mathf.Abs(pixelColor.r - targetColor.r);
                    float gDiff = Mathf.Abs(pixelColor.g - targetColor.g);
                    float bDiff = Mathf.Abs(pixelColor.b - targetColor.b);
                    float aDiff = Mathf.Abs(pixelColor.a - targetColor.a);

                    // If all components are within the tolerance range, count the pixel
                    if (rDiff <= tolerance && gDiff <= tolerance && bDiff <= tolerance && aDiff <= tolerance) {
                        count++;
                    }
                }
            }

            return count;
        }

        public static Quaternion SlerpWithMaxSpeed(Quaternion current, Quaternion target, float maxDegreesPerSecond, float deltaTime) {
            float angleToTarget = Quaternion.Angle(current, target);

            if (angleToTarget > 0.0f) {
                float t = Mathf.Min((maxDegreesPerSecond * deltaTime) / angleToTarget, 1.0f);
                return Quaternion.Slerp(current, target, t);
            }

            return current;
        }
        public static List<Quaternion> LoadQuaternionsFromJSON(string path) {
            string json = File.ReadAllText(path);
            QuaternionContainer container = JsonUtility.FromJson<QuaternionContainer>(json);
            List<Quaternion> loadedQuaternions = new List<Quaternion>();

            foreach (var quatData in container.quaternions) {
                loadedQuaternions.Add(CommonFunctions.StandardizeQuaternion(new Quaternion(quatData.x, quatData.y, quatData.z, quatData.w)));
            }

            return loadedQuaternions;
        }

        public static float Mod360(float input) {
            return ((input + 360) % 360);
        }

        public static float SnapToNearestInt(float value, int integer) {
            return Mathf.Round(value / (float)integer) * (float)integer;
        }



    }




    [System.Serializable]
    public class QuaternionData {
        public float x;
        public float y;
        public float z;
        public float w;
        // Constructor to easily create a QuaternionData from Unity's Quaternion
        public QuaternionData(Quaternion quaternion) {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
    }

    public class QuaternionContainer {
        public List<QuaternionData> quaternions;
    }


}



