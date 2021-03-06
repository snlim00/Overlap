using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameInfo : MonoBehaviour
{
    public static GameInfo S = null;

    private const float perfectScore = 1000000;
    private const float goodScore = 500000;

    public float score = 0;
    [SerializeField] private TMP_Text scoreText;

    public int combo = 0;
    [SerializeField] private TMP_Text comboText;

    public int sPerfect = 0;
    public int perfect = 0;
    public int good = 0;

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
            case JUDG.S_PERFECT:
                score += perfectScore / Level.S.noteCount;

                combo += 1;
                sPerfect += 1;

                if (combo > maxCombo)
                    maxCombo = combo;
                break;

            case JUDG.PERFECT:
                score += perfectScore / Level.S.noteCount;

                combo += 1;
                perfect += 1;

                if (combo > maxCombo)
                    maxCombo = combo;
                break;

            case JUDG.GOOD:
                score += goodScore / Level.S.noteCount;

                combo += 1;
                good += 1;

                if (combo > maxCombo)
                    maxCombo = combo;
                break;

            case JUDG.MISS:
                combo = 0;
                break;
        }

        scoreText.text = String.Format("{0:#,###}", Math.Ceiling(score));
        comboText.text = combo.ToString();
    }
}
