using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInfo : MonoBehaviour
{
    [SerializeField] private bool _isHaveNote = false;
    public bool isHaveNote
    {
        get { return _isHaveNote; }
        set { _isHaveNote = value; }
    }

    public TimeLineNote haveNote = null;

    [SerializeField] private int _eventCount = 0;
    public int eventCount
    {
        get { return _eventCount; }
        set { _eventCount = value; }
    }
}
