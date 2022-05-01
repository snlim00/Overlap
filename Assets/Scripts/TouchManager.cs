using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private LevelPlayer levelPlayer;
    [SerializeField] private ParticleManager particleMgr;


    [SerializeField] private List<Note> hitNoteList = new List<Note>();
    private List<Note> clearedNoteList = new List<Note>();


    [SerializeField] private int inputCount = 0;

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
    }

    private void Touch()
    {
        inputCount = Input.inputString.Length; //입력된 터치 수 확인

        //주변 노트 가져오기
        GetAroundNote();

        //가져온 노트가 0개라면 함수 종료
        if (hitNoteList.Count <= 0)
            return;

        //퍼펙트 판정인 노트가 있는지 확인
        //각 판정에 해당하는 노트가 있다면 다음 판정을 확인하지 않는 방식.
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
            if (Level.S.noteList[i].timing - levelPlayer.timer <= Level.S.judgRange[JUDG.MISS] * 1.3f)
            {
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
            if (Mathf.Abs(hitNoteList[i].timing - (float)levelPlayer.timer) <= Level.S.judgRange[judg])
            {
                clearedNoteList.Add(hitNoteList[i]);
                //Debug.Log(hitNoteArr[i].timing - levelPlayer.t);
                isClear = true;
            }
        }

        return isClear;
    }

    private void ClearNote(int judg)
    {
        for (int i = 0; i < inputCount && i < clearedNoteList.Count; ++i)
        {
            clearedNoteList[i].Clear(judg);
        }

        particleMgr.ParticleGeneration(judg);
    }
}