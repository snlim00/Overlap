using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager S = null;

    private SpriteRenderer spr;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(this.gameObject);

            return;
        }

        DontDestroyOnLoad(this.gameObject);

        S = this;

        spr = GetComponent<SpriteRenderer>();
    }

    public void SetBgImage(string levelName, int num = 0)
    {
        _SetBgImage(levelName, num);
    }

    public void SetBgImage(int num)
    {
        _SetBgImage(Level.S.levelName, num);
    }

    private void _SetBgImage(string levelName, int num)
    {
        spr.sprite = Resources.Load<Sprite>(levelName + "_bg" + num);

        float scale = Screen.width / spr.sprite.rect.width;

        spr.transform.localScale = new Vector2(scale, scale);
    }
}
