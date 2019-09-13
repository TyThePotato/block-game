using UnityEngine;
using UnityEngine.UI;

public class FpsGraph : Graph
{
    public void UpdateParameters() {
        _shaderGraph.arrayMaxSize = GraphShader.c_arrayMaxSizeFull;
        _shaderGraph.image.material = new Material(_shader);

        _shaderGraph.InitializeShader();

        CreatePoints();
    }

    private void Init() {
        _fpsMonitorUi = GetComponent<FpsMonitorUI>();

        _shaderGraph = new GraphShader() {
            image = _imageGraph
        };

        UpdateParameters();
    }

    [SerializeField] private Image _imageGraph;

    [SerializeField] private Shader _shader;

    private FpsMonitorUI _fpsMonitorUi;

    private const int _resolution = 150;

    private GraphShader _shaderGraph;

    private int[] _fpsArray;

    private int _highestFps;

    public FpsGraph(Image imageGraph, Shader shader) {
        _imageGraph = imageGraph;
        _shader = shader;
    }

    private void OnEnable() {
        Init();
    }

    private void FixedUpdate() {
        UpdateGraph();
    }

    protected override void UpdateGraph() {
        int fps = (int)(1 / Time.unscaledDeltaTime);

        int currentMaxFps = 0;

        for (int i = 0; i <= _resolution - 1; i++) {
            if (i >= _resolution - 1)
                _fpsArray[i] = fps;
            else
                _fpsArray[i] = _fpsArray[i + 1];

            if (currentMaxFps < _fpsArray[i]) {
                currentMaxFps = _fpsArray[i];
            }
        }

        _highestFps = _highestFps < 1 || _highestFps <= currentMaxFps ? currentMaxFps : _highestFps - 1;

        for (int i = 0; i <= _resolution - 1; i++)
            _shaderGraph.array[i] = _fpsArray[i] / (float)_highestFps;
        _shaderGraph.average = _fpsMonitorUi.averageFps;

        _shaderGraph.UpdatePoints();
        _shaderGraph.UpdateAverage();
    }

    protected override void CreatePoints() {
        _shaderGraph.array = new float[_resolution];

        _fpsArray = new int[_resolution];

        for (int i = 0; i < _resolution; i++) {
            _shaderGraph.array[i] = 0;
        }

        _shaderGraph.UpdateColors();
        _shaderGraph.UpdateArray();
    }
}
