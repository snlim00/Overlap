using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    [SerializeField] private Texture2D cursorImg;

    public bool editerMode = true;

    public static PlayerSetting S = null;

    void Awake()
    {
        //cursorImg.Reinitialize((int)(cursorImg.width * 0.5f), (int)(cursorImg.height * 0.5f));

        //Cursor.


        if(S != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        PlayerSetting.S = this;
    }

    private void Update()
    {
        
    }

    public float songOffset;
    public float noteOffset;
}
