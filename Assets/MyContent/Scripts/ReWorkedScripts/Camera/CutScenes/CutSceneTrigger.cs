using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour{

    public string cutSceneTag;
    public bool _isActive;
    

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 && _isActive)
        {
            EventManager.DispatchEvent(GameEvent.CAMERA_STORY, cutSceneTag);
            _isActive = false;
        }    
    }
}
