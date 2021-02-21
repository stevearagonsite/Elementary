using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormChangeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.DOUBLE_STORM_STOP);
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.DOUBLE_STORM_PLAY);

    }
}
