using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlatform : MonoBehaviour
{
    public bool isOnPlatform;
    public Transform parent;
    private Transform _platform;
    private CharacterController _cc;
    private Vector3 _actualPosition;
    private void Start()
    {
        _cc = GetComponent<CharacterController>();
    }
    public void SetPlatformToFollow(Transform platform)
    {
        _platform = platform;
        _actualPosition = _platform.position;
        UpdatesManager.instance.AddUpdate(UpdateType.LATE, Execute);
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
        UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
    }

    private void OnDestroy()
    {
        ReleasePlatform();
    }
}
