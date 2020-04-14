using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGeneratorMesh : MonoBehaviour
{
    public FastNoise CloudNoise = new FastNoise();
    public FastNoise.NoiseType CloudNoiseType;
    public int CloudResolution = 256;
    public float NoiseFrequency;
    public float CloudSpeed = 1.0f;

    public FastNoise CloudNoise2 = new FastNoise();
    public FastNoise.NoiseType CloudNoiseType2;
    public float NoiseFrequency2;
    public float CloudSpeed2 = 1.0f;

    public float CloudClip; // unlike the CloudClip in CloudGenerator.cs, which takes byte values between 0 and 255, this takes float values between 0 and 1

    public bool MoveWithPlayer;
    public float CloudHeight = 200;
    public Transform Player;

    private float cloudX = 0f;
    private float cloudY = 0f;
    private float cloudX2 = 0f;
    private float cloudY2 = 0f;

    private Mesh mesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    void Start() {
        CloudNoise.SetNoiseType(CloudNoiseType);
        CloudNoise.SetFrequency(NoiseFrequency);
        CloudNoise.SetSeed(Random.Range(int.MinValue, int.MaxValue));

        CloudNoise2.SetNoiseType(CloudNoiseType2);
        CloudNoise2.SetFrequency(NoiseFrequency2);
        CloudNoise2.SetSeed(Random.Range(int.MinValue, int.MaxValue));

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update() {
        cloudX += CloudSpeed * Time.deltaTime;
        cloudY += CloudSpeed * Time.deltaTime;
        cloudX2 += CloudSpeed2 * Time.deltaTime;
        cloudY2 += CloudSpeed2 * Time.deltaTime;

        CloudNoise.SetNoiseType(CloudNoiseType);
        CloudNoise.SetFrequency(NoiseFrequency);
        GenerateCloudsMesh();

        if (MoveWithPlayer) {
            transform.position = new Vector3(Player.position.x - (CloudResolution / 2) * transform.localScale.x, CloudHeight, Player.position.z - (CloudResolution / 2) * transform.localScale.z);
        }
    }

    void GenerateCloudsMesh () {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        int face = 0;
        for (int x = 0; x < CloudResolution; x++) {
            for (int y = 0; y < CloudResolution; y++) {
                float sample1 = (CloudNoise.GetNoise(cloudX + x, cloudY + y) + 1) / 2f; // sample 1
                float sample2 = (CloudNoise2.GetNoise(cloudX2 + x, cloudY2 + y) + 1) / 2f; // sample 2, used to make the clouds change shape over time to make them seem more lifelike
                float lerpedSample = Mathf.Lerp(sample1, sample2, 0.5f);
                if (lerpedSample < CloudClip)
                    continue;

                // generate vertices for this square
                vertices.Add(new Vector3(x, 0, y));
                vertices.Add(new Vector3(x+1, 0, y));
                vertices.Add(new Vector3(x+1, 0, y+1));
                vertices.Add(new Vector3(x, 0, y+1));

                // and some more for the top face
                vertices.Add(new Vector3(x, 0, y + 1));
                vertices.Add(new Vector3(x + 1, 0, y + 1));
                vertices.Add(new Vector3(x + 1, 0, y));
                vertices.Add(new Vector3(x, 0, y));
                
                // generate triangles for this square
                triangles.Add(face*4);
                triangles.Add(face * 4 + 1);
                triangles.Add(face * 4 + 2);
                triangles.Add(face * 4);
                triangles.Add(face * 4 + 2);
                triangles.Add(face * 4 + 3);

                // and again for the top face
                triangles.Add((face+1) * 4);
                triangles.Add((face + 1) * 4 + 1);
                triangles.Add((face + 1) * 4 + 2);
                triangles.Add((face + 1) * 4);
                triangles.Add((face + 1) * 4 + 2);
                triangles.Add((face + 1) * 4 + 3);

                // generate uvs for this square
                uvs.Add(new Vector2(x / (float)CloudResolution, y / (float)CloudResolution));
                uvs.Add(new Vector2((x + 1) / (float)CloudResolution, y / (float)CloudResolution));
                uvs.Add(new Vector2((x + 1) / (float)CloudResolution, (y + 1) / (float)CloudResolution));
                uvs.Add(new Vector2(x / (float)CloudResolution, (y + 1) / (float)CloudResolution));

                // and once more for the top face
                uvs.Add(new Vector2(x / (float)CloudResolution, y / (float)CloudResolution));
                uvs.Add(new Vector2((x + 1) / (float)CloudResolution, y / (float)CloudResolution));
                uvs.Add(new Vector2((x + 1) / (float)CloudResolution, (y + 1) / (float)CloudResolution));
                uvs.Add(new Vector2(x / (float)CloudResolution, (y + 1) / (float)CloudResolution));

                face+=2;
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }
}
