using TMPro;
using UnityEngine;

public class FpsMonitorUI : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    public int averageFps;
    public int fps;

    private float _avg;

    private void Update() {
        fps = (int)(1f / Time.unscaledDeltaTime);

        _avg += (Time.deltaTime / Time.timeScale - _avg) * 0.03f;

        averageFps = (int)(1f / _avg);

        fpsText.text = $"{fps} FPS ({averageFps} avg)";
    }
}