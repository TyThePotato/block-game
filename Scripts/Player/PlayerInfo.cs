using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFSW.QC;

public class PlayerInfo : MonoBehaviour
{
    public int Health;
    public int HotbarIndex;
    public RectTransform HotbarSelector;

    public BlockBreaker bb;
    public PlayerControl playerControl;
    public QuantumConsole qc;
    public Texture NoTexture;
    public Item[] HotbarItems;
    public RawImage[] HotbarIcons;
    public Transform handPoint;
    public Animator handAnimator;
    public bool CanUseHotbar = true;
    public bool CanInteract = true;

    private int oldHotbarIndex = -1;

    private void Start() {
        qc.OnActivate += ConsoleOpen;
        qc.OnDeactivate += ConsoleClose;

        HotbarItems = new Item[10];
        HotbarItems[0] = ItemList.instance.items["Grass"];
        HotbarItems[1] = ItemList.instance.items["Dirt"];
        HotbarItems[2] = ItemList.instance.items["Stone"];
        HotbarItems[3] = ItemList.instance.items["Sand"];
        HotbarItems[4] = ItemList.instance.items["Wood"];
        HotbarItems[5] = ItemList.instance.items["Stone Pickaxe"];
        HotbarItems[6] = ItemList.instance.items["Iron Pickaxe"];
        HotbarItems[7] = ItemList.instance.items["Diamond Pickaxe"];
        HotbarItems[8] = ItemList.instance.items["Torch"];
        UpdateHotbar();
    }

    private void UpdateHotbar () {
        for (int i = 0; i < 10; i++) {
            if(HotbarItems[i] == null || HotbarItems[i].icon == null) {
                HotbarIcons[i].texture = NoTexture;
            } else {
                HotbarIcons[i].texture = HotbarItems[i].icon;
            }
        }
    }

    private void Update() {
        if (CanInteract) {
            if (Input.GetMouseButtonDown(0)) {
                // l click
                bb.BreakBlock(false);
                SwingHand();
            }

            if (Input.GetMouseButtonDown(1)) {
                // r click
                bb.PlaceBlock();
                SwingHand();
            }
        }

        if (CanUseHotbar) {
            for (int k = 0; k < 10; k++) {
                if (Input.GetKeyDown(k.ToString())) {
                    if(k == 0) {
                        HotbarIndex = 9;
                    } else {
                        HotbarIndex = k-1;
                    }
                }
            }

            float sw = Input.GetAxisRaw("Mouse ScrollWheel");
            if (sw > 0) {
                HotbarIndex = (int)Mathf.Repeat(HotbarIndex-1, 10); // i wanted to do some epic modulus for this, but c#'s "modulo" operator isnt actually modulo but rather remainder, which doesnt work the same with negative numbers, oof
            } else if (sw < 0) {
                HotbarIndex = (int)Mathf.Repeat(HotbarIndex+1, 10); 
            }
        }

        if(HotbarIndex != oldHotbarIndex) {
            UpdateHeldItem();
            HotbarSelector.anchoredPosition = new Vector2(-432 + (96 * HotbarIndex), 0); // moves the hotbar selection box to the correct spot using epic math
        }

        oldHotbarIndex = HotbarIndex;
    }

    void UpdateHeldItem () {
        if (HotbarItems[HotbarIndex] != null && HotbarItems[HotbarIndex].itemType == ItemType.Block) {
            bb.currentlySelectedBlock = BlockList.instance.blocks[HotbarItems[HotbarIndex].name];
        } else {
            bb.currentlySelectedBlock = null;
        }
        SetItemInHand(HotbarItems[HotbarIndex]);
    }

    void SetItemInHand (Item item) {
        if (handPoint.childCount > 0) {
            Transform oldItem = handPoint.GetChild(0);
            oldItem.SetParent(null);
            oldItem.position = new Vector3(-5,-5,-5);
            oldItem.gameObject.GetComponent<ToolInfo>().Disable();
            oldItem.gameObject.SetActive(false);
        }
        if (item != null && item.itemInHand != null) {
            GameObject itemInHand = item.itemInHand;
            itemInHand.transform.SetParent(handPoint);
            itemInHand.transform.localPosition = Vector3.zero;
            itemInHand.transform.localEulerAngles = Vector3.zero;
            itemInHand.SetActive(true);
            itemInHand.GetComponent<ToolInfo>().Enable();
        }
    }

    [Command("setslot")]
    void SetHotbarSlot (int slot, string item) {
        int _s = Mathf.Clamp(slot, 1, 10);
        if (ItemList.instance.items.TryGetValue(item, out Item _i)) {
            HotbarItems[_s-1] = _i;
            UpdateHotbar();
            UpdateHeldItem();
            QuantumConsole.print($"Successfully set slot {_s} to '{item}'");
        } else {
            QuantumConsole.print("Cannot set hotbar slot, chosen item doesn't exist.");
        }
    }

    private void ConsoleOpen () {
        playerControl.movementEnabled = false;
        playerControl.cameraEnabled = false;
        playerControl.cursorLocked = false;
        CanInteract = false;
        CanUseHotbar = false;
    }

    private void ConsoleClose () {
        playerControl.movementEnabled = true;
        playerControl.cameraEnabled = true;
        playerControl.cursorLocked = true;
        CanInteract = true;
        CanUseHotbar = true;
    }

    void SwingHand () {
        handAnimator.Play("Swing");
    }
}
