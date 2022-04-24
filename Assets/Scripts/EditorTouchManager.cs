using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTouchManager : MonoBehaviour
{
    //
    private EditorManager editorMgr;
    private GridManager gridMgr;
    private TLNoteManager tlNoteMgr;

    //
    [SerializeField] private Toggle editingToggle;

    //스크롤 구현에 필요한 변수
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;
    private float centerPos;

    //스크롤 슬라이더 관련 변수
    [SerializeField] private Slider tlSlider;
    private float tlLength;

    // Start is called before the first frame update
    void Awake()
    {        
        editorMgr = FindObjectOfType<EditorManager>();

        editorMgr.InitEvent.AddListener(Init);
    }

    private void Init()
    {
        InitVariable();
    }

    private void InitVariable()
    {
        gridMgr = FindObjectOfType<GridManager>();
        tlNoteMgr = FindObjectOfType<TLNoteManager>();

        tlPos = editorMgr.timeLine.transform.position;

        centerPos = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0)).x;

        SetTLSliderMaxValue();
    }

    // Update is called once per frame
    void Update()
    {
        SetEditingMode();

        if(editorMgr.editingMode == true)
        {
            Scroll();

            SetInterval();
        }
    }

    #region 에디트 모드 On/Off 관련 함수
    private void SetEditingMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            editorMgr.editingMode = !editorMgr.editingMode;

            editingToggle.isOn = editorMgr.editingMode;
        }
    }

    public void EditingToggle()
    {
        editorMgr.editingMode = editingToggle.isOn;
    }
    #endregion



    #region 스크롤 관련 함수
    private void Scroll()
    {
        SetActiveScroll();
        
        if(isScrolling == true)
        {
            ScrollTimeLine();
            BlockScroll();
            SetTLSliderValue();
        }

    }

    private void SetActiveScroll()
    {
        //우클릭이 시작되면 스크롤을 활성화
        if (Input.GetMouseButtonDown(1) == true)
        {
            isScrolling = true;
            lastMousePos = Input.mousePosition.x;
            tlPos = editorMgr.timeLine.transform.position;
        }
        else if (Input.GetMouseButtonUp(1) == true)
        {
            isScrolling = false;
        }
    }

    //마우스의 마지막 위치와 현재 위치를 비교하여 변한 값 만큼 타임라인 위치 이동
    private void ScrollTimeLine()
    {
        MoveTimeLine(lastMousePos - Input.mousePosition.x);
        lastMousePos = Input.mousePosition.x;
    }

    private void BlockScroll()
    {
        if (editorMgr.gridList[0].transform.position.x > centerPos)
        {
            editorMgr.timeLine.transform.Translate(-editorMgr.gridList[0].transform.position.x + centerPos, 0, 0);
        }
        else if (editorMgr.gridList[editorMgr.gridList.Count - 1].transform.position.x < centerPos)
        {
            editorMgr.timeLine.transform.Translate(-editorMgr.gridList[editorMgr.gridList.Count - 1].transform.position.x + centerPos, 0, 0);
        }
    }

    private void MoveTimeLine(float moveDis)
    {
        tlPos.x -= moveDis;

        editorMgr.timeLine.transform.position = tlPos;
    }
    #endregion


    #region TimeLineSlider 관련 스크립트
    private void SetTLSliderValue()
    {
        tlSlider.value = -editorMgr.timeLine.transform.position.x + centerPos;
    }
    #endregion



    #region 그리드 및 노트 간격 설정 관련 함수
    private void SetInterval()
    {
        if(Input.GetKey(KeyCode.LeftControl) == true && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            editorMgr.interval += Input.GetAxis("Mouse ScrollWheel") * editorMgr.intervalSensivisity;

            if (editorMgr.interval < 50)
            {
                editorMgr.interval = 50;
            }

            gridMgr.SetAllGridPosition();
            tlNoteMgr.SetAllTLNotePosition();
            SetTLSliderMaxValue();
        }
    }

    private void SetTLSliderMaxValue()
    {
        tlLength = Mathf.Abs(-editorMgr.gridList[editorMgr.gridList.Count - 1].transform.position.x + centerPos);
        tlSlider.maxValue = tlLength;
    }
    #endregion

    //private void CheckSongTime()
    //{
    //    float tlRatio = tlSlider.value / tlSlider.maxValue;
    //    float musicTime = FindObjectOfType<AudioSource>().clip.length * tlRatio;
    //    Debug.Log(musicTime);
    //}
}
