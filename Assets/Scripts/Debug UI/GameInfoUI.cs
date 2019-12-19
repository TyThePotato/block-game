using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class GameInfoUI : MonoBehaviour
{
    public TextMeshProUGUI gameInfoText;

    private void OnEnable() {
        Init();
    }

    public void Init() {
        gameInfoText.text = $"{Application.productName} - Alpha v{Application.version}";
    }
}
