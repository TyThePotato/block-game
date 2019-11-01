using System;
using UnityEngine;

[Serializable]
public class Item
{
    public string name = "Nothing";
    public ItemType itemType = ItemType.Tool;
    public Texture2D icon = null;
    public GameObject itemInHand;
}

public enum ItemType
{
    Block, Tool
}
