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

    #region ���� �÷��� ���� ����
    [SerializeField] private Button[] difBtn;
    [SerializeField] private TMP_Text[] bestScore;
    #endregion

    //BackGroundManager���� ������ �ʱ�ȭ �Ǿ�� ��. (Select ������)
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

    #region ���� �÷���
    public void OnButtonClick()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
    }
    #endregion
}
