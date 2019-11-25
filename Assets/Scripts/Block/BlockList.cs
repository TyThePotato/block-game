using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{
    public static BlockList instance;

    public TextAsset BlockListText;
    public Dictionary<string, Block> blocks = new Dictionary<string, Block>();

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
        }
        Debug.Log($"Registered {blocks.Count} Blocks");
    }
}

[Serializable]
public class _BL
{
    public Block[] blocks;
}
