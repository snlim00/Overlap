using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public ParticleManager particleMgr;

    [SerializeField] private GameObject touchSFXPref;
    private AudioSource[] touchSFXObj;
    private int touchSFXObjCount = 20;

    [SerializeField] private List<Note> hitNoteList = new List<Note>();
    private List<Note> clearedNoteList = new List<Note>();

    [SerializeField] private int inputCount = 0;

    [SerializeField] private GameObject center;
    [SerializeField] private int judgAngle = 35;

    public static bool holding = false;
    private bool isEndHolding = false;
    private Coroutine corEndHolding;

    private void Awake()
    {
        TouchSFXGeneration();
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

        if (Input.GetKeyDown(KeyCode.R))
            SceneMgr.S.ResultScene();
    }
    
    private IEnumerator EndHolding()
    {
        isEndHolding = true;
        yield return new WaitForSeconds(Level.S.judgRange[JUDG.S_PERFECT]);

        holding = false;
        isEndHolding = false;
    }

    private void Touch()
    {
        inputCount = Input.inputString.Length; //입력된 터치 수 확인
        Debug.Log("Touch: " + Time.time);
        //PlayTouchSFX();

        //주변 노트 가져오기
        GetAroundNote();

        //가져온 노트가 0개라면 함수 종료
        if (hitNoteList.Count <= 0)
            return;

        //퍼펙트 판정인 노트가 있는지 확인
        //각 판정에 해당하는 노트가 있다면 다음 판정을 확인하지 않는 방식.
        if(CheckJudg(JUDG.S_PERFECT) == true)
        {
            ClearNote(JUDG.S_PERFECT);
        }
        else if (CheckJudg(JUDG.PERFECT) == true)
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

        int checkedNoteCount = 0;

        for(int i = 0; i < Level.S.noteList.Count; ++i)
        {
            //판정 범위 내의 노트인지 확인하고 리스트에 추가
            if (Mathf.Abs(Level.S.noteList[i].timing - LevelPlayer.timer) <= Level.S.judgRange[JUDG.MISS])
            {
                if (Level.S.noteList[i].type == NOTE_TYPE.TAP || Level.S.noteList[i].type == NOTE_TYPE.DOUBLE)
                {
                    hitNoteList.Add(Level.S.noteList[i]);
                    checkedNoteCount += 1;
                }
            }
            else if (Level.S.noteList[i].timing <= LevelPlayer.timer - Level.S.judgRange[JUDG.MISS])
            {
                continue;
            }
            //판정 범위보다 뒤에 있는 노트를 마주치면 체크 종료
            else
            {
                break;
            }
        }

        clearedNoteList.Clear();
    }

    private bool CheckJudg(int judg)
    {
        bool isClear = false;

        for (int i = 0; i < hitNoteList.Count; ++i)
        {
            //판정 처리
            if (Mathf.Abs(hitNoteList[i].timing - LevelPlayer.timer) <= Level.S.judgRange[judg])
            {
                //각도 처리
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
            //Debug.Log(LevelPlayer.timer - clearedNoteList[i].timing);
            clearedNoteList[i].Clear(judg);
        }

        particleMgr.ParticleGeneration(judg);
    }

    private void TouchSFXGeneration()
    {
        AudioClip touchSFXClip = Resources.Load(Level.S.levelName + "_SFX") as AudioClip;

        touchSFXObj = new AudioSource[touchSFXObjCount];

        for(int i = 0; i < touchSFXObj.Length; ++i)
        {
            touchSFXObj[i] = Instantiate(touchSFXPref).GetComponent<AudioSource>();

            touchSFXObj[i].clip = touchSFXClip;
        }
            
    }

    private int poolingCount = 0;
    private void PlayTouchSFX()
    {
        touchSFXObj[poolingCount].Play();

        poolingCount += 1;

        if(poolingCount >= touchSFXObjCount)
        {
            poolingCount = 0;
        }

        Debug.Log("SFX: " + Time.time);
    }
}