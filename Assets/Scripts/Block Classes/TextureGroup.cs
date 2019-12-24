using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGroup
{
    public Material mainMaterial;
    public Texture2D diffuse;
    public Texture2D normal;
    public Texture2D occlusion;
    // TODO: emission?

    public void UpdateMaterial () {
        // enable keywords
        mainMaterial.EnableKeyword("_NORMALMAP");
        mainMaterial.EnableKeyword("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A");

        // set textures
        mainMaterial.SetTexture("_MainTex", diffuse);
        mainMaterial.SetTexture("_BumpMap", normal);
        mainMaterial.SetTexture("_OcclusionTex", occlusion);
    }
}
