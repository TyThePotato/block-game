using System;
using System.Linq;
using UnityEngine;
using TMPro;
using QFSW.QC;

public class DayCycle : MonoBehaviour
{
    public static DayCycle instance;

    public int DayLengthInSeconds = 60;
    public float Hours = 0.0f;

    public float StartingTime = 8f; // what time in hours the world starts at
    public float SkyboxSpeed;

    public int UpdateRate = 1; // this minus 1 is how many frames to wait before updating time

    public Light Sun;
    public Vector3 SunRotationVector;
    public AnimationCurve SkyboxBlend;
    public AnimationCurve LightCurve;
	
	[ColorUsage(false, true)]
	public Color HighAmbientColor;
	[ColorUsage(false, true)]
	public Color LowAmbientColor;

    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DayText;

    Material skybox;
    private int frameCount = 0;
    private float[] deltaTimes;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        skybox = RenderSettings.skybox;
        Hours = StartingTime;
        deltaTimes = new float[UpdateRate];
    }

    private void Update() {
        if (++frameCount < UpdateRate) {
            deltaTimes[frameCount - 1] = Time.deltaTime;
            return;
        }
        deltaTimes[frameCount - 1] = Time.deltaTime;
        frameCount = 0;
        float totalDeltaTime = deltaTimes.Sum();

        Hours += totalDeltaTime * (24f / DayLengthInSeconds);

        skybox.SetFloat("_Rotation", (Time.time * SkyboxSpeed * UpdateRate) % 360); // Rotate skybox over time
        Sun.transform.eulerAngles = SunRotationVector * ((((Hours % 24) / 24) - 0.25f) * 360); // Rotate sun depending on time of day
        skybox.SetFloat("_SkyBlend", 1 - SkyboxBlend.Evaluate((Hours % 24) / 24)); // Blend skybox depending on time of day, inverts the number because i feel like it

        float intensity = LightCurve.Evaluate((Hours % 24) / 24);
        Sun.intensity = intensity; // Change sun brightness depending on time of day
        //RenderSettings.ambientIntensity = intensity; // change ambient intensity too, so nights are dark and days are bright
		RenderSettings.ambientLight = Color.Lerp(LowAmbientColor, HighAmbientColor, intensity);
		

        TimeText.SetText(Get12H());
        DayText.SetText("Day " + GetDay(true));
    }

    public int GetDay(bool startFromOne = false) {
        return startFromOne ? (int)(Hours / 24) + 1 : (int)(Hours / 24); // if startFromOne is true,return day+1, else return day
    }

    public int GetMinute() {
        return (int)(Hours * 60);
    }

    public int GetSecond() {
        return (int)(Hours * 3600);
    }

    public string Get24H() {
        string hour = ((int)Hours % 24).ToString();
        int _m = GetMinute() % 60;

        string minute = _m > 9 ? _m.ToString() : "0" + _m;

        return hour + ":" + minute;
    }
	
	public string Get12H() {
		string period = (int)(Hours % 24) >= 12 ? " PM" : " AM";
		string hour = ((int)(Hours % 12) == 0 ? 12 : (int)(Hours % 12)).ToString();
		int _m = GetMinute() % 60;

        string minute = _m > 9 ? _m.ToString() : "0" + _m;
		
		return hour + ":" + minute + period;
	}

    [Command("settime")]
    public void SetTime(float hours, bool add) {
        if (add) {
            Hours += hours;
        } else {
            Hours = hours;
        }
    }
}