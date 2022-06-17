using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour
{
    private bool isPressing = false;

    public IEnumerator PressBtn()
    {
        if(isPressing == true)
        {
            yield break;
        }

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
