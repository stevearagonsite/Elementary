using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCameraEnd : MonoBehaviour
{
    public GameEvent dispatchedEventOnCameraAnimationEnd;
    public void EndStory()
    {
        EventManager.DispatchEvent(dispatchedEventOnCameraAnimationEnd);
    }
}
