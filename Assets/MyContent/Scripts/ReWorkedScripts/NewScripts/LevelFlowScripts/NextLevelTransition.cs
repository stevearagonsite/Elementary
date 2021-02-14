using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTransition : MonoBehaviour
{
    public bool checkGoals;
    private void OnTriggerEnter(Collider other)
    {
        if (checkGoals)
        {
            if (GameObjectiveManager.instance.CheckEndOfLevelGoals())
            {
                EventManager.DispatchEvent(GameEvent.START_LEVEL_TRANSITION);
            }
        }
        else
        {
            EventManager.DispatchEvent(GameEvent.START_LEVEL_TRANSITION);
        }
    }
}
