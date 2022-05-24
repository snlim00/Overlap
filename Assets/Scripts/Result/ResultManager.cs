using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Image cover;
    private Color black = new Color(0.2f, 0.2f, 0.2f, 0.7f);
    private Color none = new Color(0.2f, 0.2f, 0.2f, 0);

    private AudioSource audioSource;

    [SerializeField] private TMP_Text[] textArr;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text perfect;
    [SerializeField] private TMP_Text good;
    [SerializeField] private TMP_Text miss;
    [SerializeField] private TMP_Text combo;
    [SerializeField] private TMP_Text accuracy;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowResult();

        SaveResult();
    }

    private void ShowResult()
    {
        for (int i = 0; i < textArr.Length; ++i)
        {
            textArr[i].color = new Color(textArr[i].color.r, textArr[i].color.g, textArr[i].color.b, 0);
        }

        Color color = new Color(1, 1, 1, 0);

        score.color = color;
        perfect.color = color;
        good.color = color;
        miss.color = color;
        combo.color = color;
        accuracy.color = color;

        StartCoroutine(BGFadeOut());
    }
    
    private bool SaveResult()
    {
        int selectedNum = SongListManager.selectedNum;
        int dif = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + "_SCORE");

        if (SongListManager.songList[selectedNum][dif] == "")
            SongListManager.songList[selectedNum][dif] = "0";

        if (GameInfo.S.score <= Convert.ToInt32(SongListManager.songList[selectedNum][dif]))
            return false;

        SongListManager.songList[selectedNum][dif] = Math.Ceiling(GameInfo.S.score).ToString();
        WriteUserData();

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BGFadeOut()
    {
        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / 1;

            cover.color = Color.Lerp(none, black, t);

            yield return null;
        }

        audioSource.Play();

        int missCount = Level.S.noteCount - GameInfo.S.perfect - GameInfo.S.good;
        float acc = ((100f * GameInfo.S.perfect) + (50f * GameInfo.S.good)) / Level.S.noteCount;

        yield return new WaitForSeconds(1f);

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / 1;

            cover.color = Color.Lerp(black, none, t);

            float alpha = Mathf.Lerp(0, 1, t);

            score.color = SetColorAlpha(score.color, alpha);
            combo.color = SetColorAlpha(combo.color, alpha);
            accuracy.color = SetColorAlpha(accuracy.color, alpha);

            perfect.color = SetColorAlpha(perfect.color, alpha);
            good.color = SetColorAlpha(good.color, alpha);
            miss.color = SetColorAlpha(miss.color, alpha);

            for (int i = 0; i < textArr.Length; ++i)
            {
                textArr[i].color = SetColorAlpha(textArr[i].color, alpha);
            }

            score.text = Mathf.Lerp(0, GameInfo.S.score, t * t).ToString("f0");
            perfect.text = Mathf.Lerp(0, GameInfo.S.perfect, t * t).ToString("f0");
            good.text = Mathf.Lerp(0, GameInfo.S.good, t * t).ToString("f0");
            miss.text = Mathf.Lerp(0, missCount, t * t).ToString("f0");
            combo.text = Mathf.Lerp(0, GameInfo.S.maxCombo, t * t).ToString("f0");
            accuracy.text = Mathf.Lerp(0, acc, t * t).ToString("f2") + "%";

            yield return null;
        }

        Destroy(FindObjectOfType<GameInfo>().gameObject);
    }

    private Color SetColorAlpha(Color color, float a)
    {
        Color rc = color;
        rc.a = a;

        return rc;
    }

    public void WriteUserData()
    {
        string a = "Assets/Levels/Resources/SongList.csv";
        using (var writer = new CsvFileWriter(a))
        {
            List<string> colums = new List<string>();
            string[] keyList = new string[SONG_LIST_KEY.COUNT];
            for (int i = 0; i < SONG_LIST_KEY.COUNT; ++i)
            {
                keyList[i] = SONG_LIST_KEY.FindName(i);
            }

            colums.AddRange(keyList);

            writer.WriteRow(colums);
            colums.Clear();

            for (int i = 0; i < SongListManager.songList.Count; ++i)
            {
                for (int j = 0; j < SONG_LIST_KEY.COUNT; ++j)
                {
                    colums.Add(SongListManager.songList[i][j].ToString());
                }

                writer.WriteRow(colums);
                colums.Clear();

                //Debug.Log("Write");
            }
        }
    }
}
