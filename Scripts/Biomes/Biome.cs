using System;

[Serializable]
public class Biome
{
    public string name = "Undefined";
    public int scale = 100;
    public int height = 100;
    public float multiplier = 1;
    public int roughnessFactor;
    public float roughnessStrength;
    public string surfaceBlock;
    public string subSurfaceBlock;
}
