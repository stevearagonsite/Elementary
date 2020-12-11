using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlatform : MonoBehaviour
{
    public bool isOnPlatform;
    private Transform _platformTR;

    private Vector3 _lastPosition;

    private CharacterController _cc;

    public Transform platformTR {
        get 
        {
            return  _platformTR; 
        } 
        set 
        {
            _platformTR = value; 
            if(_platformTR != null)
                _lastPosition = _platformTR.position; 
        } 
    }

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    // Update is called once per frame
    void Execute()
    {
        if (_platformTR != null)
        {
            var delta = _platformTR.position - _lastPosition;
            if (isOnPlatform)
            {
                _cc.Move(delta);
            }
            _lastPosition = _platformTR.position;
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);

    }
}
