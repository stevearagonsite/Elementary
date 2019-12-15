using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class EndPanel : MonoBehaviour {

    float curedLerpValue = 0;
    public float activeDistance;

    public Vector3 offset;

    public string cutSceneTag;
    bool _cutSceneStart;


    void Start()
    {
        EventManager.AddEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

    void OnTriggerStay(Collider other)
    {
        var layer = other.gameObject.layer;

        if (layer == 9 && !_cutSceneStart)
        {
            EventManager.DispatchEvent(GameEvent.CAMERA_STORY, cutSceneTag);
            _cutSceneStart = true;
        }
    }

    private void SaveDiskEnter(object[] parameterContainer)
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

    void Execute()
    {
        if(curedLerpValue >= 0.9f)
        {
            LevelManager.instance.whiteOutAnimator.SetTrigger("FadeOutWin");
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
            
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(collider.center, collider.size);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }
}
