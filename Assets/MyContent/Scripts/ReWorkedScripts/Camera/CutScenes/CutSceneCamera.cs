using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCamera : MonoBehaviour {

    public string cutSceneTag;
    [HideInInspector]
    public Camera cam;
    CutScene csScript;

	void Start ()
    {
        CutScenesManager.instance.AddCamera(this, cutSceneTag);
        csScript = GetComponent<CutScene>();
        cam = GetComponent<Camera>();
	}
	
    public void Play()
    {
        csScript.Enter();
        UpdatesManager.instance.AddUpdate(UpdateType.LATE, Execute);
    }

	void Execute ()
    {
        csScript.Execute();
	}

    public void Stop()
    {
        csScript.Exit();
        UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
        CutScenesManager.instance.RemoveCamera(cutSceneTag);
    }
}
