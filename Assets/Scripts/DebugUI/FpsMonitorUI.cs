using TMPro;
using UnityEngine;

public class FpsMonitorUI : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI fpsMinText;
    public TextMeshProUGUI fpsMaxText;
    public TextMeshProUGUI fpsAverageText;

    public int averageFps;
    public int minFps;
    public int maxFps;
    public int fps;

    private float _avg;

    private void Update() {
        fps = (int)(1f / Time.unscaledDeltaTime);

        _avg += (Time.deltaTime / Time.timeScale - _avg) * 0.03f;

        averageFps = (int)(1f / _avg);

        if (fps < minFps || minFps == 0)
            minFps = fps;
        if (fps > maxFps || maxFps == 0)
            maxFps = fps;

        fpsText.text = $"{fps} FPS";
        fpsMinText.text = $"{minFps} min";
        fpsMaxText.text = $"{maxFps} max";
        fpsAverageText.text = $"{averageFps} avg";
    }
}