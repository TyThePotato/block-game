using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
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

    public byte CloudClip;

    public bool MoveWithPlayer;
    public float CloudHeight = 200;
    public Transform Player;

    private float cloudX = 0f;
    private float cloudY = 0f;
    private float cloudX2 = 0f;
    private float cloudY2 = 0f;

    private Texture2D noiseTex;
    private Color32[] pix;
    private Material mat;

    private int frameskip = 0;
    private int desiredFrameskips = 2;

    void Start () {
        noiseTex = new Texture2D(CloudResolution, CloudResolution);
        noiseTex.filterMode = FilterMode.Point;
        pix = new Color32[CloudResolution * CloudResolution];
        mat = GetComponent<MeshRenderer>().sharedMaterial;
        mat.mainTexture = noiseTex;
        
        CloudNoise.SetNoiseType(CloudNoiseType);
        CloudNoise.SetFrequency(NoiseFrequency);
        CloudNoise.SetSeed(Random.Range(int.MinValue, int.MaxValue));

        CloudNoise2.SetNoiseType(CloudNoiseType2);
        CloudNoise2.SetFrequency(NoiseFrequency2);
        CloudNoise2.SetSeed(Random.Range(int.MinValue, int.MaxValue));
    }

    void Update () {
        cloudX += CloudSpeed * Time.deltaTime;
        cloudY += CloudSpeed * Time.deltaTime;
        cloudX2 += CloudSpeed2 * Time.deltaTime;
        cloudY2 += CloudSpeed2 * Time.deltaTime;

        CloudNoise.SetNoiseType(CloudNoiseType);
        CloudNoise.SetFrequency(NoiseFrequency);
        GenerateClouds();

        if(MoveWithPlayer) {
            transform.position = new Vector3(Player.position.x, CloudHeight, Player.position.z);
        }
    }

    void GenerateClouds () {
        for (int x = 0; x < CloudResolution; x++) {
            for (int y = 0; y < CloudResolution; y++) {
                // funny math is because FastNoise returns a value between -1 and 1, but we want a value between 0 and 1
                float sample1 = (CloudNoise.GetNoise(cloudX + x, cloudY + y) + 1) / 2f;
                float sample2 = (CloudNoise2.GetNoise(cloudX2 + x, cloudY2 + y) + 1) / 2f;
                byte finalSample = (byte)(Mathf.Lerp(sample1, sample2, sample2)*255);
                byte a = (byte)(sample2 * 255);
                pix[y * CloudResolution + x] = new Color32(255,255,255, (a > CloudClip) ? (byte)180 : (byte)0);
            }
        }
        noiseTex.SetPixels32(pix);
        noiseTex.Apply();
    }
}
