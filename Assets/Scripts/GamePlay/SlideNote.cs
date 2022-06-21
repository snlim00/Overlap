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

        if(TouchManager.holding == true && Mathf.Abs(timing - LevelPlayer.timer) <= Level.S.judgRange[JUDG.S_PERFECT])
        {
            if(touchMgr.CheckAngle(transform.eulerAngles.z) == true)
            {
                Clear(JUDG.S_PERFECT);
                touchMgr.particleMgr.ParticleGeneration(-1);
            }
        }
    }
}
