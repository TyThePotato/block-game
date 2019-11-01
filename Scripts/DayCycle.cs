using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayCycle : MonoBehaviour
{
   Material skybox;
   public float TimeOfDay;
   public float StartingSeconds = 200; // starts at 8am
   public float DayLength = 600; // 600 seconds
   public float RotationSpeed;
   
   public Light Sun;
   public Vector3 SunRotationVector;
   public AnimationCurve SkyboxBlend;
   public AnimationCurve LightCurve;

   public TextMeshProUGUI TimeText;

   private void Awake() {
       skybox = RenderSettings.skybox;
   }

    private void Update() {
        TimeOfDay = ((((Time.time+StartingSeconds) % DayLength) / DayLength) * 24);
        skybox.SetFloat("_Rotation", (Time.time * RotationSpeed) % 360); // Rotate skybox over time
        skybox.SetFloat("_SkyBlend", 1-SkyboxBlend.Evaluate(TimeOfDay/24)); // Blend skybox depending on time of day, inverts the number because i feel like it
        Sun.intensity = LightCurve.Evaluate(TimeOfDay/24); // Change sun brightness depending on time of day
        Sun.transform.eulerAngles = SunRotationVector * (((TimeOfDay/24)-0.25f)*360); // Rotate sun depending on time of day

        TimeText.SetText(Get24HrTime());
    }

    // BROKEN
    private string Get12HrTime () {
        int h = Mathf.FloorToInt(TimeOfDay);
        int m = Mathf.FloorToInt((TimeOfDay * 60)%60);
        string ampm;
        if(h>=12) {
            ampm = "pm";
        } else {
            ampm = "am";
        }
        string h_fixed;
        if(h>12) {
            h_fixed = (h-12).ToString();
        } else if(h==0) {
            h_fixed = 12.ToString();
        } else {
            h_fixed = h.ToString();
        }
        string m_fixed;
        if(m>9) {
            m_fixed = m.ToString();
        } else {
            m_fixed = "0" + m;
        }
        return $"{h_fixed}:{m_fixed} {ampm}";
    }

    private string Get24HrTime () {
        int h = Mathf.FloorToInt(TimeOfDay);
        int m = Mathf.FloorToInt((TimeOfDay*60)%60);
        string m_fixed;
        if(m>9) {
            m_fixed = m.ToString();
        } else {
            m_fixed = "0" + m;
        }
        return $"{h}:{m_fixed}";
    }
}
