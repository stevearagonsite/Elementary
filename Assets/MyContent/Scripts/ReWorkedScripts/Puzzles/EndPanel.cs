using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EndPanel : MonoBehaviour {

    float curedLerpValue = 0;
    public float activeDistance;

    public Animator cpuCap;
    public Animator cpuDrive;
    public Vector3 offset;

    public string cutSceneTag;
    bool _cutSceneStart;

    public Animator panelAnimator;
    public GameObject saveDisk;

    void Start()
    {
        EventManager.AddEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
        saveDisk.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        var dist = Vector3.Distance(transform.position + offset, other.transform.position) < activeDistance;
        var saveDisk = HUDManager.instance.saveDisk.enabled;
        var layer = other.gameObject.layer;

        if (layer == 9)
        {
            if (saveDisk && dist && !_cutSceneStart)
            {
                EventManager.DispatchEvent(GameEvent.CAMERA_STORY, cutSceneTag);
                this.saveDisk.SetActive(true);
                _cutSceneStart = true;
            }
            cpuCap.SetBool("isNear", true);
            cpuDrive.SetBool("isNear", true);
        }
    }

    private void SaveDiskEnter(object[] parameterContainer)
    {
        panelAnimator.SetTrigger("Win");
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            cpuCap.SetBool("isNear", false);
            cpuDrive.SetBool("isNear", false);
        }
    }

    void Execute()
    {
        curedLerpValue = Mathf.Lerp(curedLerpValue, 1, Time.deltaTime * 0.5f);
        foreach (var mat in LevelManager.instance.breathingScenarioMaterials)
        {
            mat.SetFloat("CuredLerp", curedLerpValue);
        }
        
        if(curedLerpValue >= 0.9f)
        {
            LevelManager.instance.whiteOutAnimator.SetTrigger("FadeOutWin");
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + offset, activeDistance);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

}
