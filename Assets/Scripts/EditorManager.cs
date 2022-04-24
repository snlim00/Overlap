using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorManager : MonoBehaviour
{
    public UnityEvent InitEvent; //EditorManager의 자식들의 Init이 여기 담김

    //에디팅 모드 활성화 관련 변수
    public bool editingMode = false;

    //타임라인 오브젝트
    public GameObject timeLine;

    //생성된 노트와 그리드를 담아두는 리스트
    public List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    public List<GameObject> gridList = new List<GameObject>();

    //그리드와 리스트의 간격 배율
    public float interval;
    public float intervalSensivisity = 100;

    //선택된 노트를 저장하는 변수
    public List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    public TimeLineNote standardNote = null;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        InitEvent.Invoke();
    }
}
