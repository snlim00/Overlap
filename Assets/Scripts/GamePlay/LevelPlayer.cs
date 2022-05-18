using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] notePref;

    private EditorManager editorMgr;


    public AudioSource audioSource;


    private SpriteRenderer bg;

    private Camera mainCam;
    private float camSize = 5;


    //게임을 진행하는 코루틴들
    private bool isCorNoteTimer = false;
    private Coroutine corNoteTimer;

    public static float timer { get; private set; } = 0;
    public float timerForDebug;
    public static float startTime;
    public Dictionary<int, int> thisRow;

    public float playStartTime;

    // Start is called before the first frame update
    void Awake()
    {
        playStartTime = Time.time;

        editorMgr = FindObjectOfType<EditorManager>();

        audioSource = GetComponent<AudioSource>();

        mainCam = Camera.main;

        //ReadLevel이 곡의 이름에서 공백을 제거하여 string으로 반환, 이후 해당 문자열로 곡 파일 탐색
        //audioSource.clip = Resources.Load<AudioClip>(Level.S.ReadLevel("MeteorStream", DIF.X));

        audioSource.clip = Resources.Load<AudioClip>(Level.S.levelName);
        Level.S.songLength = audioSource.clip.length;


        if(PlayerSetting.S.editerMode == false)
            GameStart();
    }

    public void GameStart(float startTimeRaito = 0)
    {
        BackgroundManager.S.SetBgImage(Level.S.levelName, 0);   

        float startTime = audioSource.clip.length * startTimeRaito;
        int startRow = 0;

        mainCam.orthographicSize = camSize;
        mainCam.transform.position = new Vector3(0, 0, -10);

        for (int i = 0; i < Level.S.level.Count; ++i)
        {
            if (Level.S.level[i][KEY.TIMING] * 0.001f >= startTime)
            {
                startRow = i;
                break;
            }
        }


        NoteGeneration(startTime, startRow);

        corNoteTimer = StartCoroutine(NoteTimer(startTime, startRow));
        StartCoroutine(SongPlay(startTime));
    }

    public void GameStop()
    {
        audioSource.Stop();

        if(isCorNoteTimer == true)
        {
            isCorNoteTimer = false;
            StopCoroutine(corNoteTimer);
        }


        for(int i = 0; i < Level.S.noteList.Count; ++i)
        {
            if(Level.S.noteList[i] != null)
                Destroy(Level.S.noteList[i].gameObject);
        }
        Level.S.noteList.Clear();
    }

    private IEnumerator NoteTimer(float startTime = 0, int startRow = 0)
    {
        isCorNoteTimer = true;

        int row;

        if(PlayerSetting.S.editerMode == true)
        {
            if (startRow >= Level.S.level.Count)
                yield break;

            playStartTime = Time.time - startTime;

            row = startRow;
        }
        else
        {
            row = 0;
            timer = 0; 
            //startDelay 대기 후 타이머 실행 (offset을 해당 타이머에서 적용하면 변속과 이벤트의 타이밍에도 영향을 미침 -> 음악 재생을 늦추는 방식으로 오프셋 맞추기)
            //yield return new WaitForSeconds(Level.S.startDelay);
        }
        Debug.Log("Start Timer: " + Time.time);

        float lastNoteTiming = Level.S.level[Level.S.level.Count - 1][KEY.TIMING] * 0.001f + 0;

        thisRow = Level.S.level[row]; 

        while (timer < audioSource.clip.length)// && row < Level.S.level.Count)
        //while (timer < lastNoteTiming && row < Level.S.level.Count)
        {
            //timer += Time.deltaTime;
            timer = Time.time - playStartTime - Level.S.startDelay;

            if (PlayerSetting.S.editerMode == true)
                timer += Level.S.startDelay;

            timerForDebug = timer;

            if(row < Level.S.level.Count)
            {
                if (timer >= thisRow[KEY.TIMING] * 0.001) //타이머가 다음 행의 TIMING에 도달하면 실행
                {
                    thisRow = Level.S.level[row];

                    if (thisRow[KEY.TYPE] == TYPE.EVENT) //다음 행이 EVENT라면 실행
                    {
                        EventExecute();
                    }

                    ++row;
                }
            }
            

            yield return null;
        }

        Debug.Log("END");
        GameManager.ResultScene();
        isCorNoteTimer = false;
    }

    private IEnumerator SongPlay(float startTime = 0)
    {
        if(PlayerSetting.S.editerMode == true)
        {
            audioSource.time = startTime;
            //yield return new WaitForSeconds(PlayerSetting.S.songOffset);
        }
        else
        {
            audioSource.time = 0;
            yield return new WaitForSeconds(Level.S.startDelay + PlayerSetting.S.songOffset);
            //Debug.Log(Level.S.startDelay);
        }
        Debug.Log("Start Song: " + Time.time);

        audioSource.Play();
        //Debug.Log("audioSource Start");
    }

    private void NoteGeneration(float startTime = 0, int startRow = 0)
    {
        for(int row = startRow; row < Level.S.level.Count; ++row)
        {
            Dictionary<int, int> thisRow = Level.S.level[row];
            
            if (thisRow[KEY.TYPE] == TYPE.NOTE) //노트 생성
            {
                if(thisRow[KEY.NOTE_TYPE] == NOTE_TYPE.DOUBLE)
                {
                    for(int i = 0; i < 2; ++i)
                    {
                        InstantiateNote(row, startTime, thisRow);
                    }
                }
                else if(thisRow[KEY.TYPE] != NOTE_TYPE.EVENT)
                {
                    InstantiateNote(row, startTime, thisRow);
                }
            }
        }
    }

    private void InstantiateNote(int row, float startTime, in Dictionary<int, int> thisRow)
    {
        Note note;

        note = Instantiate(notePref[thisRow[KEY.NOTE_TYPE]]).GetComponent<Note>();

        int angle = thisRow[KEY.ANGLE];

        if(angle > 0)
        {
            angle = angle % 360;
        }
        else
        {
            angle = 360 + (angle % -360);
        }

        float timing;

        if (PlayerSetting.S.editerMode == true)
        {
            timing = (thisRow[KEY.TIMING] * 0.001f) - startTime;
        }
        else
        {
            timing = (thisRow[KEY.TIMING] * 0.001f) + Level.S.startDelay;// + PlayerSetting.S.noteOffset;
        }

        float spawnDis = Level.S.noteSpeed * timing;

        //timing += Level.S.startDelay;

        note.Execute(row, angle, thisRow[KEY.TIMING] * 0.001f, spawnDis, thisRow[KEY.NOTE_TYPE]);

        Level.S.noteList.Add(note);
    }

    private void EventExecute()
    {
        StartCoroutine(EVENT_NAME.FindName(thisRow[KEY.EVENT_NAME]));

        //switch (thisRow[KEY.EVENT_NAME]) //각 이벤트 호출
        //{
        //    case EVENT_NAME.SET_SPEED:
        //        SetSpeed(thisRow);
        //        break;
        //}
    }

    private float LerpValue(float t, int type)
    {
        float p = 0;

        switch (type)
        {
            case 0:
                p = t;
                break;

            case 1:
                p = t * t;
                break;

            case 2:
                p = -((2 * t - 1) * (2 * t - 1)) + 1;
                break;
        }

        return p;
    }

    private float BeatToDuration(float beat)
    {
        float duration = (60f / Level.S.bpm) / beat;

        return duration;
    }

    #region 레벨 이벤트 함수
    private void SET_SPEED()
    {
        int speed = thisRow[KEY.VALUE[0]];

        Level.S.noteSpeed = speed;
    }

    private IEnumerator CAMERA_MOVE()
    {
        Vector3 targetPos = new Vector3(thisRow[KEY.VALUE[0]], thisRow[KEY.VALUE[1]], -10);
        bool withBG = Convert.ToBoolean(thisRow[KEY.VALUE[2]]);
        float duration = BeatToDuration(thisRow[KEY.DURATION]);
        int type = thisRow[KEY.EVENT_TYPE];

        Vector3 startPos = mainCam.transform.position;
        Vector3 bgPos;
        float t = 0;
        float p = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            p = LerpValue(t, type);

            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, p);


            if(withBG == true)
            {
                bgPos = mainCam.transform.position;
                bgPos.z = 0;
                //bg.transform.position = bgPos;
            }

            yield return null;
        }
    }

    private IEnumerator CAMERA_ZOOM()
    {
        float targetScale = thisRow[KEY.VALUE[0]];
        int type = thisRow[KEY.EVENT_TYPE];
        float duration = BeatToDuration(thisRow[KEY.DURATION]);
        bool withBG = Convert.ToBoolean(thisRow[KEY.VALUE[1]]);

        float startScale = mainCam.orthographicSize;

        float t = 0;
        float p = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            p = LerpValue(t, type);

            mainCam.orthographicSize = Mathf.Lerp(startScale, startScale * (targetScale / 100f), p);


            yield return null;
        }
    }

    private IEnumerator CAMERA_ROTATE()
    {
        bool relative = Convert.ToBoolean(thisRow[KEY.VALUE[1]]);
        float targetAngle;
        bool withBG = Convert.ToBoolean(thisRow[KEY.VALUE[2]]);
        int type = thisRow[KEY.EVENT_TYPE];
        float duration = BeatToDuration(thisRow[KEY.DURATION]);

        float startAngle = mainCam.transform.eulerAngles.y;
            

        if(relative == true)
        {
            targetAngle = startAngle + thisRow[KEY.VALUE[0]];
        }
        else
        {
            targetAngle = thisRow[KEY.VALUE[0]];
        }

        float t = 0;
        float p = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            p = LerpValue(t, type);

            mainCam.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(startAngle, targetAngle, p));

            yield return null;
        }
    }

    private IEnumerator SET_BG_IMAGE()
    {
        int num = thisRow[KEY.VALUE[0]];

        BackgroundManager.S.SetBgImage(Level.S.levelName, num);

        yield break;
    }
    #endregion
}
