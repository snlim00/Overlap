using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInfo : MonoBehaviour
{
    private bool _haveNote = false;
    
    public bool haveNote
    {
        get { return _haveNote; }
        set { _haveNote = value; }
    }
}
