﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlatform : MonoBehaviour
{
    private bool _isOnPlatform;
    public bool isOnPlatform { get { return _isOnPlatform || _platform != null;  } set { _isOnPlatform = value; } }
    private Transform _platform;
    private CharacterController _cc;
    private Vector3 _actualPosition;

    private void Start()
    {
        _cc = GetComponentInParent<CharacterController>();
    }
    public void SetPlatformToFollow(Transform platform)
    {
        _platform = platform;
        _actualPosition = _platform.position;
        _isOnPlatform = true;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    private void Execute()
    {
        if(_platform != null)
        {
            var delta = (_platform.position - _actualPosition);
            _cc.Move(delta);
            _actualPosition = _platform.position;
        }
    }

    public void ReleasePlatform()
    {
        _platform = null;
        _isOnPlatform = false;
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    private void OnDestroy()
    {
        ReleasePlatform();
    }
}
