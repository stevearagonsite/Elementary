using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCameraEnd : MonoBehaviour
{
    public void EndStory()
    {
        EventManager.DispatchEvent(GameEvent.CAMERA_NORMAL);
    }
}
