using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public string gameObjectToFollowName;

    private bool _isActive;
    private bool _isAssigned;

    private GameObject _goToFollow;

    void Start()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute()
    {
        if (!_isAssigned)
        {
            _goToFollow = GameObject.Find(gameObjectToFollowName);
            if(_goToFollow != null) 
            {
                _isAssigned = true;
            }
        }
        else
        {
            transform.position = _goToFollow.transform.position;
        }

        
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);

    }
}
