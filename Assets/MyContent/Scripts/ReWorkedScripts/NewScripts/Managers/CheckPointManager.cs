using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

public class CheckPointManager : MonoBehaviour
{
    private static CheckPointManager _instance;
    public static CheckPointManager instance { get { return _instance; } set { _instance = value; } }
    CheckPoint _activeCheckPoint;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    public void RegisterActiveCheckPoint(CheckPoint cp)
    {
        _activeCheckPoint = cp;
    }

    public void RemoveCheckPoint()
    {
        _activeCheckPoint = null;
    }

    public void RespawnPlayerInLastActiveCheckpoint()
    {
        var player = GameObject.Find("Character");
        player.transform.position = _activeCheckPoint.transform.position;
        player.transform.rotation = _activeCheckPoint.transform.rotation;
        player.GetComponent<TPPController>().RotateToStartPosition(transform.forward);


        var camera = GameObject.Find("TTPCamera");
        camera.GetComponent<CameraFSM>().GoToStartPosition();
    }
}
