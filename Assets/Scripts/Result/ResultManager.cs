using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
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

    private float acc;
    private int selectedNum;
    private int scoreData;
    private int accuracyData;
    private int comboData;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        acc = ((100f * GameInfo.S.sPerfect) + (80f * GameInfo.S.perfect) + (50f * GameInfo.S.good)) / Level.S.noteCount;
        selectedNum = SongListManager.selectedSongNum;
        scoreData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + "_SCORE");
        accuracyData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + SONG_LIST_KEY._RATE);
        comboData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + SONG_LIST_KEY._COMBO);

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
        if (SongListManager.songList[selectedNum][scoreData] == "")
            SongListManager.songList[selectedNum][scoreData] = "0";

        if (GameInfo.S.score <= Convert.ToInt32(SongListManager.songList[selectedNum][scoreData]))
            return false;

        SongListManager.songList[selectedNum][scoreData] = Math.Ceiling(GameInfo.S.score).ToString();
        SongListManager.songList[selectedNum][accuracyData] = acc.ToString("f2");
        SongListManager.songList[selectedNum][comboData] = GameInfo.S.combo.ToString();

        Debug.Log(Math.Ceiling(GameInfo.S.score).ToString() + " / " + acc.ToString("f2") + " / " + GameInfo.S.combo.ToString());

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
        audioSource.Play();

        int missCount = Level.S.noteCount - GameInfo.S.perfect - GameInfo.S.good;

        yield return new WaitForSeconds(1f);

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / 1;

            float alpha = Mathf.Lerp(0, 1, t);

            score.color = Utility.SetColorAlpha(score.color, alpha);
            combo.color = Utility.SetColorAlpha(combo.color, alpha);
            accuracy.color = Utility.SetColorAlpha(accuracy.color, alpha);

            perfect.color = Utility.SetColorAlpha(perfect.color, alpha);
            good.color = Utility.SetColorAlpha(good.color, alpha);
            miss.color = Utility.SetColorAlpha(miss.color, alpha);

            for (int i = 0; i < textArr.Length; ++i)
            {
                textArr[i].color = Utility.SetColorAlpha(textArr[i].color, alpha);
            }

            score.text = Mathf.Lerp(0, GameInfo.S.score, t * t).ToString("f0");
            perfect.text = Mathf.Lerp(0, GameInfo.S.perfect + GameInfo.S.sPerfect, t * t).ToString("f0");
            good.text = Mathf.Lerp(0, GameInfo.S.good, t * t).ToString("f0");
            miss.text = Mathf.Lerp(0, missCount, t * t).ToString("f0");
            combo.text = Mathf.Lerp(0, GameInfo.S.maxCombo, t * t).ToString("f0");
            accuracy.text = Mathf.Lerp(0, acc, t * t).ToString("f2") + "%";

            yield return null;
        }

        Destroy(FindObjectOfType<GameInfo>().gameObject);
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
