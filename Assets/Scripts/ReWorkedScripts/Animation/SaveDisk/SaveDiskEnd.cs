using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDiskEnd : MonoBehaviour {

    bool isActive = true;
    Material mat;
    Animator anim;
    float disolveLerp = 1;
    public float disolveSpeed;


    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat("_DisolveAmount", disolveLerp);

        anim = GetComponent<Animator>();
        EventManager.AddEventListener(GameEvent.SAVEDISK_END, OnFinalSceneStart);
    }

    void OnFinalSceneStart(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_END, OnFinalSceneStart);
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        HUDManager.instance.saveDisk.enabled = false;
    }


    void Execute()
    {
        if(disolveLerp > 0)
        {
            disolveLerp -= Time.deltaTime * disolveSpeed;
            mat.SetFloat("_DisolveAmount", disolveLerp);
        }
        else
        {
            if (anim == null) anim = GetComponent<Animator>();
            anim.SetTrigger("EnterDrive");
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }

    public void OnEnterDrive()
    {
       EventManager.DispatchEvent(GameEvent.SAVEDISK_ENTER);
       isActive = false; 
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, OnFinalSceneStart);
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_END, OnFinalSceneStart);
    }
}
