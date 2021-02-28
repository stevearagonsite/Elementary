using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorToTeleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.TELEPORT_START);
    }
}
