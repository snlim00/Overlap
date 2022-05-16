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

    Vector2 startPos;

    public void Execute(int num, int angle, float timing, float spawnDis, int type)
    {
        this.num = num;
        this.angle = -(angle + 90);
        this.timing = timing;
        this.type = type;

        transform.position = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, this.angle);
        transform.Translate(-spawnDis, 0, 0);

        startPos = transform.position;

        doMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    protected void Move()
    {
        if (doMove == false)
            return;

        transform.Translate(Level.S.noteSpeed * Time.deltaTime, 0, 0);
        //transform.position = new Vector3(startPos.x + (Level.S.noteSpeed * (Time.time - LevelPlayer.startTime)), 0, 0);
    }

    public void Clear(int judg)
    {
        switch(judg)
        {
            case JUDG.PERFECT:

                Delete();
                break;

            case JUDG.GOOD:

                Delete();
                break;

            case JUDG.MISS:

                Delete();
                break;
        }
    }

    private void Delete()
    {
        Level.S.noteList.Remove(this);

        Destroy(this.gameObject);
    }
}
