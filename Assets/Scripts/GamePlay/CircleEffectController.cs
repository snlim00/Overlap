using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleEffectController : MonoBehaviour
{
    public float duration = 1;

    private SpriteRenderer spr;

    private Vector2 curSize;

    [SerializeField] private Vector2 startSize = new Vector2(0.07f, 0.07f); //나타나는 이펙트의 시작 사이즈
    public Vector2 targetSize = new Vector2(0.35f, 0.35f); //나타나는 이펙트의 시작 사이즈

    private Color32 curAlpha; //이펙트의 현재 투명도

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;

        spr = gameObject.GetComponent<SpriteRenderer>();

        transform.localScale = startSize;

        StartCoroutine(ParticleMove());
    }

    public IEnumerator ParticleMove()
    {
        float t = 0;
        float size = 1;

        curSize = transform.localScale;
        curAlpha = spr.color;
        Color targetAlpha = new Color(spr.color.r, spr.color.g, spr.color.b, 0);

        while (t <= 1)
        {
            t += Time.deltaTime / duration;

            //curSize = transform.localScale;
            size -= size * 0.97f * Time.deltaTime * 8;
            transform.localScale = Vector2.Lerp(curSize, targetSize, 1 - size);

            curAlpha = spr.color;
            spr.color = Color.Lerp(curAlpha, targetAlpha, t * 0.06f);

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
