using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(params string[] path)
    {
        //string filePath = PATH.ASSETS;
        string filePath = Application.dataPath;

        for(int i = 0; i < path.Length; ++i)
        {
            filePath += "/" + path[i];
        }

        StreamReader file = new StreamReader(filePath);
        //Debug.Log(filePath);

        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(file.ReadToEnd(), LINE_SPLIT_RE);

        file.Close();

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                value = value.Replace("<br>", "\n"); // 추가된 부분. 개행문자를 \n대신 <br>로 사용한다.
                value = value.Replace("<c>", ",");

                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public delegate string A(int a);
    public static void ConvertDicInt(List<Dictionary<string, object>> dic, ref List<Dictionary<int, int>> original, A FindName)
    {
        Dictionary<int, int> temp;

        for (int i = 0; i < dic.Count; ++i)
        {
            temp = new Dictionary<int, int>();

            for (int j = 0; j < dic[0].Count; ++j)
            {
                int value;
                try
                {
                    value = Convert.ToInt32(dic[i][FindName(j)]);
                }
                catch
                {
                    value = -1;
                }

                temp[j] = value;
            }

            original.Add(temp);
        }
    }

    public static List<Dictionary<int, string>> ConvertDicString(List<Dictionary<string, object>> dic, A FindName)
    {
        List<Dictionary<int, string>> result = new List<Dictionary<int, string>>();

        Dictionary<int, string> temp;

        for (int i = 0; i < dic.Count; ++i)
        {
            temp = new Dictionary<int, string>();

            for (int j = 0; j < dic[0].Count; ++j)
            {
                string value;
                try
                {
                    value = Convert.ToString(dic[i][FindName(j)]);
                }
                catch
                {
                    value = "";
                }

                temp[j] = value;
            }

            result.Add(temp);
        }

        return result;
    }
}