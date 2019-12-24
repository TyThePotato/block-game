using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    public static BlockList instance;

    public TextAsset BlockListText; // json file to load blocks from
    public Dictionary<string, Block> blocks = new Dictionary<string, Block>(); // dictionary of all the registered blocks
    public Dictionary<string, TextureGroup> textureGroups = new Dictionary<string, TextureGroup>(); // dictionary of all the registered texturegroups

    public Material blockMaterialTemplate;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
        RegisterBlocks(BlockListText.text);
    }

    private void RegisterBlocks(string blockJSON) {
        _BL bl = JsonUtility.FromJson<_BL>(blockJSON);
        for (int i = 0; i < bl.blocks.Length; i++) {
            blocks.Add(bl.blocks[i].name, bl.blocks[i]);
            for (int j = 0; j < bl.blocks[i].textures.Length; j++) {
                RegisterTextureGroup(bl.blocks[i].textures[j]);
            }
        }
        Debug.Log($"Registered {blocks.Count} Blocks");
    }

    private void RegisterTextureGroup (string texture) {
        // avoid duplicate texturegroups
        if (textureGroups.ContainsKey(texture))
            return;

        TextureGroup tg = new TextureGroup();
        tg.mainMaterial = new Material(blockMaterialTemplate);
        tg.diffuse = Resources.Load<Texture2D>("blocks/" + texture);
        tg.normal = Resources.Load<Texture2D>("blocks/" + texture + "_n");
        tg.occlusion = Resources.Load<Texture2D>("blocks/" + texture + "_o");
        tg.UpdateMaterial();

        textureGroups.Add(texture, tg);
    }
}

[Serializable]
public class _BL
{
    public Block[] blocks;
}
