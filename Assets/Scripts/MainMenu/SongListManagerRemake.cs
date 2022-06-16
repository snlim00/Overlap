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

    #region 레벨 플레이 관련 변수
    [SerializeField] private Button[] difBtn;
    [SerializeField] private TMP_Text[] bestScore;
    #endregion

    //BackGroundManager보다 느리게 초기화 되어야 함. (Select 때문에)
    private void Start()
    {

        //interval = (Screen.height * interval) / 634;
        //textSize = (Screen.height * textSize) / 634;

        Init();
    }

    public void Init()
    {
        mainCam = Camera.main;

        ReadSongList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ReadSongList()
    {
        songList.Clear();

        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDicString(temp, ref songList, SONG_LIST_KEY.FindName);
    }

    #region 레벨 플레이
    public void OnButtonClick()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
    }
    #endregion
}
