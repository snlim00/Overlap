using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorManager : MonoBehaviour
{
    public UnityEvent InitEvent; //EditorManager�� �ڽĵ��� Init�� ���� ���

    //������ ��� Ȱ��ȭ ���� ����
    public bool editingMode = false;

    //Ÿ�Ӷ��� ������Ʈ
    public GameObject timeLine;

    //������ ��Ʈ�� �׸��带 ��Ƶδ� ����Ʈ
    public List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    public List<GridInfo> gridList = new List<GridInfo>();

    //�׸���� ����Ʈ�� ���� ����
    public float interval;
    public float intervalSensivisity = 100;

    //���õ� ��Ʈ�� �����ϴ� ����
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