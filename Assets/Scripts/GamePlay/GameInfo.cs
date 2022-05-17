using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameInfo : MonoBehaviour
{
    public static GameInfo S = null;

    private const int perfectScore = 1000000;
    private const int goodScore = 500000;

    public float score = 0;
    [SerializeField] private TMP_Text scoreText;

    public int combo = 0;
    [SerializeField] private TMP_Text comboText;

    public int maxCombo = 0;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(this.gameObject);
            return;
        }

        S = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ClearNote(int judg)
    {
        switch(judg)
        {
            case JUDG.PERFECT:
                score += perfectScore / Level.S.noteCount;

                combo += 1;
                if (combo > maxCombo)
                    maxCombo = combo;
                break;

            case JUDG.GOOD:
                score += goodScore / Level.S.noteCount;

                combo += 1;
                if (combo > maxCombo)
                    maxCombo = combo;
                break;

            case JUDG.MISS:
                combo = 0;
                break;
        }

        scoreText.text = Convert.ToString((int)score);
        comboText.text = combo.ToString();
    }
}
