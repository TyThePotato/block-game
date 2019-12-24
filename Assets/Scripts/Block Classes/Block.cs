using System;
using UnityEngine;

[Serializable]
public class Block
{
    public string name = "Undefined";
    public string[] textures = new string[] { "stone" }; // todo: replace with error texture
    public bool translucent = false;
    public bool invincible = false;
    public string soundType = "none";

    public TextureGroup GetTextureGroup(Faces side) {
        return textures.Length == 6 ? BlockList.instance.textureGroups[textures[(int)side]] : BlockList.instance.textureGroups[textures[0]];
    }
}

public enum Faces
{
    Top, Bottom, Left, Right, Front, Back
}
