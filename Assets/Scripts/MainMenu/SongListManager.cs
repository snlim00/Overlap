using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongListManager : MonoBehaviour
{
    private List<Dictionary<int, string>> songList = new List<Dictionary<int, string>>();

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

    private float sensivisity = 0.04f;

    private float centerPos;

    private bool isSliding = false;
    private Coroutine corSlide;
    #endregion

    private void Awake()
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
    }

    private void ItemGeneration()
    {
        AllItemGeneration();

        SetAllItemPosition();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scroll();
    }

    #region 아이템 생성
    private void AllItemGeneration()
    {
        List<Dictionary<string, object>> temp = CSVReader.Read("SongList");

        CSVReader.ConvertDIcString(temp, ref songList, SONG_LIST_KEY.FindName);

        itemList = new TMP_Text[songList.Count];


        for (int i = 0; i < songList.Count; ++i)
        {
            itemList[i] = InstantiateItem(i);
        }
    }

    private TMP_Text InstantiateItem(int num)
    {
        TMP_Text text = Instantiate(itemPref).GetComponent<TMP_Text>();

        text.transform.SetParent(this.transform);

        text.text = songList[num][SONG_LIST_KEY.SONG_NAME];

        text.fontSize = textSize;

        text.transform.localScale = Vector3.one;

        return text;
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

    private void Scroll()
    {
        if (canScroll == false)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            StartScroll();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopScroll();
        }
        else
        {
            SongSelect();
        }

        Scrolling();
    }

    private void StartScroll()
    {
        scrolling = true;

        touchStartTime = Time.time;
        touchStartMousePos = Input.mousePosition.y;

        lastMousePos = Input.mousePosition.y;
    }

    private void StopScroll()
    {
        scrolling = false;

        StopSliding();

        if (lastMousePos != Input.mousePosition.y)
        {
            corSlide = StartCoroutine(Slide());
        }
        else
        {
            SongSelect();
        }
    }


    private void Scrolling()
    {
        if (scrolling == false)
            return;

        float dir = (lastMousePos - Input.mousePosition.y) * 35f;

        transform.Translate(Vector2.down * dir * sensivisity);

        BlockScroll();

        lastMousePos = Input.mousePosition.y;
    }

    private void BlockScroll()
    {
        if(itemList[0].transform.position.y < centerPos)
        {
            SetPosition(0);
        }
        else if (itemList[itemList.Length - 1].transform.position.y > centerPos)
        {
            SetPosition(itemList.Length - 1);
        }
    }

    private void SetPosition(int num)
    {
        transform.Translate(0, -itemList[num].transform.position.y + centerPos, 0);
    }

    private IEnumerator Slide()
    {
        isSliding = true;

        float scrollTime = Time.time - touchStartTime;
        float slideStartPos = Input.mousePosition.y;

        float scrollDis = touchStartMousePos - slideStartPos;

        float startSlideSpeed = Mathf.Pow((scrollDis / scrollTime), 2) / 5000;
        if (touchStartMousePos > slideStartPos)
        {
            startSlideSpeed *= -1f;
        }

        float slideSpeed = startSlideSpeed;

        

        float duration = Mathf.Abs(scrollDis / scrollTime / 1000);
        Debug.Log(duration);
        float t = 0;

        while(t <= 1 && Mathf.Abs(slideSpeed) > 1)
        {
            t += Time.deltaTime / duration;

            slideSpeed = Mathf.Lerp(startSlideSpeed, 0, (t * t) * 2);

            //Debug.Log(slideSpeed);

            transform.Translate(0, slideSpeed * Time.deltaTime, 0);

            BlockScroll();
            
            yield return null;
        }

        isSliding = false;
    }

    private bool StopSliding()
    {
        if (isSliding == false)
            return false;


        StopCoroutine(corSlide);

        isSliding = false;

        return true;
    }

    private void SongSelect()
    {
        for(int i = 0; i < itemList.Length; ++i)
        {

        }
    }
}
