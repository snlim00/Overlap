using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TAG
{
    public const string NOTE = "Note";
}

public static class JUDG
{
    public const int PERFECT = 0;
    public const int GOOD = 1;
    public const int MISS = 2;
}

public static class DIF
{
    public const int E = 0;
    public const int N = 1;
    public const int H = 2;
    public const int X = 3;
    public const int I = 4;

    public static string FindName(int value)
    {
        switch(value)
        {
            case E:
                return nameof(E);

            case N:
                return nameof(N);

            case H:
                return nameof(H);

            case X:
                return nameof(X);

            case I:
                return nameof(I);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }
    }

    public static int FindValue(string name)
    {
        switch(name)
        {
            case nameof(E):
                return E;

            case nameof(N):
                return N;

            case nameof(H):
                return H;

            case nameof(X):
                return X;

            case nameof(I):
                return I;

            default:
                Debug.LogError("FindValue: 해당 이름을 가진 변수를 찾을 수 없습니다." + name);
                return -1;
        }
    }
}

public static class KEY
{
    public const int TIMING = 0;
    public const int GRID_NUM = 1;
    public const int TYPE = 2;
    public const int NOTE_TYPE = 3;
    public const int ANGLE = 4;
    public const int EVENT_NAME = 5;
    public const int DURATION = 6;
    public const int EVENT_TYPE = 7;
    public static readonly int[] VALUE = {  8, 9, 10, 11, 12 };

    public static readonly int[] KEY_TYPE = { 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
    public const int COUNT = 13;

    public static string FindName(int value)
    {
        switch (value)
        {
            case TIMING:
                return nameof(TIMING);

            case TYPE:
                return nameof(TYPE);

            case GRID_NUM:
                return nameof(GRID_NUM);


            case NOTE_TYPE:
                return nameof(NOTE_TYPE);

            case ANGLE:
                return nameof(ANGLE);

            case EVENT_NAME:
                return nameof(EVENT_NAME);

            case EVENT_TYPE:
                return nameof(EVENT_TYPE);

            case DURATION:
                return nameof(DURATION);

            default:
                for(int i = 0; i < VALUE.Length; ++i)
                {
                    if(VALUE[i] == value)
                    {
                        return nameof(VALUE) + i;
                    }
                }

                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }
    }
}

public static class TYPE
{
    public const int NOTE = 0;
    public const int EVENT = 1;

    public const int COUNT = 2;

    public static string FindName(int value)
    {
        switch (value)
        {
            case NOTE:
                return nameof(NOTE);

            case EVENT:
                return nameof(EVENT);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }

    }

    public static int FindValue(string name)
    {
        switch (name)
        {
            case nameof(NOTE):
                return NOTE;

            case nameof(EVENT):
                return EVENT;

            default:
                Debug.LogError("FindValue: 해당 이름을 가진 변수를 찾을 수 없습니다.");
                return -1;
        }
    }
}

public static class NOTE_TYPE
{
    public const int NONE = -1;
    public const int TAP = 0;
    public const int DOUBLE = 1;
    public const int SLIDE = 2;
    public const int EVENT = 3;

    public const int COUNT = 3;

    public static string FindName(int value)
    {
        switch (value)
        {
            case TAP:
                return nameof(TAP);

            case DOUBLE:
                return nameof(DOUBLE);

            case SLIDE:
                return nameof(SLIDE);

            case EVENT:
                return nameof(EVENT);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }

    }

    public static int FindValue(string name)
    {
        switch (name)
        {
            case nameof(TAP):
                return TAP;

            case nameof(DOUBLE):
                return DOUBLE;

            case nameof(SLIDE):
                return SLIDE;

            default:
                Debug.LogError("FindValue: 해당 이름을 가진 변수를 찾을 수 없습니다.");
                return -1;
        }
    }
}

public static class EVENT_NAME
{
    public static List<Dictionary<int, string>> VALUES = new List<Dictionary<int, string>>();

    public const int NONE = -1;

    public const int SET_SPEED = 0;

    public const int CAMERA_MOVE = 1;

    public const int CAMERA_ZOOM = 2;

    public const int CAMERA_ROTATE = 3;

    public const int SET_BG_IMAGE = 4;

    public const int SET_BG_SCALE = 5;

    public const int COUNT = 6;

    public static void ReadEventTypeName()
    {
        List<Dictionary<string, object>> tempValues = CSVReader.Read("EventValues");

        for(int i = 0; i < tempValues[0].Count; ++i)
        {
            Dictionary<int, string> temp = new Dictionary<int, string>();

            for(int j = 0; j < KEY.VALUE.Length; ++j)
            {
                temp[j] = tempValues[j][FindName(i)].ToString();
                //Debug.Log(temp[i]);
            }

            VALUES.Add(temp);
        }

        //for(int i = 0; i < VALUES.Count; ++i)
        //{
        //    for(int j = 0; j < KEY.VALUE.Length; ++j)
        //    {
        //        Debug.Log(VALUES[i][j]);
        //    }
        //}
    }

    public static string FindName(int value)
    {
        switch (value)
        {
            case SET_SPEED:
                return nameof(SET_SPEED);

            case CAMERA_MOVE:
                return nameof(CAMERA_MOVE);

            case CAMERA_ZOOM:
                return nameof(CAMERA_ZOOM);

            case CAMERA_ROTATE:
                return nameof(CAMERA_ROTATE);

            case SET_BG_IMAGE:
                return nameof(SET_BG_IMAGE);

            case SET_BG_SCALE:
                return nameof(SET_BG_SCALE);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다." + value);
                return "";
        }
    }

    public static int FindValue(string name)
    {
        switch (name)
        {
            case nameof(SET_SPEED):
                return SET_SPEED;

            default:
                Debug.LogError("FindValue: 해당 이름을 가진 변수를 찾을 수 없습니다.");
                return -1;
        }
    }
}

public static class INFO_KEY
{
    public const int OFFSET = 0;
    public const int START_DELAY = 1;
    public const int BPM = 2;
    public const int NOTE_SPEED = 3;
    public const int JUDG_RANGE = 4;

    public static string FindName(int value)
    {
        switch (value)
        {
            case OFFSET:
                return nameof(OFFSET);

            case START_DELAY:
                return nameof(START_DELAY);

            case BPM:
                return nameof(BPM);

            case NOTE_SPEED:
                return nameof(NOTE_SPEED);

            case JUDG_RANGE:
                return nameof(JUDG_RANGE);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }
    }
}

public static class NOTE_INFO_TYPE
{
    public const int INPUT_FIELD = 0;
    public const int DROPDOWN = 1;
}

public static class SONG_LIST_KEY
{
    public const int SONG_NAME = 0;

    public const int ARTIST = 1;

    public const int E = 2, N = 3, H = 4, X = 5;

    public const int E_SCORE = 6, N_SCORE = 7, H_SCORE = 8, X_SCORE = 9;

    public const int E_RATE = 10, N_RATE = 11, H_RATE = 12, X_RATE = 13;

    public const int E_COMBO = 14, N_COMBO = 15, H_COMBO = 16, X_COMBO = 17;

    public const int COUNT = 18;

    public static string FindName(int value)
    {
        switch (value)
        {
            case SONG_NAME:
                return nameof(SONG_NAME);

            case ARTIST:
                return nameof(ARTIST);

            case E:
                return nameof(E);

            case N:
                return nameof(N);

            case H:
                return nameof(H);

            case X:
                return nameof(X);

            case E_SCORE:
                return nameof(E_SCORE);

            case N_SCORE:
                return nameof(N_SCORE);

            case H_SCORE:
                return nameof(H_SCORE);

            case X_SCORE:
                return nameof(X_SCORE);

            case E_RATE:
                return nameof(E_RATE);

            case N_RATE:
                return nameof(N_RATE);

            case H_RATE:
                return nameof(H_RATE);

            case X_RATE:
                return nameof(X_RATE);

            case E_COMBO:
                return nameof(E_COMBO);

            case N_COMBO:
                return nameof(N_COMBO);

            case H_COMBO:
                return nameof(H_COMBO);

            case X_COMBO:
                return nameof(X_COMBO);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다." + value);
                return "";
        }
    }

    public static int FindValue(string name)
    {
        switch (name)
        {
            case nameof(SONG_NAME):
                return SONG_NAME;

            case nameof(ARTIST):
                return ARTIST;

            case nameof(E):
                return E;

            case nameof(N):
                return N;

            case nameof(H):
                return H;

            case nameof(X):
                return X;

            case nameof(E_SCORE):
                return E_SCORE;

            case nameof(N_SCORE):
                return N_SCORE;

            case nameof(H_SCORE):
                return H_SCORE;

            case nameof(X_SCORE):
                return X_SCORE;

            case nameof(E_RATE):
                return E_RATE;

            case nameof(N_RATE):
                return N_RATE;

            case nameof(H_RATE):
                return H_RATE;

            case nameof(X_RATE):
                return X_RATE;

            case nameof(E_COMBO):
                return E_COMBO;

            case nameof(N_COMBO):
                return N_COMBO;

            case nameof(H_COMBO):
                return H_COMBO;

            case nameof(X_COMBO):
                return X_COMBO;

            default:
                Debug.LogError("FindValue: 해당 값을 가진 변수를 찾을 수 없습니다." + name);
                return -1;
        }
    }
}

public static class SCENE
{
    public const string MAIN_MENU = "MainMenu";
    public const string GAME_SCENE = "GameScene";
    public const string RESULT_SCENE = "ResultScene";
}