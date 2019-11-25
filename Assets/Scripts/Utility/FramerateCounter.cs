using TMPro;
using UnityEngine;

public class FramerateCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;
    public int avgFrameRate;

    private void Start() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Update() {
        float current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        _text.text = avgFrameRate + " FPS";
    }
}
