using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TLNoteManager : MonoBehaviour
{
    private EditorManager editorMgr;

    [SerializeField] private GameObject tlNotePrefab;

    //노트의 부모 오브젝트
    private Transform noteParents;

    private void Awake()
    {
        editorMgr = FindObjectOfType<EditorManager>();

        editorMgr.InitEvent.AddListener(Init);
    }
    private void Init()
    {
        noteParents = editorMgr.timeLine.transform.Find("Notes");

        TLNoteInit();
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
            editorMgr.tlNoteList.Add(TLNoteGeneration(row));
        }
    }

    private TimeLineNote TLNoteGeneration(int row)
    {
        TimeLineNote tlNote = InstantiateTLNote();

        tlNote.Setting(Level.S.level[row]);

        tlNote.gameObject.name = row.ToString();

        return tlNote;
    }

    private TimeLineNote InstantiateTLNote()
    {
        TimeLineNote tlNote = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

        tlNote.transform.SetParent(noteParents);

        tlNote.transform.localScale = Vector3.one;

        tlNote.GetComponent<Button>().onClick.AddListener(NoteSelect);

        return tlNote;
    }

    public void SetAllTLNotePosition()
    {
        for (int row = 0; row < editorMgr.tlNoteList.Count; ++row)
        {
            TimeLineNote tlNote = editorMgr.tlNoteList[row];

            SetNotePosition(tlNote);
        }
    }

    private void SetNotePosition(TimeLineNote tlNote)
    {
        tlNote.transform.localPosition = new Vector2(tlNote.info[KEY.TIMING] * 0.001f * editorMgr.interval, 0);
    }

    private void SortNoteNum()
    {
        //노트의 인덱스와 num을 x축을 기준으로 하여 오름차순 정렬
        editorMgr.tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < editorMgr.tlNoteList.Count; ++i)
        {
            editorMgr.tlNoteList[i].num = i;
        }
    }
    #endregion




    #region 노트 선택 관련 함수
    //노트의 OnClick에 할당되는 이벤트
    private void NoteSelect()
    {
        if (Input.GetKey(KeyCode.LeftControl) == true)
        {
            SingleNoteToggle();
        }
        else if (Input.GetKey(KeyCode.LeftAlt) == true)
        {
            ChangeStandardNote();
        }
        else if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            MultiNoteSelect();
        }
        else
        {
            SingleNoteSelect();
        }
    }

    //단일 노트 선택
    private void SingleNoteSelect()
    {
        DeselectAll();


        TimeLineNote tlNote = GetCurrentSelectedNote();

        AddSelectedNote(tlNote);

        SetStandardNote(tlNote); //노트 두개식 선택되는거 여기가 문제임. 원인은 모름.
    }

    //단일 노트 토글
    private void SingleNoteToggle()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        NoteToggle(tlNote);
    }

    //다중 노트 선택
    private void MultiNoteSelect()
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
    private void NoteToggle(TimeLineNote tlNote)
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
    private void Deselect(TimeLineNote tlNote)
    {
        editorMgr.selectedNoteList.Remove(tlNote);

        tlNote.Deselect();
    }

    //selectedNoteList에 노트 추가
    private void AddSelectedNote(TimeLineNote tlNote)
    {
        if (editorMgr.selectedNoteList.IndexOf(tlNote) != -1)
            return;

        tlNote.Select();
        editorMgr.selectedNoteList.Add(tlNote);
    }

    //기준 노트 설정
    private void SetStandardNote(TimeLineNote tlNote, bool doRelease = false)
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
    }


    //Event.current.currentSelectedGameObject을 가져오는 함수 (쉬운 추적을 위해 해당 함수를 사용할 것)
    private TimeLineNote GetCurrentSelectedNote()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();
    }
    #endregion
}