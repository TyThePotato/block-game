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

    // "real" modulo and not % which is actually remainder
    public static int Mod(this int a, int b) {
        return (a % b + b) % b;
    }

    // calculates mesh tangents
    public static void CalculateMeshTangents(Mesh mesh) {
        //speed up math by copying the mesh arrays
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        //variable definitions
        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3) {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            //float r = 1.0f / (s1 * t2 - s2 * t1);
            float div = s1 * t2 - s2 * t1;
            float r = div == 0.0f ? 0.0f : 1.0f / div;

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (long a = 0; a < vertexCount; ++a) {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];

            //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}
