using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class SongListManagerRemake : MonoBehaviour
{
    public static List<Dictionary<int, string>> songList = new List<Dictionary<int, string>>();

    private Camera mainCam;

    [SerializeField] private WaveEffect waveEffect;

    #region 레벨 플레이 관련 변수
    [SerializeField] private TMP_Text songName;
    [SerializeField] private TMP_Text producer;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text rate;
    [SerializeField] private TMP_Text combo;
    #endregion

    #region 곡 선택 관련 변수
    private int selectedDif = DIF.E;
    private int selectedSongNum = 0;
    #endregion

    //BackGroundManager보다 느리게 초기화 되어야 함. (Select 때문에)
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        mainCam = Camera.main;

        ReadSongList();

        SelectSong(0);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSong();
    }

    private void ReadSongList()
    {
        songList.Clear();

        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDicString(temp, ref songList, SONG_LIST_KEY.FindName);
    }

    #region 버튼
    public void LevelPlay()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
    }

    public void SelectDif()
    {
        ButtonEffect btn = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonEffect>();

        selectedDif = DIF.FindValue(btn.tag);

        StartCoroutine(btn.PressBtn());

        SpawnCircle(selectedDif);
    }
    #endregion

    private void SpawnCircle(int dif)
    {
        StartCoroutine(waveEffect.SpawnCircle(dif));
    }

    #region 곡 선택
    private void ChangeSong()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedSongNum -= 1;

            if(selectedSongNum < 0)
            {
                selectedSongNum = 0;
            }

            SelectSong(selectedSongNum);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedSongNum += 1;

            if (selectedSongNum > songList.Count - 1)
            {
                selectedSongNum = songList.Count - 1;
            }

            SelectSong(selectedSongNum);
        }
    }

    private void SelectSong(int num)
    {
        string levelName = Level.RemoveSapce(songList[num][SONG_LIST_KEY.SONG_NAME]);

        StartCoroutine(ChangeBackground(num, levelName));

        SpawnCircle(selectedDif);
    }

    private float changeDuration = 0.15f;
    private IEnumerator ChangeBackground(int num, string levelName)
    {
        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / changeDuration;

            BackgroundManager.S.spr.color = Utility.SetColorAlpha(BackgroundManager.S.spr.color, 1 - Utility.LerpValue(t, 1));

            yield return null;
        }

        BackgroundManager.S.SetBgImage(levelName);
        songName.text = songList[num][SONG_LIST_KEY.SONG_NAME];
        producer.text = "Artist. " + songList[num][SONG_LIST_KEY.ARTIST];

        string scoreData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_SCORE")];
        if(scoreData == "")
        {
            score.text = "Score : -,---,---";
        }
        else
        {
            score.text = "Score : " + String.Format("{0:#,###}", Convert.ToInt32(songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_SCORE")]));
        }

        string rateData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_RATE")];
        if (rateData == "")
        {
            rate.text = "Rate : --.--%";
        }
        else
        {
            rate.text = "Rate : " + songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_RATE")] + "%";
        }

        string comboData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_COMBO")];
        if (comboData == "")
        {
            combo.text = "Combo : ----";
        }
        else
        {
            combo.text = "Combo : " + songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + "_COMBO")];
        }
        t = 0;
        while (t <= 1)

        {
            t += Time.deltaTime / changeDuration;

            BackgroundManager.S.spr.color = Utility.SetColorAlpha(BackgroundManager.S.spr.color, Utility.LerpValue(t, 1));

            yield return null;
        }
    }
    #endregion
}
