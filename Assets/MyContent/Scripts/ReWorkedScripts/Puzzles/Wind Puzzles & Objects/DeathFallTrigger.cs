using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFallTrigger : MonoBehaviour {

    public string cutSceneTag;

    float _timmer = 3;
    float _tick;


    public Animator blackOut;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            _tick = 0;
            EventManager.DispatchEvent(GameEvent.CAMERA_STORY, cutSceneTag);
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }    
    }

    void Execute()
    {
        _tick += Time.deltaTime;
        if (_tick > _timmer)
        {
            blackOut.SetTrigger("FadeOutLose");
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
