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
    private LevelPlayer levelPlayer;

    //
    [SerializeField] private Toggle editingToggle;
    
    //��ũ�� ������ �ʿ��� ����
    private bool canScroll = true;
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;
    private float centerPos;

    //��ũ�� �����̴� ���� ����
    [SerializeField] private Slider tlSlider;
    private float tlLength;

    private bool isPlaying = false;
    private Coroutine corTimeLinePlay;

    // Start is called before the first frame update
    void Awake()
    {        
        editorMgr = FindObjectOfType<EditorManager>();

        editorMgr.InitEvent.AddListener(Init);

        levelPlayer = FindObjectOfType<LevelPlayer>();
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

            tlNoteMgr.PutNote();

            tlNoteMgr.SetNoteType();

            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
                tlNoteMgr.SaveLevel();

            if (Input.GetKey(KeyCode.Delete))
                tlNoteMgr.DeleteAllSelectedNote();

            gridMgr.SetBeat();

        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isPlaying == false)
            {
                LevelPlay();
            }
            else
            {
                LevelStop();
            }
        }
    }

    #region ����Ʈ ��� On/Off ���� �Լ�
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



    #region ��ũ�� ���� �Լ�
    private void Scroll()
    {
        SetActiveScroll();
        
        if(isScrolling == true && canScroll == true)
        {
            ScrollTimeLine();
            BlockScroll();
            SetTLSliderValue();
        }

    }

    private void SetActiveScroll()
    {
        //��Ŭ���� ���۵Ǹ� ��ũ���� Ȱ��ȭ
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

    //���콺�� ������ ��ġ�� ���� ��ġ�� ���Ͽ� ���� �� ��ŭ Ÿ�Ӷ��� ��ġ �̵�
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


    #region TimeLineSlider ���� ��ũ��Ʈ
    private void SetTLSliderValue()
    {
        tlSlider.value = -editorMgr.timeLine.transform.position.x + centerPos;
    }

    public void OnTLSliderValueChange()
    {
        editorMgr.timeLine.transform.position = new Vector2(-tlSlider.value + centerPos, editorMgr.timeLine.transform.position.y);
    }
    #endregion



    #region �׸��� �� ��Ʈ ���� ���� ���� �Լ�
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

        //Debug.Log(centerPos);
        //Debug.Log(tlSlider.maxValue);
        //Debug.Log(editorMgr.gridList[editorMgr.gridList.Count - 1].transform.position.x);
    }
    #endregion



    #region �����Ϳ��� ���� ��� ���� �Լ�
    private void LevelPlay()
    {
        tlNoteMgr.SaveLevel();


        float startTimeRaito = tlSlider.value / tlSlider.maxValue;

        //Debug.Log(startTimeRaito);

        levelPlayer.GameStart(startTimeRaito);

        corTimeLinePlay = StartCoroutine(TimeLinePlay());

        isPlaying = true;
    }

    private void LevelStop()
    {
        levelPlayer.GameStop();

        StopCoroutine(corTimeLinePlay);

        isPlaying = false;
    }

    private IEnumerator TimeLinePlay()
    {
        float speed = -editorMgr.gridList[editorMgr.gridList.Count - 1].transform.localPosition.x / levelPlayer.audioSource.clip.length * (Screen.width / 800f);

        yield return new WaitForSecondsRealtime(Level.S.editorStartDelay + 0.5f);

        while (true)
        {
            editorMgr.timeLine.transform.Translate(speed * Time.deltaTime, 0, 0);

            SetTLSliderValue();

            yield return null;
        }
    }
    #endregion
}