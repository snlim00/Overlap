using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public bool editerMode = true;

    public static PlayerSetting S = null;
    void Awake()
    {
        if(S != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        PlayerSetting.S = this;
    }

    public float songOffset = -0.05f;
    public float noteOffset = 0.15f;
}
