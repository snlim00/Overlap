using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public ParticleManager particleMgr;


    [SerializeField] private List<Note> hitNoteList = new List<Note>();
    private List<Note> clearedNoteList = new List<Note>();


    [SerializeField] private int inputCount = 0;

    [SerializeField] private GameObject center;
    [SerializeField] private int judgAngle = 35;

    public static bool holding = false;
    private bool isEndHolding = false;
    private Coroutine corEndHolding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && Input.inputString.Length > 0)
        {
            Touch();
        }

        if (Input.anyKey)
        {
            holding = true;
        }
        else if (holding == true)
        {
            if(isEndHolding == true)
            {
                StopCoroutine(corEndHolding);
                holding = false;
                isEndHolding = false;
            }

            corEndHolding = StartCoroutine(EndHolding());
        }
    }

    private IEnumerator EndHolding()
    {
        isEndHolding = true;
        yield return new WaitForSeconds(Level.S.judgRange[JUDG.PERFECT] * 0.5f);

        holding = false;
        isEndHolding = false;
    }

    private void Touch()
    {
        inputCount = Input.inputString.Length; //�Էµ� ��ġ �� Ȯ��

        //�ֺ� ��Ʈ ��������
        GetAroundNote();

        //������ ��Ʈ�� 0����� �Լ� ����
        if (hitNoteList.Count <= 0)
            return;

        //����Ʈ ������ ��Ʈ�� �ִ��� Ȯ��
        //�� ������ �ش��ϴ� ��Ʈ�� �ִٸ� ���� ������ Ȯ������ �ʴ� ���.
        if (CheckJudg(JUDG.PERFECT) == true)
        {
            ClearNote(JUDG.PERFECT);
        }
        else if(CheckJudg(JUDG.GOOD) == true)
        {
            ClearNote(JUDG.GOOD);
        }
        else if(CheckJudg(JUDG.MISS) == true)
        {
            ClearNote(JUDG.MISS);
        }
    }

    private void GetAroundNote()
    {
        hitNoteList.Clear();

        for(int i = 0; i < Level.S.noteList.Count; ++i)
        {
            if (Level.S.noteList[i].timing - LevelPlayer.timer <= Level.S.judgRange[JUDG.MISS] * 1.3f)
            {
                if(Level.S.noteList[i].type == NOTE_TYPE.TAP || Level.S.noteList[i].type == NOTE_TYPE.DOUBLE)
                    hitNoteList.Add(Level.S.noteList[i]);
            }
            else break;
        }

        clearedNoteList.Clear();
    }

    private bool CheckJudg(int judg)
    {
        bool isClear = false;

        for (int i = 0; i < hitNoteList.Count; ++i)
        {
            //���� ó��
            if (Mathf.Abs(hitNoteList[i].timing - LevelPlayer.timer) <= Level.S.judgRange[judg])
            {
                //���� ó��
                if(CheckAngle(hitNoteList[i].transform.eulerAngles.z) == true)
                {
                    clearedNoteList.Add(hitNoteList[i]);
                    //Debug.Log(hitNoteArr[i].timing - levelPlayer.t);
                    isClear = true;
                }
            }
        }

        return isClear;
    }

    public bool CheckAngle(float angle)
    {
        if(Mathf.Abs(angle - (center.transform.eulerAngles.z - 90)) < judgAngle || Mathf.Abs(angle - (center.transform.eulerAngles.z - 90) - 360) < judgAngle)
        {
            return true;
        }

        return false;
    }

    private void ClearNote(int judg)
    {
        for (int i = 0; i < inputCount && i < clearedNoteList.Count; ++i)
        {
            Debug.Log(LevelPlayer.timer - clearedNoteList[i].timing);
            clearedNoteList[i].Clear(judg);
        }

        particleMgr.ParticleGeneration(judg);
    }
}