using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongListManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPref;

    private List<TMP_Text> itemList = new List<TMP_Text>();

    private List<Dictionary<int, string>> songList = new List<Dictionary<int, string>>();

    private float interval = 50;

    private int textSize = 18;

    private void Awake()
    {
        //interval = (Screen.height * interval) / 634;
        //textSize = (Screen.height * textSize) / 634;

        ItemGeneration();
    }

    public void ItemGeneration()
    {
        AllItemGeneration();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AllItemGeneration()
    {
        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDIcString(temp, ref songList, SONG_LIST_KEY.FindName);

        for (int i = 0; i < songList.Count; ++i)
        {
            itemList.Add(InstantiateItem(i));
        }
    }

    private TMP_Text InstantiateItem(int num)
    {
        TMP_Text text = Instantiate(itemPref).GetComponent<TMP_Text>();

        text.transform.SetParent(this.transform);

        text.text = songList[num][SONG_LIST_KEY.SONG_NAME];

        return text;
    }

    private void SetAllItemPosition()
    {
        for(int i = 0; i < itemList.Count; ++i)
        {
            SetItemPosition(itemList[i], i);
        }
    }
    
    private void SetItemPosition(TMP_Text text, int num)
    {
        text.transform.localPosition = new Vector2(0, -num * interval);
    }
}
