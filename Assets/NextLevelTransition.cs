using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.START_LEVEL_TRANSITION);
    }
}
