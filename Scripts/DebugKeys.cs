using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugKeys : MonoBehaviour
{
    public GameObject GameUI;

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
        string filename = date.ToString("MMM d yyyy H mm");
        string extension = $" {ssnum}.png";
        for (int i = 0; i < int.MaxValue; i++) {
            if (System.IO.File.Exists(filename+extension)) {
                ssnum++;
                extension = $" {ssnum}.png";
            } else {
                break;
            }
        }
        ScreenCapture.CaptureScreenshot(filename+extension);
        Debug.Log($"Saved screenshot to {filename+extension}");
    }
}
