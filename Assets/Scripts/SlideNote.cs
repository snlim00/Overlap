using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideNote : Note
{
    private static ParticleManager particleMgr;

    void Awake()
    {
        particleMgr = FindObjectOfType<ParticleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if(TouchManager.holding == true && Mathf.Abs(timing - LevelPlayer.timer) <= Level.S.judgRange[JUDG.PERFECT] * 0.5f)
        {
            Clear(JUDG.PERFECT);
            particleMgr.ParticleGeneration(-1);
        }
    }
}
