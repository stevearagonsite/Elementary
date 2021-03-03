using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCameraTrigger : MonoBehaviour
{
    public Camera storyCam;
    private void OnTriggerEnter(Collider other)
    {
        ActivateCameraStory();
    }

    public void ActivateCameraStory()
    {
        EventManager.DispatchEvent(GameEvent.CAMERA_STORY, storyCam);
        Destroy(this);
    }
}


