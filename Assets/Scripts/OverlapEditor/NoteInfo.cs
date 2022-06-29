using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class NoteInfo
{
    public int type;

    public GameObject gameObject;

    private Text infoName;

    private InputField inputField;
    private Dropdown dropdown;

    public void InitInfo(GameObject info, int num, UnityAction<int> onValueChanged)
    { 
        this.type = KEY.KEY_TYPE[num];

        switch(KEY.KEY_TYPE[num])
        {
            case NOTE_INFO_TYPE.INPUT_FIELD:
                inputField = info.transform.GetChild(1).GetComponent<InputField>();

                inputField.contentType = InputField.ContentType.DecimalNumber;

                inputField.onEndEdit.AddListener((data) => { TLNoteManager.SetEditInputFieldNow(false); });


                EventTrigger.Entry entry_Select = new EventTrigger.Entry();
                entry_Select.eventID = EventTriggerType.Select;
                entry_Select.callback.AddListener((data) => { TLNoteManager.SetEditInputFieldNow(true); });
                inputField.GetComponent<EventTrigger>().triggers.Add(entry_Select);
                
                break;

            case NOTE_INFO_TYPE.DROPDOWN:

                dropdown = info.transform.GetChild(1).GetComponent<Dropdown>();

                dropdown.onValueChanged.AddListener(onValueChanged);
                

                dropdown.ClearOptions();

                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                options.Add(new Dropdown.OptionData("none"));

                for (int i = 0; i < EVENT_NAME.COUNT; ++i)
                {
                    options.Add(new Dropdown.OptionData(EVENT_NAME.FindName(i)));
                }

                dropdown.AddOptions(options);

                break;
        }

        infoName = info.transform.GetChild(0).GetComponent<Text>();
        infoName.text = KEY.FindName(num);

        gameObject = info;
    }

    public void SetInfo(float value)
    {
        if(type == 0)
        {
            inputField.text = value.ToString();
        }
        else
        {
            dropdown.value = (int)value + 1;
        }
    }

    public float GetInfo()
    {
        if(type == 0)
        {
            return (float)Convert.ToDouble(inputField.text);
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