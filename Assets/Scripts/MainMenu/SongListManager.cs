using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class SongListManager : MonoBehaviour
{
    public static List<Dictionary<int, string>> songList = new List<Dictionary<int, string>>();

    private Camera mainCam;

    #region 아이템 생성 관련 변수
    [SerializeField] private GameObject itemPref;

    private TMP_Text[] itemList;

    private float interval = 40;

    private int textSize = 15;
    #endregion

    #region 스크롤 관련 변수
    public bool canScroll = true;

    private bool scrolling = false;
    private float lastMousePos = 0;
    private float touchStartTime;
    private float touchStartMousePos;

    private float sensivisity = 1.4f;

    private float centerPos;

    private bool isSliding = false;
    private Coroutine corSlide;
    #endregion

    public static int selectedNum;

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

        centerPos = transform.position.y;

        ItemGeneration();

        Select(0, 0);
    }

    private void ItemGeneration()
    {
        AllItemGeneration();

        SetAllItemPosition();
    }

    // Update is called once per frame
    void Update()
    {
        Scroll();

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedNum + 1 < itemList.Length)
                Select(selectedNum + 1, 0.15f);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedNum - 1 >= 0)
                Select(selectedNum - 1, 0.15f);
        }
    }

    #region 아이템 생성
    private void AllItemGeneration()
    {
        songList.Clear();

        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDicString(temp, ref songList, SONG_LIST_KEY.FindName);

        itemList = new TMP_Text[songList.Count];


        for (int i = 0; i < songList.Count; ++i)
        {
            itemList[i] = InstantiateItem(i);
        }
    }

    private TMP_Text InstantiateItem(int num)
    {
        TMP_Text tmp = Instantiate(itemPref).GetComponent<TMP_Text>();

        tmp.transform.SetParent(this.transform);

        tmp.text = songList[num][SONG_LIST_KEY.SONG_NAME];

        tmp.fontSize = textSize;

        tmp.transform.localScale = Vector3.one;

        tmp.gameObject.name = Level.RemoveSapce(songList[num][SONG_LIST_KEY.SONG_NAME]);

        return tmp;
    }

    private void SetAllItemPosition()
    {
        for (int i = 0; i < itemList.Length; ++i)
        {
            SetItemPosition(itemList[i], i);
        }
    }

    private void SetItemPosition(TMP_Text text, int num)
    {
        text.transform.localPosition = new Vector2(0, -num * interval);
    }
    #endregion

    #region 스크롤
    private void Scroll()
    {
        if (Input.GetMouseButtonDown(0))
            StartScroll();

        else if (Input.GetMouseButtonUp(0))
            EndScroll();

        if(scrolling == true)
            Scrolling();
    }

    private void StartScroll()
    {
        scrolling = true;

        SetLastMousePos();

        touchStartMousePos = Input.mousePosition.y;

        touchStartTime = Time.time;
    }

    private void EndScroll()
    {
        scrolling = false;

        StopSlide();
        //corSlide = StartCoroutine(Slide());
        StartCoroutine(nameof(Slide));

        //Select(FindNearItem());
    }

    private void Scrolling()
    {
        float dir = (lastMousePos - Input.mousePosition.y);

        transform.Translate(Vector2.down * dir * sensivisity);

        SetLastMousePos();

        BlockScroll();
    }

    private void SetLastMousePos()
    {
        lastMousePos = Input.mousePosition.y;
    }

    private bool BlockScroll()
    {
        if (itemList[0].transform.position.y < centerPos)
        {
            Select(0, 0);
            StopSlide();
            return true;
        }
        else if(itemList[itemList.Length - 1].transform.position.y > centerPos)
        {
            Select(itemList.Length - 1, 0); 
            StopSlide();
            return true;
        }

        return false;
    }

    private void Select(int num, float duration = 0.05f)
    {
        selectedNum = num;

        StartCoroutine(MoveSelectedItem(num, duration));

        LightingItem(num);

        string levelName = Level.RemoveSapce(songList[num][SONG_LIST_KEY.SONG_NAME]);

        for(int i = 0; i < difBtn.Length; ++i)
        {
            difBtn[i].name = levelName;

            difBtn[i].interactable = songList[num][i + SONG_LIST_KEY.E] == "0" ? false : true;
        }

        for(int i = 0; i < bestScore.Length; ++i)
        {
            bestScore[i].text = songList[num][i + SONG_LIST_KEY.E_SCORE];
        }

        BackgroundManager.S.SetBgImage(levelName);

    }

    private IEnumerator MoveSelectedItem(int num, float duration)
    {
        Vector2 startPos = transform.position;

        Vector2 targetPos = startPos;
        targetPos.y += -itemList[num].transform.position.y + centerPos;

        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;

        float scrollTime = Time.time - touchStartTime;
        float slideStartPos = Input.mousePosition.y;

        float scrollDis = touchStartMousePos - slideStartPos;

        float startSlideSpeed = Mathf.Pow((scrollDis / scrollTime), 2) / 10000;
        if (touchStartMousePos > slideStartPos)
        {
            startSlideSpeed *= -1f;
        }

        float slideSpeed = startSlideSpeed;

        //Debug.Log(startSlideSpeed);


        float duration = Mathf.Abs(scrollDis / scrollTime / 1000);
        float t = 0;

        while(t <= 1 && Mathf.Abs(slideSpeed) >= 50)
        {
            t += Time.deltaTime / duration;

            slideSpeed = Mathf.Lerp(startSlideSpeed, 0, (t * t) * 10);

            transform.Translate(0, slideSpeed * Time.deltaTime, 0);

            if (BlockScroll())
            {
                isSliding = false;
                yield break;
            }

            //Debug.Log(slideSpeed);

            yield return null;
        }

        Select(FindNearItem());

        isSliding = false;
    }

    private void StopSlide()
    {
        //StopCoroutine(nameof(Slide));

        //if (isSliding == true)
        //{
        //    StopCoroutine(corSlide);
        //    isSliding = false;
        //}

        StopCoroutine(nameof(Slide));
    }

    private void LightingItem(int num)
    {
        for(int i = 0; i < itemList.Length; ++i)
        {
            itemList[i].color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
        }

        itemList[num].color = Color.white;
        //Debug.Log(num);
    }

    private int FindNearItem()
    {
        int nearItem = 0;

        for (int i = 1; i < itemList.Length; ++i)
        {
            if (Mathf.Abs(itemList[nearItem].transform.position.y - centerPos) > Mathf.Abs(itemList[i].transform.position.y - centerPos))
            {
                nearItem = i;
            }
        }

        return nearItem;
    }
    #endregion

    #region 레벨 플레이
    public void OnButtonClick()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        SceneManager.S.StartGame(btn.gameObject.name, DIF.FindValue(btn.tag));
    }
    #endregion
}
