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

        startPos = transform.position.normalized;

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

        //transform.Translate(Level.S.noteSpeed * Time.deltaTime, 0, 0);
        //transform.position = new Vector3(startPos.x + (Level.S.noteSpeed * (Time.time - LevelPlayer.startTime)), 0, 0);
        transform.position = startPos * (timing - LevelPlayer.timer) * Level.S.noteSpeed;
    }

    public void Clear(int judg)
    {
        GameInfo.S.ClearNote(judg);
        Delete();
    }

    private void Delete()
    {
        Level.S.noteList.Remove(this);

        Destroy(this.gameObject);
    }
}
