using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    [SerializeField] private GameObject cursor;

    public bool editorMode = true;

    public static PlayerSetting S = null;

    private Camera mainCam;

    void Awake()
    {
        if(S != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(cursor);
        PlayerSetting.S = this;

        if (editorMode == false)
        {
            mainCam = Camera.main;
            Cursor.visible = false;
        }
        else
        {
            cursor.SetActive(false);
        }
    }

    private void Update()
    {
        if(editorMode == false)
        {
            cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }
    }

    public float songOffset;
    public float noteOffset;
}
