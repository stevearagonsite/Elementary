using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseNode
{
    [HideInInspector]
    public Rect myRect;

    [HideInInspector]
    public bool isInitialNode;
    public string dialog;
    public float duration;
    [SerializeField]
    public BaseNode nextNode;
    
    public bool skipable;

    private bool _overNode;

    public BaseNode(float x, float y, float width, float height)
    {
        myRect = new Rect(x, y, width, height);
    }

    public void CheckMouse(Event cE, Vector2 pan)
    {
        if (myRect.Contains(cE.mousePosition - pan))
            _overNode = true;
        else
            _overNode = false;
    }

    public bool OverNode
    { get { return _overNode; } }
}
