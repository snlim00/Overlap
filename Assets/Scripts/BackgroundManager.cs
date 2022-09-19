using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager S = null;

    private SpriteRenderer spr;

    private Vector2 defaultScale;

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

        transform.localScale = new Vector2(scale, scale);

        defaultScale = transform.localScale;
    }

    public void SetBgColor(Color color)
    {
        spr.color = color;
    }

    public void SetBgAlpha(float a)
    {
        spr.color = Utility.SetColorAlpha(spr.color, a);
    }

    public void SetBgScale(float scale)
    {
        scale *= 0.01f;

        transform.localScale = defaultScale * scale;
    }
}
