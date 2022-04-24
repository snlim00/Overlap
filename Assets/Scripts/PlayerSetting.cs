using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public bool editerMode = true;

    public static PlayerSetting S;
    void Awake()
    {
        PlayerSetting.S = this;
    }

    public float offset = -0.05f;
}
