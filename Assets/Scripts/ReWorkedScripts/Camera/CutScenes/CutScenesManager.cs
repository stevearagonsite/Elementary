using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScenesManager : MonoBehaviour {

    static CutScenesManager _instance;
    public static CutScenesManager instance { get { return _instance; } }

    public Dictionary<string, CutSceneCamera> cameraDictionary;


    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }    
    }

    void Start()
    {
        EventManager.AddEventListener(GameEvent.CAMERA_STORY, ManageCutSceneCamera);
    }

    private void ManageCutSceneCamera(object[] parameterContainer)
    {
        var camTag = (string)parameterContainer[0];
        ActivateCutSceneCamera(camTag);
    }

    public void AddCamera(CutSceneCamera cam, string tag)
    {
        if(cameraDictionary == null) cameraDictionary = new Dictionary<string, CutSceneCamera>();
        if(!cameraDictionary.ContainsKey(tag)) cameraDictionary.Add(tag, cam);
    }

    public void RemoveCamera(string tag)
    {
        if (cameraDictionary != null && cameraDictionary.ContainsKey(tag)) cameraDictionary.Remove(tag);
    }

    public void ActivateCutSceneCamera(string tag)
    {
        if (cameraDictionary.ContainsKey(tag)) cameraDictionary[tag].Play();
    }

    public void DeActivateCutSceneCamera(string tag)
    {
        if (cameraDictionary.ContainsKey(tag)) cameraDictionary[tag].Stop();
    }

    public Camera GetCamera(string tag)
    {
        if (cameraDictionary.ContainsKey(tag)) return cameraDictionary[tag].cam;
        else return null;
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, ManageCutSceneCamera);
    }
}
