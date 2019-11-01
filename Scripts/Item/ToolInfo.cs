using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInfo : MonoBehaviour
{
    public Behaviour[] extraComponents;
    public ParticleSystem particles;

    public void Enable () {
        if (extraComponents != null) {
            for (int i = 0; i < extraComponents.Length; i++) {
                extraComponents[i].enabled = true;
            }
        }

        if (particles != null) {
            particles.Play();
            Debug.Log("Play");
        }
    }

    public void Disable () {
        if (extraComponents != null) {
            for (int i = 0; i < extraComponents.Length; i++) {
                extraComponents[i].enabled = false;
            }
        }

        if (particles != null)
            particles.Stop();
    }
}
