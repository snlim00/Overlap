using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideNote : Note
{
    private static TouchManager touchMgr;

    void Awake()
    {
        if(touchMgr == null)
            touchMgr = FindObjectOfType<TouchManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if(TouchManager.holding == true && Mathf.Abs(timing - LevelPlayer.timer) <= Level.S.judgRange[JUDG.PERFECT] * 0.5f)
        {
            if(touchMgr.CheckAngle(transform.eulerAngles.z) == true)
            {
                Clear(JUDG.PERFECT);
                touchMgr.particleMgr.ParticleGeneration(-1);
            }
        }
    }
}
