using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.RichPresence;

public class RPTest : MonoBehaviour
{
    void Start () {
        IRichPresence rp = RichPresenceComponent.Instance.RichPresenceModule; // for ez access
        rp.SetPrimaryText("In-game");
        rp.SetPrimaryImage("main_icon");
        rp.SetPrimaryImageTooltip("look at this epic house");
    }
}
