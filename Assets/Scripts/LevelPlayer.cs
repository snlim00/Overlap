using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] notePref;

    private EditorManager editorMgr;

    public static float timer { get; private set; } = 0;

    public AudioSource audioSource;

    //������ �����ϴ� �ڷ�ƾ��
    private bool isCorNoteTimer = false;
    private Coroutine corNoteTimer;

    // Start is called before the first frame update
    void Awake()
    {
        editorMgr = FindObjectOfType<EditorManager>();

        audioSource = GetComponent<AudioSource>();

        //ReadLevel�� ���� �̸����� ������ �����Ͽ� string���� ��ȯ, ���� �ش� ���ڿ��� �� ���� Ž��
        audioSource.clip = Resources.Load<AudioClip>(Level.S.ReadLevel("MeteorStream", DIF.X));

        Level.S.songLength = audioSource.clip.length;


        if(PlayerSetting.S.editerMode == false)
            GameStart();
    }

    public void GameStart(float startTimeRaito = 0)
    {
        float startTime = audioSource.clip.length * startTimeRaito;
        int startRow = 0;

        for (int i = 0; ; ++i)
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

            timer = startTime;

            row = startRow;
        }
        else
        {
            row = 0;
            timer = 0; 
            //startDelay ��� �� Ÿ�̸� ���� (offset�� �ش� Ÿ�̸ӿ��� �����ϸ� ���Ӱ� �̺�Ʈ�� Ÿ�ֿ̹��� ������ ��ħ -> ���� ����� ���ߴ� ������� ������ ���߱�)
            yield return new WaitForSeconds(Level.S.startDelay);
        }


        float lastNoteTiming = Level.S.level[Level.S.level.Count - 1][KEY.TIMING] * 0.001f;
        Dictionary<int, int> thisRow = Level.S.level[row];

        while (timer < lastNoteTiming && row < Level.S.level.Count)
        {
            timer += Time.deltaTime;

            if(timer >= thisRow[KEY.TIMING] * 0.001) //Ÿ�̸Ӱ� ���� ���� TIMING�� �����ϸ� ����
            {
                if(thisRow[KEY.TYPE] == TYPE.EVENT) //���� ���� EVENT��� ����
                {
                    EventExecute(thisRow);
                }

                ++row;
                thisRow = Level.S.level[row];
            }

            yield return null;
        }

        isCorNoteTimer = false;
    }

    private IEnumerator SongPlay(float startTime = 0)
    {
        if(PlayerSetting.S.editerMode == true)
        {
            audioSource.time = startTime;
        }
        else
        {
            audioSource.time = 0;
            yield return new WaitForSeconds(Level.S.startDelay);
            //Debug.Log(Level.S.startDelay);
        }

        yield return new WaitForSeconds(PlayerSetting.S.offset + Level.S.offset);

        audioSource.Play();
        //Debug.Log("audioSource Start");
    }

    private void NoteGeneration(float startTime = 0, int startRow = 0)
    {
        for(int row = startRow; row < Level.S.level.Count; ++row)
        {
            Dictionary<int, int> thisRow = Level.S.level[row];
            
            if (thisRow[KEY.TYPE] == TYPE.NOTE) //��Ʈ ����
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

    private void InstantiateNote(int row, float startTime, Dictionary<int, int> thisRow)
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
            timing = (thisRow[KEY.TIMING] * 0.001f) + Level.S.startDelay;
        }

        float spawnDis = Level.S.noteSpeed * timing;

        note.Execute(row, angle, thisRow[KEY.TIMING] * 0.001f, spawnDis, thisRow[KEY.NOTE_TYPE]);

        Level.S.noteList.Add(note);
    }

    private void EventExecute(Dictionary<int, int> thisRow)
    {
        switch (thisRow[KEY.EVENT_NAME]) //�� �̺�Ʈ ȣ��
        {
            case EVENT_TYPE.SET_SPEED:
                {
                    float speed = thisRow[KEY.VALUE[0]];
                    SetSpeed(speed);
                }
                break;
        }
    }

    private void SetSpeed(float speed)
    {
        Level.S.noteSpeed = speed;
    }
}
