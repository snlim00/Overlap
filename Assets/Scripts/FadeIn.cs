using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private bool instantFade = true;

    [SerializeField] private float duration = 1;
    [SerializeField] private float startAlpha;
    [SerializeField] private Color targetColor;

    private Color startColor;

    private MaskableGraphic maskGrph;

    // Start is called before the first frame update
    void Start()
    {
        Image img;
        TMP_Text text;

        if(TryGetComponent<Image>(out img) == true)
        {
            maskGrph = GetComponent<Image>();
        }
        else if(TryGetComponent<TMP_Text>(out text))
        {
            maskGrph = GetComponent<TMP_Text>();
        }
        else
        {
            return;
        }

        startColor = maskGrph.color;
        startColor.a = startAlpha;
        maskGrph.color = startColor;

        if(instantFade == true)
        {
            StartCoroutine(SetColor());
        }
    }

    private IEnumerator SetColor()
    {
        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            maskGrph.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }
    }
}
