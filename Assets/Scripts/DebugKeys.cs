using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using QFSW.QC;

public class DebugKeys : MonoBehaviour
{
    public GameObject GameUI;
    public Material TerrainMaterial;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            ToggleUI();
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            Screenshot();
        }
    }

    void ToggleUI () {
        GameUI.SetActive(!GameUI.activeSelf);
    }
    

    void Screenshot () {
        DateTime date = DateTime.Now;
        int ssnum = 0;
        string folder = "Screenshots/";
        string filename = date.ToString("MMM d yyyy H mm");
        string extension = $" {ssnum}.png";
        for (int i = 0; i < int.MaxValue; i++) {
            if (System.IO.File.Exists(folder+filename+extension)) {
                ssnum++;
                extension = $" {ssnum}.png";
            } else {
                break;
            }
        }
        ScreenCapture.CaptureScreenshot(folder+filename+extension);
        Debug.Log($"Saved screenshot to {folder+filename+extension}");
    }

    void SetMaterialProperty (string property, float value) {
        TerrainMaterial.SetFloat(property, value);
    }
}
