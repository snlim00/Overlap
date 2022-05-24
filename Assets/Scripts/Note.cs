using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int num;

    public int type;

    protected int angle;
    public float timing;

    protected bool doMove = false;

    private Vector2 startPos;

    private static float spawnDis = 100;

    public void Execute(int num, int angle, float timing, int type)
    {
        this.num = num;
        this.angle = -(angle + 90);
        this.timing = timing;
        this.type = type;

        transform.position = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, this.angle);
        transform.Translate(-spawnDis, 0, 0);

        startPos = transform.position.normalized;
    }
    //Ǯ�� ��� �� �Լ�
    //public void Execute(int num, int angle, float timing, float spawnDis, int type)
    //{
    //    this.num = num;
    //    this.angle = -(angle + 90);
    //    this.timing = timing;
    //    this.type = type;

    //    transform.position = Vector3.zero;
    //    transform.eulerAngles = new Vector3(0, 0, this.angle);
    //    transform.Translate(-spawnDis, 0, 0);

    //    startPos = transform.position.normalized;
    //}

    // Update is called once per frame

    void Update()
    {
        Move();
    }

    protected void Move()
    {
        //��Ʈ�� 20�� �̳��� �����ؾ� �ϴ� ��Ȳ�� �Ǹ� �����̱� ����
        if (timing <= LevelPlayer.timer + 5)
            doMove = true;

        if (doMove == false)
            return;

        transform.position = startPos * (timing - LevelPlayer.timer) * Level.S.noteSpeed;

        //MISS �������� ������ ����� �׳� MISSó��
        if (timing <= LevelPlayer.timer - Level.S.judgRange[JUDG.MISS])
        {
            Clear(JUDG.MISS);
        }
    }

    public void Clear(int judg)
    {
        GameInfo.S.ClearNote(judg);
        Delete(judg);
    }

    private void Delete(int judg)
    {
        Level.S.noteList.Remove(this);

        Destroy(this.gameObject);
    }
}
