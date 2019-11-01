using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateIcons : MonoBehaviour
{
    public Camera iconCam;

    public Texture2D GenerateIcon (GameObject go, float cameraSize) {
        go.transform.position = Vector3.zero;
        iconCam.gameObject.SetActive(true);
        Texture2D tex = CaptureIcon(go.name, cameraSize);
        go.transform.position = new Vector3(-5,-5,-5);
        go.SetActive(false);
        iconCam.gameObject.SetActive(false);
        return tex;
    }

    void CaptureIconToIconCache (string TextureName) {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = iconCam.targetTexture;
        GL.Clear(true, true, Color.clear);
        iconCam.Render();
        Texture2D img = new Texture2D(iconCam.targetTexture.width, iconCam.targetTexture.height);
        img.ReadPixels(new Rect(0, 0, iconCam.targetTexture.width, iconCam.targetTexture.height), 0, 0);
        img.Apply();
        RenderTexture.active = rt;
        img.name = TextureName;
        IconCache.icons.Add(TextureName, img);
    }

    Texture2D CaptureIcon (string TextureName, float cameraSize = 1.0f) {
        iconCam.orthographicSize = cameraSize;
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = iconCam.targetTexture;
        GL.Clear(true, true, Color.clear);
        iconCam.Render();
        Texture2D img = new Texture2D(iconCam.targetTexture.width, iconCam.targetTexture.height);
        img.ReadPixels(new Rect(0, 0, iconCam.targetTexture.width, iconCam.targetTexture.height), 0, 0);
        img.Apply();
        RenderTexture.active = rt;
        img.name = TextureName;
        return img;
    }
}
