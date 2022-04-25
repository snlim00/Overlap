using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineNote : MonoBehaviour
{
    public bool isSelected = false;

    public int num;

    public Dictionary<int, int> info = new Dictionary<int, int>();

    [HideInInspector] public Image image;

    public static Color32 subSelectColor = new Color32(255, 83, 83, 180);
    public static Color32 stdSelectColor = Color.red;
    public static Color32 defaultColor = Color.white;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Setting(Dictionary<int, int> info)
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
