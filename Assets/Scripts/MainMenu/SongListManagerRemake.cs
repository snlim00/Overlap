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

    #region �� ���� ���� ����
    [SerializeField] private TMP_Text songName;
    [SerializeField] private TMP_Text producer;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text rate;
    [SerializeField] private TMP_Text combo;

    [SerializeField] private GameObject[] arrow;

    private int selectedDif = DIF.E;
    public static int selectedSongNum = 0;
    #endregion

    //BackGroundManager���� ������ �ʱ�ȭ �Ǿ�� ��. (Select ������)
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        mainCam = Camera.main;

        ReadSongList();

        SelectSong(selectedSongNum);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSong();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private void ReadSongList()
    {
        songList.Clear();

        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDicString(temp, ref songList, SONG_LIST_KEY.FindName);
    }

    #region ��ư
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

        ShowData(selectedSongNum);
    }
    #endregion

    private void SpawnCircle(int dif)
    {
        StartCoroutine(waveEffect.SpawnCircle(dif));
    }

    #region �� ����
    private void ChangeSong()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            LeftSongSelect();
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            RightSongSelect();
        }
    }

    public void LeftSongSelect()
    {
        selectedSongNum -= 1;

        if (selectedSongNum < 0)
        {
            selectedSongNum = 0;
            return;
        }

        SelectSong(selectedSongNum);
    }

    public void RightSongSelect()
    {
        selectedSongNum += 1;

        if (selectedSongNum > songList.Count - 1)
        {
            selectedSongNum = songList.Count - 1;
            return;
        }

        SelectSong(selectedSongNum);
    }

    private void SelectSong(int num)
    {
        if (selectedSongNum <= 0)
        {
            arrow[0].SetActive(false);
            arrow[1].SetActive(true);
        }
        else if (selectedSongNum >= songList.Count - 1)
        {
            arrow[0].SetActive(true);
            arrow[1].SetActive(false);
        }
        else
        {
            arrow[0].SetActive(true);
            arrow[1].SetActive(true);
        }


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
        ShowData(num);

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / changeDuration;

            BackgroundManager.S.spr.color = Utility.SetColorAlpha(BackgroundManager.S.spr.color, Utility.LerpValue(t, 1));

            yield return null;
        }
    }

    private void ShowData(int num)
    {
        songName.text = songList[num][SONG_LIST_KEY.SONG_NAME];
        producer.text = "Artist. " + songList[num][SONG_LIST_KEY.ARTIST];

        string scoreData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._SCORE)];
        if (scoreData == "")
        {
            score.text = "Score : -,---,---";
        }
        else
        {
            score.text = "Score : " + String.Format("{0:#,###}", Convert.ToInt32(songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._SCORE)]));
        }

        string rateData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._RATE)];
        if (rateData == "")
        {
            rate.text = "Rate : --.--%";
        }
        else
        {
            rate.text = "Rate : " + songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._RATE)] + "%";
        }

        string comboData = songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._COMBO)];
        if (comboData == "")
        {
            combo.text = "Combo : ----";
        }
        else
        {
            combo.text = "Combo : " + songList[num][SONG_LIST_KEY.FindValue(DIF.FindName(selectedDif) + SONG_LIST_KEY._COMBO)];
        }
    }
    #endregion

    public void StartGame()
    {
        SceneManager.S.StartGame(songList[selectedSongNum][SONG_LIST_KEY.SONG_NAME], selectedDif);
    }
}
