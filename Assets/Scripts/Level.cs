using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;


public class Level : MonoBehaviour
{
    public static Level S = null;
    private void Awake()
    {
        if(S != null)
        {
            Destroy(this.gameObject);

            return;
        }

        DontDestroyOnLoad(this.gameObject);

        Level.S = this;

        judgRange = new float[JUDG.COUNT];
        this.judgRange[JUDG.PERFECT] = 66 * 0.001f; //판정 범위는 perfect를 기준으로 하여 일정 비율로 다른 판정 범위를 결정함.
        this.judgRange[JUDG.S_PERFECT] = judgRange[JUDG.PERFECT] * 0.5f;
        this.judgRange[JUDG.GOOD] = judgRange[JUDG.PERFECT] * 2;
        this.judgRange[JUDG.MISS] = judgRange[JUDG.PERFECT] * 3f;

        for (int i = 0; i < KEY.COUNT; ++i)
        {
            levelFormat[i] = -1;
        }
        levelFormat[KEY.ANGLE] = 90;
    }

    public static class PLAY_INFO
    {
        public const int STOPPED = 0;
        public const int PLAYING = 1;
        public const int WAIT = 2;
    }


    public List<Dictionary<int, float>> level = new List<Dictionary<int, float>>(); //Timing을 기반으로 하는 레벨 파일을 담아둠
    public List<Dictionary<int, int>> levelInfo = new List<Dictionary<int, int>>(); //레벨의 기본 정보들을 담아둠 (오프셋, 딜레이, BPM 등)

    public List<Note> noteList = new List<Note>();

    public Dictionary<int, float> levelFormat = new Dictionary<int, float>();

    //선택된 레벨의 이름과 난이도
    public string levelName;
    public int levelDifficulty;

    //level을 바탕으로 결정되는 수치
    public int noteCount;

    //levelInfo를 바탕으로 결정되는 수치
    public float offset;
    public float startDelay;
    public float bpm;

    public float songLength;

    public float noteSpeed;

    public float[] judgRange;

    public int isPlaying = PLAY_INFO.STOPPED;

    //public float editorStartDelay = 0.1f;

    //레벨의 이름과 난이도를 받아 레벨을 읽어옴
    public void ReadLevel(string levelName, int dif)
    {
        //레벨명에서 공백 제거
        this.levelName = RemoveSapce(levelName);
        this.levelDifficulty = dif;

        Debug.Log(this.levelName+"_"+DIF.FindName(dif));
        //레벨명_난이도 의 파일 탐색
        //string path = "Assets/Levels/" + this.levelName + "/Resources/" + this.levelName + "_" + DIF.FindName(dif) + ".csv";
        List<Dictionary<string, object>> tempLevel = CSVReader.Read(PATH.LEVELS, this.levelName, PATH.RESOURCES, this.levelName + "_" + DIF.FindName(dif) + ".csv");
        //List<Dictionary<string, object>> tempLevel = CSVReader.Read(PATH.ASSETS, PATH.LEVELS, this.levelName, PATH.RESOURCES, this.levelName + "_" + DIF.FindName(dif) + ".csv");


        level.Clear();
        noteList.Clear();
        //레벨 파일 변환
        CSVReader.ConvertDicFloat(tempLevel, ref level, KEY.FindName);

        //레벨 파일에서 필요한 값 저장
        //노트 개수 세기
        noteCount = 0;
        for(int i = 0; i < level.Count; ++i)
        {
            switch(level[i][KEY.NOTE_TYPE])
            {
                case NOTE_TYPE.TAP:
                case NOTE_TYPE.SLIDE:
                    noteCount += 1;
                    break;

                case NOTE_TYPE.DOUBLE:
                    noteCount += 2;
                    break;
            }
        }

        ReadLevelInfo();

        this.offset = levelInfo[0][INFO_KEY.OFFSET] * 0.001f;
        this.startDelay = levelInfo[0][INFO_KEY.START_DELAY] * 0.001f;
        this.bpm = levelInfo[0][INFO_KEY.BPM];
    }

    private void ReadLevelInfo()
    {
        //레벨 정보 파일 가져오기
        //List<Dictionary<string, object>> tempLevelInfo = CSVReader.Read(this.levelName + "_" + DIF.FindName(DIF.I));
        //List<Dictionary<string, object>> tempLevelInfo = CSVReader.Read(PATH.ASSETS, PATH.LEVELS, this.levelName, PATH.RESOURCES, this.levelName + "_" + DIF.FindName(DIF.I) + ".csv");
        List<Dictionary<string, object>> tempLevelInfo = CSVReader.Read(PATH.LEVELS, this.levelName, PATH.RESOURCES, this.levelName + "_" + DIF.FindName(DIF.I) + ".csv");

        //레벨 정보 파일 자료형 변환 List<Dictionary<string, object>> -> List<Dictionary<int,int>>
        //ConvertLevelInfo(tempLevelInfo);
        CSVReader.ConvertDicInt(tempLevelInfo, ref levelInfo, INFO_KEY.FindName);
    }

    private void ConvertLevelInfo(List<Dictionary<string, object>> tempLevelInfo)
    {
        //각 행을 담아둘 변수 temp 생성
        Dictionary<int, int> temp;

        for (int i = 0; i<tempLevelInfo.Count; ++i)
        {
            //새로운 행을 마주할 때 마다 temp에 새로운 메모리 할당 (얕은 복사 방지)
            temp = new Dictionary<int, int>();

            for (int j = 0; j<tempLevelInfo[0].Count; ++j)
            {
                //해당 값이 비었다면 -1로 변환하여 값을 전달. (오류 방지)
                string value = Convert.ToString(tempLevelInfo[i][INFO_KEY.FindName(j)]);
                if (value == "")
                    value = "-1";

                temp[j] = Convert.ToInt32(value);
            }
            this.levelInfo.Add(temp);
        }
    }

    public void WriteLevel()
    {
        string path = ("Assets/Levels/" + levelName + "/Resources/" + levelName + "_" + DIF.FindName(levelDifficulty)) + ".csv";
        //Debug.Log(path);
        using (var writer = new CsvFileWriter(path))
        {
            List<string> colums = new List<string>();
            string[] keyList = new string[KEY.COUNT];
            for(int i = 0; i < KEY.COUNT; ++i)
            {
                keyList[i] = KEY.FindName(i);
            }

            colums.AddRange(keyList);

            writer.WriteRow(colums);
            colums.Clear();

            for (int i = 0; i < level.Count; ++i)
            {
                for(int j = 0; j < KEY.COUNT; ++j)
                {
                    colums.Add(level[i][j].ToString());
                }

                writer.WriteRow(colums);
                colums.Clear();

                //Debug.Log("Write");
            }
        }

    }

    public static string RemoveSapce(string text)
    {
        return Regex.Replace(text, @"\s", "");
    }
}
