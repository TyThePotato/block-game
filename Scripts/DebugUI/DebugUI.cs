using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public List<GameObject> debugUiGameObjects;

    void Update() {
        if (!Input.GetKeyDown(KeyCode.F3))
            return;
        foreach (GameObject ui in debugUiGameObjects) {
            ui.SetActive(!ui.activeSelf);
        }
    }
}