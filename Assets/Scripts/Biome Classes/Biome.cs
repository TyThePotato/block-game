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
    public string alternateSurfaceBlock;
    public string subSurfaceBlock;
    public string subSurfaceStoneTransitionBlock;
    public float alternateSurfaceBlockChance;

    // all fancy stuff
    public Biome (string Name, int Scale, int Height, float Multiplier, int RoughnessFactor, float RoughnessStrength, string SurfaceBlock, string AlternateSurfaceBlock, string SubSurfaceBlock, string SubSurfaceStoneTransitionBlock, float AlternateSurfaceBlockChance) {
        name = Name;
        scale = Scale;
        height = Height;
        multiplier = Multiplier;
        roughnessFactor = RoughnessFactor;
        roughnessStrength = RoughnessStrength;
        surfaceBlock = SurfaceBlock;
        alternateSurfaceBlock = AlternateSurfaceBlock;
        subSurfaceBlock = SubSurfaceBlock;
        subSurfaceStoneTransitionBlock = SubSurfaceStoneTransitionBlock;
        alternateSurfaceBlockChance = AlternateSurfaceBlockChance;
    }

    // no alternate surface blocks
    public Biome(string Name, int Scale, int Height, float Multiplier, int RoughnessFactor, float RoughnessStrength, string SurfaceBlock, string SubSurfaceBlock, string SubSurfaceStoneTransitionBlock) {
        name = Name;
        scale = Scale;
        height = Height;
        multiplier = Multiplier;
        roughnessFactor = RoughnessFactor;
        roughnessStrength = RoughnessStrength;
        surfaceBlock = SurfaceBlock;
        alternateSurfaceBlock = SurfaceBlock;
        subSurfaceBlock = SubSurfaceBlock;
        subSurfaceStoneTransitionBlock = SubSurfaceStoneTransitionBlock;
        alternateSurfaceBlockChance = 0.0f;
    }

    // no alternate surface blocks or transition block
    public Biome(string Name, int Scale, int Height, float Multiplier, int RoughnessFactor, float RoughnessStrength, string SurfaceBlock, string SubSurfaceBlock) {
        name = Name;
        scale = Scale;
        height = Height;
        multiplier = Multiplier;
        roughnessFactor = RoughnessFactor;
        roughnessStrength = RoughnessStrength;
        surfaceBlock = SurfaceBlock;
        alternateSurfaceBlock = SurfaceBlock;
        subSurfaceBlock = SubSurfaceBlock;
        subSurfaceStoneTransitionBlock = SubSurfaceBlock;
        alternateSurfaceBlockChance = 0.0f;
    }

    // no transition block
    public Biome(string Name, int Scale, int Height, float Multiplier, int RoughnessFactor, float RoughnessStrength, string SurfaceBlock, string AlternateSurfaceBlock, string SubSurfaceBlock, float AlternateSurfaceBlockChance) {
        name = Name;
        scale = Scale;
        height = Height;
        multiplier = Multiplier;
        roughnessFactor = RoughnessFactor;
        roughnessStrength = RoughnessStrength;
        surfaceBlock = SurfaceBlock;
        alternateSurfaceBlock = AlternateSurfaceBlock;
        subSurfaceBlock = SubSurfaceBlock;
        subSurfaceStoneTransitionBlock = SubSurfaceBlock;
        alternateSurfaceBlockChance = AlternateSurfaceBlockChance;
    }

    public void FillBlankValues () {
        // no alternate surface blocks
        if (alternateSurfaceBlock == null) {
            alternateSurfaceBlock = surfaceBlock;
            alternateSurfaceBlockChance = 0.0f;
        }
        // no transition block
        if (subSurfaceStoneTransitionBlock == null) {
            subSurfaceStoneTransitionBlock = subSurfaceBlock;
        }
    }
}
