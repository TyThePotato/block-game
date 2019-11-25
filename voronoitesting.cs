using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Unity;
using LibNoise.Unity.Generator;

public class voronoitesting : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    public Renderer r1;
    public Renderer r2;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.

    public FastNoise.NoiseType noiseType;
    public float perlinScale;
    public float displacement;
    public int seed;
    public float freq;
    public float jitter;
    public int ci1;
    public int ci2;
    public bool useDist;

    private Texture2D noiseTex;
    private Texture2D noiseTex2;
    private Color[] pix;
    private FastNoise vore = new FastNoise();

    void Start() {

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        noiseTex2 = new Texture2D(pixWidth, pixHeight);

        noiseTex.filterMode = FilterMode.Point;
        noiseTex2.filterMode = FilterMode.Point;

        pix = new Color[noiseTex.width * noiseTex.height];
        r1.material.mainTexture = noiseTex;
        r1.material.mainTexture = noiseTex2;

        CalcNoise();
    }

    void CalcNoise () {

    }

    void CalcFastNoise() {
        // For each pixel in the texture...
        float y = 0.0F;

        vore.SetNoiseType(noiseType);
        vore.SetFrequency(freq);
        vore.SetSeed(seed);

        while (y < noiseTex.height) {
            float x = 0.0F;
            while (x < noiseTex.width) {
                float xCoord = xOrg + x / noiseTex.width * perlinScale;
                float yCoord = yOrg + y / noiseTex.height * perlinScale;
                float sample = (1 + vore.GetNoise(xCoord, yCoord))/2;
                //float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    /*
    void  CalcNoiseNoise() {
        //int x, int y, int z, float scale, float yScale, float height, float power, int seed
        float rVal = Noise.Noise.GetNoise(((double)x + seed) / scale, ((double)y + seed) / yScale, ((double)z + seed) / scale);
        rVal *= height;

        if (System.Math.Abs(power) > 0) {
            rVal = Mathf.Pow(rVal, power);
        }
        return rVal;
    }

    void Update() {
        CalcNoise();
    }
    */
}
