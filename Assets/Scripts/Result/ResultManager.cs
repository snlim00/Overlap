using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private Color[] difColorArr;
    [SerializeField] private string[] difNameArr;
    [SerializeField] private TMP_Text dif;

    [SerializeField] private TMP_Text[] textArr;
    [SerializeField] private Image[] imageArr;

    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text perfect;
    [SerializeField] private TMP_Text good;
    [SerializeField] private TMP_Text miss;
    [SerializeField] private TMP_Text combo;
    [SerializeField] private TMP_Text accuracy;


    //정확도
    private float acc;

    //csv파일 위치
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

        StartCoroutine(ShowResult());

        SaveResult();
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

        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / 3f;

            for(int i = 0; i < textArr.Length; ++i)
            {
                textArr[i].color = Utility.SetColorAlpha(textArr[i].color, t);
            }

            for(int i = 0; i < imageArr.Length; ++i)
            {
                imageArr[i].color = Utility.SetColorAlpha(imageArr[i].color, t);
            }
            

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
