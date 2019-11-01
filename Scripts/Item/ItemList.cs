using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    public static ItemList instance;

    private GenerateIcons gi;
    public float BlockCameraSize;
    public float ItemCameraSize;
    public BlockPreview blockPreview;
    public List<GameObject> itemsToGen;
    public List<GameObject> blocksToGen;
    public Dictionary<string, Item> items = new Dictionary<string, Item>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
        gi = GetComponent<GenerateIcons>();
        RegisterItems();
        RegisterBlocks();
    }

    void GetBlocks () {
        foreach (KeyValuePair<string, Block> kvp in BlockList.instance.blocks) {
            GameObject blockGO = blockPreview.GenerateBlockMesh(kvp.Value);
            blockGO.transform.position = new Vector3(-10, -10, -10);
            blockGO.AddComponent<ToolInfo>();
            blocksToGen.Add(blockGO);
        }
    }

    void RegisterItems () {
        for (int i = 0; i < itemsToGen.Count; i++) {
            itemsToGen[i].layer = 10;
            for (int j = 0; j < itemsToGen[i].transform.childCount; j++) {
                itemsToGen[i].transform.GetChild(j).gameObject.layer = 10;
            }
            Item newItem = new Item();
            newItem.name = itemsToGen[i].name;
            newItem.itemType = ItemType.Tool;
            newItem.icon = gi.GenerateIcon(itemsToGen[i], ItemCameraSize);
            newItem.itemInHand = itemsToGen[i];
            items.Add(newItem.name, newItem);
        }
    }

    void RegisterBlocks () {
        GetBlocks();
        for (int i = 0; i < blocksToGen.Count; i++) {
            blocksToGen[i].layer = 10;
            for (int j = 0; j < blocksToGen[i].transform.childCount; j++) {
                blocksToGen[i].transform.GetChild(j).gameObject.layer = 10;
            }
            Item newItem = new Item();
            newItem.name = blocksToGen[i].name;
            newItem.itemType = ItemType.Block;
            newItem.icon = gi.GenerateIcon(blocksToGen[i], BlockCameraSize);
            newItem.itemInHand = blocksToGen[i];
            blocksToGen[i].transform.localScale = new Vector3(0.33f,0.33f,0.33f);
            items.Add(newItem.name, newItem);
        }
    }
}
