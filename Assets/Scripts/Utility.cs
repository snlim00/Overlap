using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static float LerpValue(float t, int type)
    {
        float p = 0;

        switch (type)
        {
            case 0:
                p = t;
                break;

            case 1:
                p = t * t;
                break;

            case 2:
                p = -((2 * t - 1) * (2 * t - 1)) + 1;
                break;
        }

        return p;
    }
}
