using System;
using UnityEngine;

[Serializable]
public class Block
{
    public string name = "Undefined";
    public Vector2[] textures = new Vector2[] { Vector2.zero };
    public bool translucent = false;
    public bool invincible = false;
    public string soundType = "none";
    public string[] drops;

    public Vector2 GetTexture(Faces side) {
        if (textures.Length == 6) {
            return textures[(int)side];
        } else {
            return textures[0];
        }
    }
}

public enum Faces
{
    Top, Bottom, Left, Right, Front, Back
}
