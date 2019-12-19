using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class DayCycle : MonoBehaviour
{
    public int DayLengthInSeconds = 60;
    public float Hours = 0.0f;

    public float StartingTime = 8f; // what time in hours the world starts at
    public float SkyboxSpeed;

    public Light Sun;
    public Vector3 SunRotationVector;
    public AnimationCurve SkyboxBlend;
    public AnimationCurve LightCurve;

    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DayText;

    Material skybox;

    private void Awake() {
        skybox = RenderSettings.skybox;
        Hours = StartingTime;
    }

    private void Update() {
        Hours += (Time.deltaTime * (24f / DayLengthInSeconds));

        skybox.SetFloat("_Rotation", (Time.time * SkyboxSpeed) % 360); // Rotate skybox over time
        skybox.SetFloat("_SkyBlend", 1-SkyboxBlend.Evaluate(Hours/24)); // Blend skybox depending on time of day, inverts the number because i feel like it
        Sun.intensity = LightCurve.Evaluate(Hours/24); // Change sun brightness depending on time of day
        Sun.transform.eulerAngles = SunRotationVector * (((Hours/24)-0.25f)*360); // Rotate sun depending on time of day

        TimeText.SetText(Get24H());
        DayText.SetText("Day " + GetDay(true));
    }

    public int GetDay (bool startFromOne = false) {
        return startFromOne ? (int)(Hours / 24) + 1 : (int)(Hours / 24); // if startFromOne is true,return day+1, else return day
    }

    public int GetMinute () {
        return (int)(Hours * 60);
    }

    public int GetSecond () {
        return (int)(Hours * 3600);
    }

    public string Get24H () {
        string hour = ((int)Hours % 24).ToString();
        int _m = GetMinute() % 60;

        string minute;
        if(_m > 9) {
            minute = _m.ToString();
        } else {
            minute = "0" + _m;
        }

        return hour + ":" + minute;
    }
}
