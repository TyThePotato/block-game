using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSway : MonoBehaviour
{
    public Transform Hand;
    public Transform HandTarget;
    public float SlerpAmount;

    private void Update() {
        Vector3 nextPos = Helper.Damp(Hand.position, HandTarget.position, SlerpAmount, Time.deltaTime);
        Hand.position = nextPos;
        Hand.rotation = HandTarget.rotation;
    }
}
