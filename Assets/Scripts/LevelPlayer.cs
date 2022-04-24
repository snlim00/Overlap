using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] notePref;

    private EditorManager editorMgr;

    public double t;

    private AudioSource audioSource;

    private bool didEditorMgrInit = false;

    // Start is called before the first frame update
    void Awake()
    {
        editorMgr = FindObjectOfType<EditorManager>();

        audioSource = GetComponent<AudioSource>();

        //ReadLevel이 곡의 이름에서 공백을 제거하여 string으로 반환, 이후 해당 문자열로 곡 파일 탐색
        audioSource.clip = Resources.Load<AudioClip>(Level.S.ReadLevel("MeteorStream", DIF.X));

        Level.S.songLength = audioSource.clip.length;


        NoteGeneration();

        GameStart();
    }

    void Start()
    {
        
    }

    private void GameStart()
    {
        StartCoroutine(SongPlay());

        StartCoroutine(NoteTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator NoteTimer()
    {
        //startDelay 대기 후 타이머 실행 (offset을 해당 타이머에서 적용하면 변속과 이벤트의 타이밍에도 영향을 미침)
        yield return new WaitForSeconds(Level.S.startDelay);

        audioSource.Play();

        t = 0;
        int row = 0;
        float lastNoteTiming = Level.S.level[Level.S.level.Count - 1][KEY.TIMING] * 0.001f;
        Dictionary<int, int> thisRow = Level.S.level[row];

        while (t < lastNoteTiming)
        {
            t += Time.deltaTime;

            if(t >= thisRow[KEY.TIMING] * 0.001) //타이머가 다음 행의 TIMING에 도달하면 실행
            {
                if(thisRow[KEY.TYPE] == TYPE.EVENT) //다음 행이 EVENT라면 실행
                {
                    EventExecute(thisRow);
                }

                ++row;
                thisRow = Level.S.level[row];
            }

            yield return null;
        }
    }

    IEnumerator SongPlay()
    {
        yield return new WaitForSeconds(PlayerSetting.S.offset + Level.S.offset);

        audioSource.Play();
    }

    void NoteGeneration()
    {
        for(int row = 0; row < Level.S.level.Count; ++row)
        {
            Dictionary<int, int> thisRow = Level.S.level[row];
            
            if (thisRow[KEY.TYPE] == TYPE.NOTE) //노트 생성
            {
                Note note = null;

                switch (thisRow[KEY.NOTE_TYPE])
                {
                    case NOTE_TYPE.TAP:
                        {
                            note = Instantiate(notePref[NOTE_TYPE.TAP]).GetComponent<Note>();

                            int angle = thisRow[KEY.ANGLE];
                            float timing = (thisRow[KEY.TIMING] * 0.001f) + Level.S.startDelay;
                            float spawnDis = Level.S.noteSpeed * timing;

                            note.num = row;
                            note.Execute(angle, thisRow[KEY.TIMING] * 0.001f, spawnDis);
                        }
                        break;
                }

                Level.S.noteList.Add(note);
            }
        }
    }

    void EventExecute(Dictionary<int, int> thisRow)
    {
        switch (thisRow[KEY.EVENT_TYPE]) //각 이벤트 호출
        {
            case EVENT_TYPE.SET_SPEED:
                {
                    float speed = thisRow[KEY.VALUE[0]];
                    SetSpeed(speed);
                }
                break;
        }
    }

    void SetSpeed(float speed)
    {
        Level.S.noteSpeed = speed;
    }
}
