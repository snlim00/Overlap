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
    //풀링 사용 전 함수
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
        //노트가 20초 이내로 등장해야 하는 상황이 되면 움직이기 시작
        if (timing <= LevelPlayer.timer + 5)
            doMove = true;

        if (doMove == false)
            return;

        transform.position = startPos * (timing - LevelPlayer.timer) * Level.S.noteSpeed;

        //MISS 판정범위 밖으로 벗어나면 그냥 MISS처리
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
