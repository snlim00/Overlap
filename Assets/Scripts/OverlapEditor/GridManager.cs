using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class GridManager : MonoBehaviour
{
    private EditorManager editorMgr;

    //생성할 그리드 프리팹
    [SerializeField] private GameObject gridPrefab;

    //그리드의 부모 오브젝트
    private Transform gridParents;

    //현재 설정된 비트 나눗수를 표시하는 UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //비트 나눗수 관련 변수
    public int selectedBeat = 2;
    public float[] beat = { 1, 2, 4, 8, 16, 32 };
    public const float minBeat = 1f;
    public const float maxBeat = 32f;
    public const float defaultBeat = 4f;

    void Awake()
    {
        editorMgr = FindObjectOfType<EditorManager>();

        editorMgr.InitEvent.AddListener(Init);
    }

    void Init()
    {
        gridParents = editorMgr.timeLine.transform.Find("Grids");

        beatSlider.minValue = 0;
        beatSlider.maxValue = beat.Length - 1;

        GridInit();
    }

    void Update()
    {
        if (editorMgr.editingMode == true)
        {
            SetBeat();
        }
    }

    public void GridInit()
    {
        AllGridGeneration();
        SetAllGridPosition();

        ShowGrid(defaultBeat);

        SetAllGridColor();

        RenewalBeatUI();
    }



    #region 그리드 생성 관련 함수
    private void AllGridGeneration()
    {
        //가장 작은 비트의 음악에서의 길이를 측정하고, 해당 길이를 통해 생성할 그리드 수 결정
        float minBeatLength = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)((Level.S.songLength) / minBeatLength) + 1;

        for (int i = 0; i < gridCount; ++i)
        {
            editorMgr.gridList.Add(InstantiateGrid(i));
        }
    }

    private GridInfo InstantiateGrid(int num)
    {
        GridInfo grid = Instantiate(gridPrefab).GetComponent<GridInfo>();

        grid.transform.SetParent(gridParents);
        grid.transform.localScale = Vector3.one;

        grid.name = num.ToString();

        return grid;
    }

    public void SetAllGridPosition()
    {
        for (int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            SetGridPosition(editorMgr.gridList[i], i);
        }
    }

    private void SetAllGridColor()
    {
        for(int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            if(i % 32 == 0)
            {
                editorMgr.gridList[i].GetComponent<Image>().color = Color.white;
            }
            else if (i % 16 == 0)
            {
                editorMgr.gridList[i].GetComponent<Image>().color = Color.red;
            }
            else if (i % 8 == 0)
            {
                editorMgr.gridList[i].GetComponent<Image>().color = Color.blue;
            }
            else if (i % 4 == 0)
            {
                editorMgr.gridList[i].GetComponent<Image>().color = Color.yellow;
            }
            else if (i % 2 == 0)
            {
                editorMgr.gridList[i].GetComponent<Image>().color = Color.cyan;
            }
            else editorMgr.gridList[i].GetComponent<Image>().color = Color.gray;
        }
    }
    #endregion



    private void SetGridPosition(GridInfo grid, int num)
    {
        float gridPosition = ((60f / Level.S.bpm) * (1 / maxBeat) * editorMgr.interval) * num;

        grid.transform.localPosition = new Vector2(gridPosition, 0);
    }

    private void ShowGrid(float beat)
    {
        for (int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            GridInfo grid = editorMgr.gridList[i];

            //해당 노트가 비트에 해당하지 않는다면 비활성화, 해당한다면 활성화
            if (i % (32 / beat) != 0)
            {
                grid.gameObject.SetActive(false);
            }
            else
            {
                grid.gameObject.SetActive(true);
            }
        }

        //첫번째 그리드는 항상 활성화
        editorMgr.gridList[0].gameObject.SetActive(true);
    }



    #region 비트 나눗수 설정 관련 함수
    private void SetBeat()
    {
        if (_SetBeat() == false)
            return;

        ShowGrid(beat[selectedBeat]);
        RenewalBeatUI();
    }

    private bool _SetBeat()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat + 1);

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat - 1);

            return true;
        }
        else return false;
    }

    private int SetBeatIndex(int index)
    {
        if (index >= beat.Length)
        {
            index = beat.Length - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        return index;
    }

    private void RenewalBeatUI()
    {
        beatSlider.value = selectedBeat;
        beatText.text = "1 / " + beat[selectedBeat];
    }
    #endregion
}