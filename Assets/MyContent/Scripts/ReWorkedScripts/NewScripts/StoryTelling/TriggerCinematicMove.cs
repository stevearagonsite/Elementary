using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveOnCinematic))]
public class TriggerCinematicMove : MonoBehaviour
{
    private PlayerMoveOnCinematic _move;
    public bool walk;
    private void Start()
    {
        _move = GetComponent<PlayerMoveOnCinematic>();
    }
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.STORY_START, _move.nodes , walk);
        Debug.Log("Start Story: " + gameObject.name);

        Destroy(this);
    }
}
