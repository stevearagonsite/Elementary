using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCameraTrigger : MonoBehaviour
{
    public Camera storyCam;
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.CAMERA_STORY, storyCam);
        Destroy(this);
    }
}
