using UnityEngine;

public static class Helper
{
    public static Vector3 Round(this Vector3 vector) {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector3Int RoundToInt(this Vector3 vector) {
        return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    public static Vector3 Floor(this Vector3 vector) {
        return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
    }

    public static Vector3Int FloorToInt(this Vector3 vector) {
        return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
    }

    public static Vector3 Ceil(this Vector3 vector) {
        return new Vector3(Mathf.Ceil(vector.x), Mathf.Ceil(vector.y), Mathf.Ceil(vector.z));
    }

    // Vector3 Damping, for translations
    public static Vector3 Damp(Vector3 a, Vector3 b, float t, float dt) {
        return Vector3.Lerp(a, b, 1 - Mathf.Pow(t, dt));
    }

    // Quaternion Damping, for rotations
    public static Quaternion Damp(Quaternion a, Quaternion b, float t, float dt) {
        return Quaternion.Slerp(a, b, 1 - Mathf.Pow(t, dt));
    }

    public static void CopyToClipboard(this string str) {
        TextEditor textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public static int Mod(this int a, int b) {
        return (a % b + b) % b;
    }
}
