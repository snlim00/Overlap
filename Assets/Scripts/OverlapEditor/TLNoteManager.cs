using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TLNoteManager : MonoBehaviour
{
    private EditorManager editorMgr;

    [SerializeField] private GameObject[] tlNotePrefab;

    //노트의 부모 오브젝트
    private Transform noteParents;

    //노트 배치 관련 변수
    [SerializeField] private int noteType = NOTE_TYPE.NONE;

    //노트 정보 수정 관련 변수
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject[] infoUI = new GameObject[2];
    private int noteInfoStartNum = 4;
    private float infoUIInterval = 40;
    private NoteInfo[] noteInfo = new NoteInfo[KEY.COUNT];

    public static bool editInputFieldNow = false;

    private void Awake()
    {
        editorMgr = FindObjectOfType<EditorManager>();

        editorMgr.InitEvent.AddListener(Init);
    }
    private void Init()
    {
        noteParents = editorMgr.timeLine.transform.Find("Notes");

        TLNoteInit();

        AllInfoUIGeneration();

        EVENT_NAME.ReadEventTypeName();
    }

    private void TLNoteInit()
    {
        AllTLNoteGeneration();
        SetAllTLNotePosition();
        SortNoteNum();
    }



    #region 노트 생성 관련 함수
    private void AllTLNoteGeneration()
    {
        for (int row = 0; row < Level.S.level.Count; ++row)
        {
            TLNoteGeneration(row);
        }
    }

    private TimeLineNote TLNoteGeneration(int row)
    {
        TimeLineNote tlNote = InstantiateTLNote((int)Level.S.level[row][KEY.NOTE_TYPE]);

        tlNote.Setting(Level.S.level[row]);

        tlNote.gameObject.name = row.ToString();

        return tlNote;
    }

    private TimeLineNote InstantiateTLNote(int noteType)
    {
        TimeLineNote tlNote = Instantiate(tlNotePrefab[noteType]).GetComponent<TimeLineNote>();

        tlNote.transform.SetParent(noteParents);

        tlNote.transform.localScale = Vector3.one;

        tlNote.GetComponent<Button>().onClick.AddListener(delegate { NoteSelect(); });

        tlNote.Setting(new Dictionary<int, float>(Level.S.levelFormat));
        
        editorMgr.tlNoteList.Add(tlNote);

        return tlNote;
    }

    public void SetAllTLNotePosition()
    {
        for (int row = 0; row < editorMgr.tlNoteList.Count; ++row)
        {
            TimeLineNote tlNote = editorMgr.tlNoteList[row];
            SetTLNotePosition(tlNote, editorMgr.gridList[(int)tlNote.info[KEY.GRID_NUM]]);
        }
    }

    private void SetTLNotePosition(in TimeLineNote tlNote, GridInfo grid)
    {
        Vector2 pos = grid.transform.localPosition;

        if (tlNote.info[KEY.TYPE] != TYPE.EVENT)
        {
            grid.haveNote = tlNote;
            grid.isHaveNote = true;
        }
        else
        {
            grid.eventCount += 1;
            pos.y = -40f + (-22f * grid.eventCount);
        }

        tlNote.transform.localPosition = pos;

        tlNote.info[KEY.GRID_NUM] = editorMgr.gridList.IndexOf(grid);
    }

    private void SortNoteNum()
    {
        //노트의 인덱스와 num을 x축을 기준으로 하여 오름차순 정렬
        editorMgr.tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < editorMgr.tlNoteList.Count; ++i)
        {
            editorMgr.tlNoteList[i].num = i;
            editorMgr.tlNoteList[i].gameObject.name = i.ToString();
        }
    }
    #endregion



    #region 노트 선택 관련 함수
    //노트의 OnClick에 할당되는 이벤트
    private void NoteSelect()
    {
        if (Input.GetKey(KeyCode.LeftControl) == true)
        {
            ToggleSingleNote();
        }
        else if (Input.GetKey(KeyCode.LeftAlt) == true)
        {
            ChangeStandardNote();
        }
        else if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            SelectMultiNote();
        }
        else
        {
            SelectSingleNote();
        }
    }

    //단일 노트 선택
    private void SelectSingleNote()
    {
        ApplyInfoValue();

        DeselectAll();


        TimeLineNote tlNote = GetCurrentSelectedNote();

        AddSelectedNote(tlNote);

        SetStandardNote(tlNote);
    }

    //단일 노트 토글
    private void ToggleSingleNote()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        NoteToggle(tlNote);
    }

    //다중 노트 선택
    private void SelectMultiNote()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        int stdNoteNum = editorMgr.standardNote.num;

        if (tlNote.num > stdNoteNum)
        {
            for (int i = stdNoteNum; i <= tlNote.num; ++i)
            {
                AddSelectedNote(editorMgr.tlNoteList[i]);
            }
        }
        else
        {
            for (int i = stdNoteNum; i >= tlNote.num; --i)
            {
                AddSelectedNote(editorMgr.tlNoteList[i]);
            }
        }
    }

    //기준 노트 변경
    private void ChangeStandardNote()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        AddSelectedNote(tlNote);
        SetStandardNote(tlNote, true);
    }

    //노트 토글
    private void NoteToggle(in TimeLineNote tlNote)
    {
        //standardNote는 선택 해제할 수 없으므로 함수 종료.
        if (tlNote == editorMgr.standardNote)
            return;

        if (tlNote.isSelected == true)
            Deselect(tlNote);
        else
            AddSelectedNote(tlNote);
    }

    //모든 노트 선택 해제 (단일 노트 선택 시에만 사용, 그 외에 사용 시 기준 노트가 사라지는 불상사 발생)
    private void DeselectAll()
    {
        //첫번째 배열이 사라지며 다음 요소가 첫번째 배열로 이동하므로 항상 첫번째 배열만 삭제하면 됨.
        int count = editorMgr.selectedNoteList.Count;
        for (int i = 0; i < count; ++i)
        {
            Deselect(editorMgr.selectedNoteList[0]);
        }

        editorMgr.selectedNoteList.Clear();
    }

    //노트 선택 해제
    private void Deselect(in TimeLineNote tlNote)
    {
        editorMgr.selectedNoteList.Remove(tlNote);

        tlNote.Deselect();
    }

    //selectedNoteList에 노트 추가
    private void AddSelectedNote(in TimeLineNote tlNote)
    {
        if (editorMgr.selectedNoteList.IndexOf(tlNote) != -1)
            return;

        tlNote.Select();
        editorMgr.selectedNoteList.Add(tlNote);
        //Debug.Log(nameof(AddSelectedNote));
    }

    //기준 노트 설정
    private void SetStandardNote(in TimeLineNote tlNote, bool doRelease = false)
    {
        _SetStandardNote(tlNote, doRelease);

        ShowInfo(tlNote);
    }

    private void _SetStandardNote(TimeLineNote tlNote, bool doRelease = false)
    {
        //기존 기준 노트를 기준 해제
        if (editorMgr.standardNote != null)
        {
            if (doRelease == false)
            {
                editorMgr.standardNote.UnsetStandardNote();
            }
            else
            {
                editorMgr.standardNote.ReleaseStandardNote();
            }
        }

        editorMgr.standardNote = tlNote;
        editorMgr.standardNote.SetStandardNote();

        //Debug.Log(nameof(_SetStandardNote));
    }


    //Event.current.currentSelectedGameObject을 가져오는 함수 (쉬운 추적을 위해 해당 함수를 사용할 것)
    private TimeLineNote GetCurrentSelectedNote()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();
    }
    #endregion



    #region 노트 설치/제거 관련 함수
    public void SetNoteType()
    {
        if (editInputFieldNow == true)
            return;

        //노트 타입 변경
        if (Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            noteType = NOTE_TYPE.TAP;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) == true)
        {
            noteType = NOTE_TYPE.DOUBLE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) == true)
        {
            noteType = NOTE_TYPE.SLIDE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) == true)
        {
            noteType = NOTE_TYPE.EVENT;
        }
        else if(Input.GetKeyDown(KeyCode.BackQuote) == true)
        {
            noteType = NOTE_TYPE.NONE;
        }

        Level.S.levelFormat[KEY.TYPE] = NOTE_TYPE.NONE;
        Level.S.levelFormat[KEY.NOTE_TYPE] = NOTE_TYPE.NONE;

        //변경된 노트 타입 적용
        if (noteType != NOTE_TYPE.EVENT)
        {
            Level.S.levelFormat[KEY.TYPE] = TYPE.NOTE;
            Level.S.levelFormat[KEY.NOTE_TYPE] = noteType;
        }
        else
        {
            Level.S.levelFormat[KEY.TYPE] = TYPE.EVENT;
            Level.S.levelFormat[KEY.NOTE_TYPE] = noteType;
        }
    }

    public void PutNote()
    {
        if (Input.GetMouseButtonDown(0) == false || noteType == NOTE_TYPE.NONE)
            return;

        GridInfo nearGrid = FindNearGrid();

        if (noteType != NOTE_TYPE.EVENT && nearGrid.isHaveNote == true) 
        {
            if (nearGrid.haveNote.info[KEY.NOTE_TYPE] != noteType) //설치하려는 그리드가 가진 노트와 타입이 다르다면 기존 노트 파괴
            {
                DeleteNote(nearGrid.haveNote);
            }
            else if (nearGrid.haveNote.info[KEY.NOTE_TYPE] == noteType) //설치하려는 그리드가 가진 노트와 타입이 같다면 함수 종료
            {
                return;
            }
        }


        TimeLineNote tlNote = InstantiateTLNote(noteType).GetComponent<TimeLineNote>();

        SetTLNotePosition(tlNote, nearGrid);

        SortNoteNum();
    }
    
    private GridInfo FindNearGrid()
    {
        GridInfo nearGrid = editorMgr.gridList[0];
        float nearDis = Mathf.Abs(nearGrid.transform.position.x - Input.mousePosition.x);

        for (int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            if (editorMgr.gridList[i].gameObject.activeSelf == true
                && Mathf.Abs(editorMgr.gridList[i].transform.position.x - Input.mousePosition.x) < nearDis)
            {
                nearGrid = editorMgr.gridList[i];
                nearDis = Mathf.Abs(nearGrid.transform.position.x - Input.mousePosition.x);
            }
        }

        return nearGrid;
    }

    public void DeleteAllSelectedNote()
    {
        //첫번째 배열이 사라지며 다음 요소가 첫번째 배열로 이동하므로 항상 첫번째 배열만 삭제하면 됨.
        for (int i = 0; i < editorMgr.selectedNoteList.Count; ++i)
        {
            DeleteNote(editorMgr.selectedNoteList[0]);
        }
    }

    private void DeleteNote(TimeLineNote tlNote)
    {
        Deselect(tlNote);

        editorMgr.tlNoteList.Remove(tlNote);

        Destroy(tlNote.gameObject);

        editorMgr.gridList[(int)tlNote.info[KEY.GRID_NUM]].isHaveNote = false;
    }
    #endregion



    public void SaveLevel()
    {
        ApplyInfoValue();

        SortNoteNum();

        Level.S.level.Clear();
        for(int i = 0; i < editorMgr.tlNoteList.Count; ++i)
        {
            //interval만큼 나누고 ms단위로 변환해주기 위해 위해 1000을 곱해줌.
            editorMgr.tlNoteList[i].info[KEY.TIMING] = Convert.ToInt32(((editorMgr.tlNoteList[i].transform.localPosition.x / editorMgr.interval) * 1000)/* + editorMgr.offset*/);


            //if (editorMgr.tlNoteList[i].info[KEY.TYPE] == TYPE.EVENT)
            //    Debug.Log(editorMgr.tlNoteList[i].info[KEY.GRID_NUM] + " / " + editorMgr.tlNoteList[i].info[KEY.TIMING]);


            Level.S.level.Add(editorMgr.tlNoteList[i].info);
        }
        Debug.Log("Saved!");
        Level.S.WriteLevel();
    }


    #region 노트 정보 변경 관련 함수
    public void MoveSelectedNote(int dir)
    {
        GridManager gridMgr = FindObjectOfType<GridManager>();

        for(int i = 0; i < editorMgr.selectedNoteList.Count; ++i)
        {
            editorMgr.selectedNoteList[i].info[KEY.GRID_NUM] += (gridMgr.maxBeat / gridMgr.beat[gridMgr.selectedBeat]) * dir;
        }

        SetAllTLNotePosition();
    }

    private void AllInfoUIGeneration()
    {
        for(int i = noteInfoStartNum; i < KEY.COUNT; ++i)
        {
            noteInfo[i] = InfoUIGeneration(i);
        }
    }

    private NoteInfo InfoUIGeneration(int num)
    {
        NoteInfo noteInfo = new NoteInfo();

        GameObject go = Instantiate(infoUI[KEY.KEY_TYPE[num]]);

        noteInfo.InitInfo(go, num, delegate { SetValueName(); });

        go.transform.SetParent(infoPanel.transform);
        go.transform.localPosition = new Vector2(0, 170 - (infoUIInterval * (num - noteInfoStartNum)));
        go.transform.localScale = Vector3.one;
        //go.transform.localScale = new Vector2(0.9f, 0.9f);

        return noteInfo;
    }

    private void ShowInfo(in TimeLineNote tlNote)
    {
        SetInfoValue(tlNote.info);

        HideUselessInfo((int)tlNote.info[KEY.NOTE_TYPE]);

        SetValueName(tlNote.info);
    }

    private void HideUselessInfo(int noteType)
    {
        if(noteType == NOTE_TYPE.EVENT)
        {
            for(int i = noteInfoStartNum; i < noteInfo.Length; ++i)
            {
                noteInfo[i].SetActive(true);
            }
            noteInfo[KEY.ANGLE].SetActive(false);
        }
        else
        {
            for (int i = noteInfoStartNum; i < noteInfo.Length; ++i)
            {
                noteInfo[i].SetActive(false);
            }
            noteInfo[KEY.ANGLE].SetActive(true);
        }
    }

    private void SetInfoValue(in Dictionary<int, float> info)
    {
        for (int i = noteInfoStartNum; i < KEY.COUNT; ++i)
        {
            noteInfo[i].SetInfo(info[i]);
        }
    }

    private void SetValueName(Dictionary<int, float> info)
    {
        if (info[KEY.NOTE_TYPE] != NOTE_TYPE.EVENT || noteInfo[KEY.EVENT_NAME].GetInfo() == EVENT_NAME.NONE)
        {
            return;
        }

        for(int i = 0; i < KEY.VALUE.Length; ++i)
        {
            noteInfo[i + KEY.VALUE[0]].SetName(EVENT_NAME.VALUES[(int)info[KEY.EVENT_NAME]][i]);
        }
    }

    private void SetValueName()
    {
        if (noteInfo[KEY.EVENT_NAME].GetInfo() == EVENT_NAME.NONE)
            return;

        for (int i = 0; i < KEY.VALUE.Length; ++i)
        {
            noteInfo[i + KEY.VALUE[0]].SetName(EVENT_NAME.VALUES[(int)noteInfo[KEY.EVENT_NAME].GetInfo()][i]);
        }
    }

    private void ApplyInfoValue()
    {
        for (int i = 0; i < editorMgr.selectedNoteList.Count; ++i)
        {
            for (int j = noteInfoStartNum; j < noteInfo.Length; ++j)
            {
                editorMgr.selectedNoteList[i].info[j] = noteInfo[j].GetInfo();
            }
        }

        //Debug.Log(nameof(ApplyInfoValue));

    }
    #endregion

    public static void SetEditInputFieldNow(bool value)
    {
        editInputFieldNow = value;

        //Debug.Log(value);
    }
}