using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*사람에게 있는 정보
 * 이름
 * 나이
 * 키
 * 몸무게
 */

public class Human
{
    public string name;
    public int age;
    protected float height;
    protected float weight;

    virtual public void WakeUp()
    {
        Debug.Log("일어나기");
        Debug.Log("씻기");
        Debug.Log("아침밥 먹기");
    }
}

public class Student : Human
{
    public string school;
    public int grade;
    public int _class;

    public void Introduce()
    {
        Debug.Log(school + "에 다니는 " + grade + "학년 " + _class + "반 " + name + "입니다");
    }

    override public void WakeUp()
    {
        base.WakeUp();

        Debug.Log("등교하기");
    }
}

public class BusinessMan : Human
{
    override public void WakeUp()
    {
        base.WakeUp();

        Debug.Log("출근하기");
    }
}

public class Test : MonoBehaviour
{
    BusinessMan choiUiMan = new BusinessMan();

    // Start is called before the first frame update
    void Start()
    {
        choiUiMan.WakeUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
