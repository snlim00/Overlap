using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour
{
    private WaveEffect waveEffect;

    private GameObject edge;

    private bool isPressing = false;

    // Start is called before the first frame update
    void Start()
    {
        waveEffect = FindObjectOfType<WaveEffect>();

        edge = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if(isPressing == false)
        {
            waveEffect.StartCoroutine(waveEffect.SpawnCircle(DIF.FindValue(EventSystem.current.currentSelectedGameObject.tag)));
            StartCoroutine(PressBtn());
        }
    }

    private IEnumerator PressBtn()
    {
        isPressing = true;

        float t = 0;

        Vector2 startPos = transform.localPosition;

        while (t <= 1)
        {
            t += Time.deltaTime / 0.12f;

            transform.localPosition = Vector2.Lerp(startPos, Vector2.zero, Utility.LerpValue(t, 2));

            yield return null;
        }

        isPressing = false;
    }
}
