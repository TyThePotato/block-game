using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBreaker : MonoBehaviour
{
    // this script is for testing

    public Camera cam;
    public float range;
    public LayerMask layerMask;

    public Block currentlySelectedBlock;
    private int amountOfBlocks;
    private int selectedBlockIndex;

    [HideInInspector]
    public Vector3Int lookingAtPos = Vector3Int.zero;
    [HideInInspector]
    public Block lookingAt;

    private Vector3 lookingAtFacePos;

    public Transform SelectionQuadParent;
    public Transform SelectionQuad;

    private void Start() {
        amountOfBlocks = BlockList.instance.blocks.Count;
    }

    private void Update() {

        if (Input.GetMouseButtonDown(0)) {
            // l click
            BreakBlock();
        }

        if (Input.GetMouseButtonDown(1)) {
            // r click
            PlaceBlock();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            // up
            selectedBlockIndex = Mathf.Clamp(selectedBlockIndex+1,1,amountOfBlocks-1);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            // down
            selectedBlockIndex = Mathf.Clamp(selectedBlockIndex-1,1,amountOfBlocks-1);
        }

        currentlySelectedBlock = BlockList.instance.blocks.Values.ElementAt(selectedBlockIndex);
    }

    private void LateUpdate() {
        SelectionQuadParent.gameObject.SetActive(false);
        // show block cursor is hovering over
        RaycastHit hit;
        if(Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out hit, range, layerMask)) {
            SelectionQuadParent.gameObject.SetActive(true);
            Vector3 position = hit.point + (hit.normal * -0.5f);
            if (position.Floor() + (hit.normal * 0.51f) == lookingAtFacePos) return;
            lookingAtFacePos = position.Floor() + (hit.normal * 0.51f);
            lookingAtPos = position.FloorToInt();
            lookingAt = World.instance.GetBlock(lookingAtPos.x,lookingAtPos.y,lookingAtPos.z);
            SelectionQuadParent.position = position.Floor() + (hit.normal * 0.51f);
            SelectionQuad.rotation = Quaternion.LookRotation(-hit.normal);
        }
    }

    void BreakBlock () {
        RaycastHit hit;
        if(Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out hit, range, layerMask)) {
            ReplaceBlock(hit, BlockList.instance.blocks["Air"], true);
        }
    }

    void PlaceBlock () {
        RaycastHit hit;
        if(Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out hit, range, layerMask)) {
            AddBlock(hit, currentlySelectedBlock, true);
        }
    }

    void ReplaceBlock (RaycastHit hit, Block block, bool PlaySound) {
        Vector3 pos = hit.point;
        pos += (hit.normal*-0.5f);
        Vector3Int pos_i = pos.FloorToInt();
        if (PlaySound) {
            Block _b = World.instance.GetBlock(pos_i.x,pos_i.y,pos_i.z);
            AudioManager.instance.PlayMaterialSound(_b.soundType, pos_i);
        }
        SetBlock(pos_i.x,pos_i.y,pos_i.z,block);  
    }

    void AddBlock (RaycastHit hit, Block block, bool PlaySound) {
        Vector3 pos = hit.point;
        pos += (hit.normal*0.5f);
        Vector3Int pos_i = pos.FloorToInt();
        if (PlaySound) {
            AudioManager.instance.PlayMaterialSound(block.soundType, pos_i);
        }
        SetBlock(pos_i.x,pos_i.y,pos_i.z,block);
    }

    //TODO: if block is on a chunk border, update adjacent chunks as needed
    void SetBlock (int x, int y, int z, Block block) {
        World.instance.SetBlock(x,y,z,block);
        World.instance.UpdateChunk(x,y,z,true);
    }
}
