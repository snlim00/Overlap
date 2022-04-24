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
                Debug.LogError("FindValue: 해당 이름을 가진 변수를 찾을 수 없습니다.");
                return -1;
        }
    }
}

public interface A
{

}

public static class KEY
{
    public const int TIMING = 0;
    public const int TYPE = 1;
    public const int NOTE_TYPE = 2;
    public const int ANGLE = 3;
    public const int EVENT_TYPE = 4;
    public const int DURATION = 5;
    public static readonly int[] VALUE = { 6, 7, 8, 9, 10 };

    public const int COUNT = 11;

    public static readonly int[] KEY_TYPE = { 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0 };

    public static string FindName(int value)
    {
        switch (value)
        {
            case TIMING:
                return nameof(TIMING);

            case TYPE:
                return nameof(TYPE);

            case NOTE_TYPE:
                return nameof(NOTE_TYPE);

            case ANGLE:
                return nameof(ANGLE);

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
    public const int TAP = 0;
    public const int DOUBLE = 1;
    public const int SLIDE = 2;

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

public static class EVENT_TYPE
{
    public const int SET_SPEED = 0;

    public const int COUNT = 1;

    public static string FindName(int value)
    {
        switch (value)
        {
            case SET_SPEED:
                return nameof(SET_SPEED);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
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
    public const int DIFFICULTY = 5;

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

            case DIFFICULTY:
                return nameof(DIFFICULTY);

            default:
                Debug.LogError("FindName: 해당 값을 가진 변수를 찾을 수 없습니다.");
                return "";
        }
    }
}