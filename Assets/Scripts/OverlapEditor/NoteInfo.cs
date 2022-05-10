using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[Serializable]
public class NoteInfo
{
    public int type;

    public GameObject gameObject;

    private Text infoName;

    private InputField inputField;
    private Dropdown dropdown;

    public void InitInfo(GameObject info, int num, UnityAction<string> onEndEdit, UnityAction<int> onValueChange)
    {
        switch(KEY.KEY_TYPE[num])
        {
            case NOTE_INFO_TYPE.INPUT_FIELD:
                inputField = info.transform.GetChild(1).GetComponent<InputField>();

                //inputField.onEndEdit.AddListener(onEndEdit);

                inputField.contentType = InputField.ContentType.IntegerNumber;
                break;

            case NOTE_INFO_TYPE.DROPDOWN:
                dropdown = info.transform.GetChild(1).GetComponent<Dropdown>();

                //dropdown.onValueChanged.AddListener(onValueChange);

                //dropdown.ClearOptions();

                //List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                //dropdown.AddOptions(options);

                break;
        }
        this.type = KEY.KEY_TYPE[num];

        infoName = info.transform.GetChild(0).GetComponent<Text>();
        infoName.text = KEY.FindName(num);

        gameObject = info;
    }

    public void SetInfo(int value)
    {
        if(type == 0)
        {
            inputField.text = value.ToString();
        }
        else
        {
            dropdown.value = value + 1;
        }
    }

    public int GetInfo()
    {
        if(type == 0)
        {
            return Convert.ToInt32(inputField.text);
        }
        else
        {
            return dropdown.value - 1;
        }
    }

    public void SetName(string name)
    {
        infoName.text = name;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}