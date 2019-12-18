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
        // if screenshots folder doesnt exist, make it
        string documents = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string screenshotsFolder = documents + @"\TyJupiter\Block Game\Screenshots\";
        System.IO.Directory.CreateDirectory(screenshotsFolder);

        DateTime date = DateTime.Now;
        Debug.Log(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
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
        Debug.Log($"Saved screenshot to {screenshotsFolder + filename+extension}");
        // todo: show message on screen informing user that screenshot has successfully been created
    }

    // im not entirely sure when or why this got here but i'll leave it here for now
    void SetMaterialProperty (string property, float value) {
        TerrainMaterial.SetFloat(property, value);
    }
}
