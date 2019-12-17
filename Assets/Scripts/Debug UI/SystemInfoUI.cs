using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class SystemInfoUI : MonoBehaviour
{
    public TextMeshProUGUI displayInfo;
    public TextMeshProUGUI gpuInfo;
    public TextMeshProUGUI ramInfo;
    public TextMeshProUGUI cpuInfo;
    public TextMeshProUGUI osInfo;

    private void OnEnable() {
        Init();
    }

    public void Init() {
        Resolution res = Screen.currentResolution;

        displayInfo.text = $"Display: {res.width} x {res.height} {res.refreshRate}Hz";

        gpuInfo.text = $"GPU: {SystemInfo.graphicsDeviceName} {SystemInfo.graphicsMemorySize} MB";

        ramInfo.text = $"RAM: {Profiler.GetTotalAllocatedMemoryLong() / 1000000} MB / {SystemInfo.systemMemorySize} MB";

        cpuInfo.text = $"CPU: {SystemInfo.processorType} x {SystemInfo.processorCount}";

        osInfo.text = $"OS: {SystemInfo.operatingSystem}";
    }

    private void Update() {
        cpuInfo.text = $"CPU: {SystemInfo.processorType} x {SystemInfo.processorCount}";
    }
}
