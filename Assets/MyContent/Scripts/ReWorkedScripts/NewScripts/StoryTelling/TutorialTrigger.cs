using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public int tutorialNumber;
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.TRIGGER_TUTORIAL, tutorialNumber);
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.TRIGGER_TUTORIAL_STOP, tutorialNumber);
    }
}
