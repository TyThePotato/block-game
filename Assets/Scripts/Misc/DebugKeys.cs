using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using QFSW.QC;

public class DebugKeys : MonoBehaviour
{
    public GameObject GameUI;
    public Material TerrainMaterial;

    private bool screenshotCaptured = false;
    private string screenshotInfo;

    private void Update() {
        if(screenshotCaptured) {
            Debug.Log("Saved screenshot to " + screenshotInfo);
            screenshotCaptured = false;
        }

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
        string screenshotsFolder = Helper.GameFolder(@"Screenshots\");

        DateTime date = DateTime.Now;
        string filename = date.ToString("MMM d yyyy H mm");
        string extension = ".png";

        // in case multiple screenshots are taken in the same second, check if a screenshot with the desired name exists and if so, append a number to the end and repeat checks until no file exists with the name
        int ssnum = 0;
        for (int i = 0; i < int.MaxValue; i++) {
            if (System.IO.File.Exists(screenshotsFolder+filename+extension)) {
                ssnum++;
                extension = $" {ssnum}.png";
            } else {
                break;
            }
        }
        ScreenCapture.CaptureScreenshot(screenshotsFolder + filename+extension);
        screenshotCaptured = true;
        screenshotInfo = screenshotsFolder + filename + extension;
        // todo: show message on screen informing user that screenshot has successfully been created
    }

    // im not entirely sure when or why this got here but i'll leave it here for now
    void SetMaterialProperty (string property, float value) {
        TerrainMaterial.SetFloat(property, value);
    }
}
