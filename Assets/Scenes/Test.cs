using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*������� �ִ� ����
 * �̸�
 * ����
 * Ű
 * ������
 */

public class Human
{
    public string name;
    public int age;
    protected float height;
    protected float weight;

    virtual public void WakeUp()
    {
        Debug.Log("�Ͼ��");
        Debug.Log("�ı�");
        Debug.Log("��ħ�� �Ա�");
    }
}

public class Student : Human
{
    public string school;
    public int grade;
    public int _class;

    public void Introduce()
    {
        Debug.Log(school + "�� �ٴϴ� " + grade + "�г� " + _class + "�� " + name + "�Դϴ�");
    }

    override public void WakeUp()
    {
        base.WakeUp();

        Debug.Log("��ϱ�");
    }
}

public class BusinessMan : Human
{
    override public void WakeUp()
    {
        base.WakeUp();

        Debug.Log("����ϱ�");
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
