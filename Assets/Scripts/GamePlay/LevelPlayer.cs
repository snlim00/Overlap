using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] notePref;

    [SerializeField] private TMP_Text songNameText;
    [SerializeField] private TMP_Text difText;

    public AudioSource audioSource;

    private Camera mainCam;
    private const float camSize = 5;


    //게임을 진행하는 코루틴들
    private bool isCorNoteTimer = false;
    private Coroutine corNoteTimer;

    public static float timer { get; private set; } = 0;
    public float timerForDebug;
    public static float startTime;
    public Dictionary<int, float> thisRow;

    private Dictionary<int, float> _thisRow;

    public float playStartTime;

    // Start is called before the first frame update
    void Awake()
    {
        playStartTime = Time.time;

        audioSource = GetComponent<AudioSource>();

        mainCam = Camera.main;

        //ReadLevel이 곡의 이름에서 공백을 제거하여 string으로 반환, 이후 해당 문자열로 곡 파일 탐색
        //audioSource.clip = Resources.Load<AudioClip>(Level.S.ReadLevel("MeteorStream", DIF.X));

        audioSource.clip = Resources.Load<AudioClip>(Level.S.levelName);
        Debug.Log(Level.S.levelName);
        Level.S.songLength = audioSource.clip.length;


        if (PlayerSetting.S.editorMode == false)
            GameStart();
    }

    void Start()
    {
        songNameText.text = SongListManager.songList[SongListManager.selectedSongNum][SONG_LIST_KEY.SONG_NAME];
        difText.text = DIF.NAME[Level.S.levelDifficulty];
        difText.color = DIF.COLOR[Level.S.levelDifficulty];
    }

    public void GameStart(float startTimeRaito = 0)
    {
        BackgroundManager.S.SetBgImage(Level.S.levelName, 0);

        float startTime = audioSource.clip.length * startTimeRaito;
        //Debug.Log("startTime: " + startTime);
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

        playStartTime = Time.time - startTime;
        timer = Time.time - playStartTime - Level.S.startDelay;

        corNoteTimer = StartCoroutine(NoteTimer(startTime, startRow));
        StartCoroutine(SongPlay(startTime));

        NoteGeneration(startRow);
    }

    public void GameStop()
    {
        audioSource.Stop();

        if (isCorNoteTimer == true)
        {
            isCorNoteTimer = false;
            StopCoroutine(corNoteTimer);
        }


        for (int i = 0; i < Level.S.noteList.Count; ++i)
        {
            if (Level.S.noteList[i] != null)
                Destroy(Level.S.noteList[i].gameObject);
        }
        Level.S.noteList.Clear();
    }

    private IEnumerator NoteTimer(float startTime = 0, int startRow = 0)
    {
        isCorNoteTimer = true;

        int row = 0;


        if (PlayerSetting.S.editorMode == true)
        {
            if (startRow >= Level.S.level.Count)
                yield break;


            //yield return new WaitForSecondsRealtime(Level.S.editorStartDelay);

            //Debug.Log(startTime);

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

        timer = Time.time - playStartTime - Level.S.startDelay;

        thisRow = Level.S.level[row];

        while (timer < audioSource.clip.length)// && row < Level.S.level.Count)
        {
            //timer += Time.deltaTime;
            timer = Time.time - playStartTime - Level.S.startDelay;

            if (PlayerSetting.S.editorMode == true)
                timer += Level.S.startDelay; // 마이너스 한 Level.S.startDelay 상쇄.

            timerForDebug = timer;

            if (row < Level.S.level.Count - 1)
            {
                if (thisRow[KEY.TYPE] == TYPE.EVENT)
                {
                    if (timer >= thisRow[KEY.TIMING] * 0.001) //타이머가 다음 행의 TIMING에 도달하면 실행
                    {
                        EventExecute(thisRow);
                        thisRow = Level.S.level[++row];
                    }
                }
                else
                {
                    thisRow = Level.S.level[++row];
                }
            }

            //if (timer >= thisRow[KEY.TIMING] * 0.001) //타이머가 다음 행의 TIMING에 도달하면 실행
            //{
            //    thisRow = Level.S.level[row];
            //    Debug.Log("Hi");
            //    if (thisRow[KEY.TYPE] == TYPE.EVENT) //다음 행이 EVENT라면 실행
            //    {
            //        EventExecute();
            //    }

            //    ++row;
            //}

            yield return null;
        }

        Debug.Log("END");
        SceneMgr.S.ResultScene();
        isCorNoteTimer = false;
    }

    private IEnumerator SongPlay(float startTime = 0)
    {
        if (PlayerSetting.S.editorMode == true)
        {
            audioSource.time = startTime;
            //yield return new WaitForSecondsRealtime(Level.S.editorStartDelay);
            yield return new WaitForSeconds(PlayerSetting.S.songOffset);
        }
        else
        {
            audioSource.time = 0;
            yield return new WaitForSeconds(Level.S.startDelay + PlayerSetting.S.songOffset);
            //Debug.Log(Level.S.startDelay);
        }
        //Debug.Log("Start Song: " + Time.time);
        //audioSource.time = Mathf.Min(audioSource.time, audioSource.clip.length - 0.01f);
        //Debug.Log(audioSource.time);
        audioSource.Play();
        //Debug.Log("audioSource Start");
    }

    private void NoteGeneration(int startRow = 0)
    {
        //Debug.Log("NoteGen");

        //Debug.Log("startRow: " + startRow);
        for (int row = startRow; row < Level.S.level.Count; ++row)
        {
            Dictionary<int, float> thisRow = Level.S.level[row];

            if (thisRow[KEY.TYPE] == TYPE.NOTE) //노트 생성
            {
                if (thisRow[KEY.NOTE_TYPE] == NOTE_TYPE.DOUBLE)
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        InstantiateNote(row, thisRow);
                    }
                }
                else if (thisRow[KEY.TYPE] != NOTE_TYPE.EVENT)
                {
                    InstantiateNote(row, thisRow);
                }
            }
        }
    }

    private void InstantiateNote(int row, in Dictionary<int, float> thisRow)
    {
        //Debug.Log("NG");
        Note note;

        note = Instantiate(notePref[(int)thisRow[KEY.NOTE_TYPE]]).GetComponent<Note>();

        int angle = (int)thisRow[KEY.ANGLE];

        if (angle > 0)
        {
            angle = angle % 360;
        }
        else
        {
            angle = 360 + (angle % -360);
        }

        note.Execute(row, angle, thisRow[KEY.TIMING] * 0.001f, (int)thisRow[KEY.NOTE_TYPE]);

        Level.S.noteList.Add(note);
    }

    private void EventExecute(Dictionary<int, float> thisRow)
    {
        //StartCoroutine(EVENT_NAME.FindName((int)thisRow[KEY.EVENT_NAME]));

        _thisRow = thisRow;
        Invoke(EVENT_NAME.FindName((int)thisRow[KEY.EVENT_NAME]), 0);

        //switch (thisRow[KEY.EVENT_NAME]) //각 이벤트 호출
        //{
        //    case EVENT_NAME.SET_SPEED:
        //        SetSpeed(thisRow);
        //        break;
        //}
    }

    private float BeatToDuration(float beat)
    {
        float duration = (60f / Level.S.bpm) / beat;

        return duration;
    }

    #region 레벨 이벤트 함수
    private void SET_SPEED()
    {
        int speed = (int)_thisRow[KEY.VALUE[0]];

        Level.S.noteSpeed = speed;
    }

    private void CAMERA_MOVE()
    {
        Vector3 targetPos = new Vector3(_thisRow[KEY.VALUE[0]], _thisRow[KEY.VALUE[1]], -10);
        bool withBG = Convert.ToBoolean(_thisRow[KEY.VALUE[2]]);
        float duration = BeatToDuration(_thisRow[KEY.DURATION]);
        int type = (int)_thisRow[KEY.EVENT_TYPE];
        StartCoroutine(_CameraMove(targetPos, withBG, type, duration));

        //Debug.Log("cammove");
    }

    private IEnumerator _CameraMove(Vector3 targetPos, bool withBG, int type, float duration)
    {
        Vector3 startPos = mainCam.transform.position;
        Vector3 bgPos;
        float t = 0;
        float p = 0;

        while (t <= 1)
        {
            t += Time.deltaTime / duration;

            p = Utility.LerpValue(t, type);
            //Debug.Log(p);

            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, p);


            if (withBG == true)
            {
                bgPos = mainCam.transform.position;
                bgPos.z = 0;

                BackgroundManager.S.transform.position = bgPos;
            }

            yield return null;
        }

        //Debug.Log("routine end");
    }

    private void CAMERA_ZOOM()
    {
        float targetScale = _thisRow[KEY.VALUE[0]];
        bool withBG = Convert.ToBoolean(_thisRow[KEY.VALUE[1]]);
        int type = (int)_thisRow[KEY.EVENT_TYPE];
        //Debug.Log(type);
        //Debug.Log(_thisRow[KEY.DURATION]);
        float duration = BeatToDuration(_thisRow[KEY.DURATION]);

        StartCoroutine(_CameraZoom(targetScale, withBG, type, duration));
    }

    private IEnumerator _CameraZoom(float targetScale, bool withBG, int type, float duration)
    {
        targetScale = camSize * (targetScale / 100f);
        float startScale = mainCam.orthographicSize;

        //Vector2 bgScale = BackgroundManager.S.transform.localScale;
        //Vector2 targetBgScale = BackgroundManager.S.transform.localScale * (_thisRow[KEY.VALUE[0]] / 100f);
        float t = 0;
        float p = 0;

        //Debug.Log(duration);

        while (t <= 1)
        {
            t += Time.deltaTime / duration;

            p = Utility.LerpValue(t, type);

            //Debug.Log(t);

            mainCam.orthographicSize = Mathf.Lerp(startScale, targetScale, p);

            //BackgroundManager.S.transform.localScale = Vector2.Lerp(bgScale, targetBgScale, p);

            yield return null;
        }
    }

    private void CAMERA_ROTATE()
    {
        float targetAngle;
        bool relative = Convert.ToBoolean(_thisRow[KEY.VALUE[1]]);
        bool withBG = Convert.ToBoolean(_thisRow[KEY.VALUE[2]]);

        int type = (int)_thisRow[KEY.EVENT_TYPE];
        float duration = BeatToDuration(_thisRow[KEY.DURATION]);

        float startAngle = mainCam.transform.eulerAngles.y;

        if (relative == true)
        {
            targetAngle = startAngle + _thisRow[KEY.VALUE[0]];
        }
        else
        {
            targetAngle = _thisRow[KEY.VALUE[0]];
        }

        StartCoroutine(_CameraRotate(targetAngle, withBG, type, startAngle, duration));
    }

    private IEnumerator _CameraRotate(float targetAngle, bool withBG, int type, float startAngle, float duration)
    {
        float t = 0;
        float p = 0;

        while (t <= 1)
        {
            t += Time.deltaTime / duration;

            p = Utility.LerpValue(t, type);

            mainCam.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(startAngle, targetAngle, p));


            if (withBG == true)
            {
                BackgroundManager.S.transform.eulerAngles = mainCam.transform.eulerAngles;
            }

            yield return null;
        }
    }

    private void SET_BG_IMAGE()
    {
        int num = (int)_thisRow[KEY.VALUE[0]];

        BackgroundManager.S.SetBgImage(Level.S.levelName, num);
    }

    private void SET_BG_SCALE()
    {
        float scale = _thisRow[KEY.VALUE[0]];
        int type = (int)_thisRow[KEY.EVENT_TYPE];
        float duration = BeatToDuration(_thisRow[KEY.DURATION]);

        StartCoroutine(_SetBgScale(scale, type, duration));
    }

    private IEnumerator _SetBgScale(float scale, int type, float duration)
    {
        float t = 0;

        Vector2 startScale = BackgroundManager.S.transform.localScale * 100;

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            BackgroundManager.S.SetBgScale(Mathf.Lerp(startScale.x, scale, Utility.LerpValue(t, type)));

            yield return null;
        }
    }

    private void BIT_CAM()
    {
        float interval = BeatToDuration(_thisRow[KEY.VALUE[0]]);
        float zoomScale = _thisRow[KEY.VALUE[1]];
        float rotateScale = _thisRow[KEY.VALUE[2]];
        bool rotationRotate = Convert.ToBoolean(_thisRow[KEY.VALUE[3]]);
        float effectDuration = BeatToDuration(_thisRow[KEY.VALUE[4]]);

        //Debug.Log(_thisRow[KEY.DURATION]);
        float duration = BeatToDuration(_thisRow[KEY.DURATION]);

        StartCoroutine(_BitCam(interval, zoomScale, rotateScale, rotationRotate, effectDuration, duration));
    }

    private IEnumerator _BitCam(float interval, float zoomScale, float rotateScale, bool rotationRotate, float effectDuration, float duration)
    {
        float t1 = 0;
        float t2 = 1;

        int rotDir = 1;

        while (t1 <= 1)
        {
            t1 += Time.deltaTime / duration;
            t2 += Time.deltaTime / interval;

            if (t2 >= 1)
            {
                if(rotationRotate == true)
                    rotDir *= -1;

                t2 -= 1;
                StartCoroutine(_CameraZoom(zoomScale, false, 2, effectDuration));
                StartCoroutine(_CameraRotate(mainCam.transform.eulerAngles.z + (rotateScale * rotDir), false, 2, mainCam.transform.eulerAngles.z, effectDuration));
            }

            yield return null;
        }
    }
    #endregion
}
