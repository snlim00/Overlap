using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineNote : MonoBehaviour
{
    public bool isSelected = false;

    public int num;

    public Dictionary<int, float> info;

    [HideInInspector] public Image image;

    public static Color32 subSelectColor = new Color32(255, 83, 83, 180);
    public static Color32 stdSelectColor = Color.red;
    private Color32 defaultColor;

    void Awake()
    {
        image = GetComponent<Image>();

        defaultColor = image.color;
    }

    public void Setting(Dictionary<int, float> info)
    {
        this.info = info;
    }

    public void Select()
    {
        //Debug.Log(name + " Select");
        isSelected = true;

        image.color = subSelectColor;
    }
    
    public void Deselect()
    {
        //Debug.Log(name + " Deselect");
        isSelected = false;

        image.color = defaultColor;
    }

    public void SetStandardNote()
    {
        image.color = stdSelectColor;
    }

    //StandardNote및 선택 상태 해제
    public void UnsetStandardNote()
    {
        Deselect();
    }

    //StandardNote만 해제, 선택 상태는 유지
    public void ReleaseStandardNote()
    {
        Select();
    }
}
