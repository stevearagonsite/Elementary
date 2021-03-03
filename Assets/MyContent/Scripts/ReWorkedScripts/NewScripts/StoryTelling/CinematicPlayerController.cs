using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicPlayerController : MonoBehaviour
{
    private List<CinematicNode> _nodes;
    private int _actualNodeIndex;
    private CharacterController _cc;
    private TPPController _tppC;
    private CharacterAnimationController _aC;
    private bool _walk;

    public bool isActive;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _tppC = GetComponent<TPPController>();
        _aC = GetComponentInChildren<CharacterAnimationController>();
        EventManager.AddEventListener(GameEvent.STORY_START, ActivateCharacterMove);
        EventManager.AddEventListener(GameEvent.STORY_END, DeActivateCharacterMove);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, DeActivateCharacterMove);
    }

    private void DeActivateCharacterMove(object[] p)
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        isActive = false;
    }

    private void ActivateCharacterMove(object[] p)
    {
        _aC.Idle();
        if (p != null && p[0] is List<CinematicNode>)
        {
            isActive = true;
            _nodes = (List<CinematicNode>)p[0];
            _actualNodeIndex = 0;
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
        if(p != null && p.Length > 1 && p[1] is bool)
        {
            _walk = (bool)p[1];
        }

    }

    
    void Execute()
    {
        if(Vector3.Distance(transform.position, _nodes[_actualNodeIndex].position) > _nodes[_actualNodeIndex].radius)
        {
            var dir = (_nodes[_actualNodeIndex].position - transform.position).normalized;
            dir.y = 0;
            var speed = _walk ? _tppC.walkSpeed/4*3 : _tppC.speed;
            _cc.Move(dir * speed * Time.deltaTime);
            RotateGFX(dir);
           
            if (_walk)
            {
                _aC.WalkForCinematic();
            }
            else
            {
                _aC.MoveForCinematic();
            }
        }
        else
        {
            if((_actualNodeIndex + 1) < (_nodes.Count))
            {
                _actualNodeIndex++;
            }
            else
            {
                UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
                isActive = false;
            }
        }
    }

    private void RotateGFX(Vector3 dir)
    {
        var targetRotation = Quaternion.LookRotation(dir);
        _tppC.gfx.rotation = Quaternion.Slerp(_tppC.gfx.rotation, targetRotation, _tppC.turnSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.STORY_START, ActivateCharacterMove);
        EventManager.RemoveEventListener(GameEvent.STORY_END, DeActivateCharacterMove);
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, DeActivateCharacterMove);

    }
}
