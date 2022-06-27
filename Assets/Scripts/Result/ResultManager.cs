using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Color[] difColorArr;
    [SerializeField] private string[] difNameArr;
    [SerializeField] private TMP_Text dif;

    [SerializeField] private TMP_Text[] textArr;
    [SerializeField] private Image[] imageArr;

    [SerializeField] private TMP_Text songName;
    [SerializeField] private TMP_Text artist;

    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text bestScore;
    [SerializeField] private TMP_Text perfect;
    [SerializeField] private TMP_Text good;
    [SerializeField] private TMP_Text miss;
    [SerializeField] private TMP_Text combo;
    [SerializeField] private TMP_Text accuracy;

    [SerializeField] private Image rank;
    [SerializeField] private Sprite[] rankSprite;


    //결과로 표시되기 위해 별도의 계산이 필요한 값들
    private float acc;
    private float missCount;
    private string bestScoreValue;
    private int rankValue;

    //csv파일 위치
    private int selectedNum;
    private int scoreData;
    private int accuracyData;
    private int comboData;


    // Start is called before the first frame update
    void Start()
    {
        selectedNum = SongListManager.selectedSongNum;

        acc = ((100f * GameInfo.S.sPerfect) + (80f * GameInfo.S.perfect) + (50f * GameInfo.S.good)) / Level.S.noteCount;
        missCount = Level.S.noteCount - GameInfo.S.perfect - GameInfo.S.sPerfect - GameInfo.S.good;
        if (SongListManager.songList[selectedNum][SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty).ToString() + "_SCORE")] != "")
        {
            bestScoreValue = SongListManager.songList[selectedNum][SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty).ToString() + "_SCORE")];
        }
        else
        {
            bestScoreValue = "-,---,---";
        }
        bestScore.text = bestScoreValue;

        scoreData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + "_SCORE");
        accuracyData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + SONG_LIST_KEY._RATE);
        comboData = SONG_LIST_KEY.FindValue(DIF.FindName(Level.S.levelDifficulty) + SONG_LIST_KEY._COMBO);

        rankValue = RANK.SetRank(Convert.ToInt32(GameInfo.S.score));

        BackgroundManager.S.SetBgColor(new Color(0.4f, 0.4f, 0.4f, 1));

        StartCoroutine(ShowResult());

        if(rankValue != RANK.F)
        {
            SaveResult();
        }
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

    private IEnumerator ShowResult()
    {
        dif.text = difNameArr[Level.S.levelDifficulty];
        dif.color = difColorArr[Level.S.levelDifficulty];

        songName.text = SongListManager.songList[selectedNum][SONG_LIST_KEY.SONG_NAME];
        artist.text = "Artist. " + SongListManager.songList[selectedNum][SONG_LIST_KEY.ARTIST];

        rank.sprite = rankSprite[rankValue];

        //rank.sprite = 

        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / 2f;

            for(int i = 0; i < textArr.Length; ++i)
            {
                textArr[i].color = Utility.SetColorAlpha(textArr[i].color, t);
            }

            for(int i = 0; i < imageArr.Length; ++i)
            {
                imageArr[i].color = Utility.SetColorAlpha(imageArr[i].color, t);
            }

            score.text = Mathf.Lerp(0, GameInfo.S.score, t * t).ToString("f0");
            combo.text = Mathf.Lerp(0, GameInfo.S.maxCombo, t * t).ToString("f0");
            accuracy.text = Mathf.Lerp(0, acc, t * t).ToString("f2") + "%";

            perfect.text = Mathf.Lerp(0, GameInfo.S.perfect + GameInfo.S.sPerfect, t * t).ToString("f0");


            yield return null;
        }

        t = 0;
        while(t <= 1)
        {
            t += Time.deltaTime / 0.7f;

            good.text = Mathf.Lerp(0, GameInfo.S.good, t * t).ToString("f0");

            yield return null;
        }

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / 0.7f;

            miss.text = Mathf.Lerp(0, missCount, t * t).ToString("f0");

            yield return null;
        }
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
