using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCinematicMove : MonoBehaviour
{
    private PlayerMoveOnCinematic _move;

    private void Start()
    {
        _move = GetComponent<PlayerMoveOnCinematic>();
    }
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.STORY_START, _move.nodes);
        Destroy(this);
    }
}
