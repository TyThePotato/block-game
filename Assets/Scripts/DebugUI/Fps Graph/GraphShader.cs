using UnityEngine;
using UnityEngine.UI;
public class GraphShader
{
    public Image image = null;

    public const int c_arrayMaxSizeFull = 512;
    public const int c_arrayMaxSizeLight = 128;

    public int arrayMaxSize = 128;

    public float[] array;

    private const string _name = "GraphValues";
    private const string _nameLength = "GraphValues_Length";

    public float average = 0;
    private int _averagePropertyId;

    public Color graphColor = Color.white;

    private int _colorPropertyId;

    public void InitializeShader() {
        image.material.SetFloatArray(_name, new float[arrayMaxSize]);

        _averagePropertyId = Shader.PropertyToID("Average");

        _colorPropertyId = Shader.PropertyToID("_GoodColor");
    }

    public void UpdateArray() {
        image.material.SetInt(_nameLength, array.Length);
    }

    public void UpdateAverage() {
        image.material.SetFloat(_averagePropertyId, average);
    }

    public void UpdateColors() {
        image.material.SetColor(_colorPropertyId, graphColor);
    }

    public void UpdatePoints() {
        image.material.SetFloatArray(_name, array);
    }
}