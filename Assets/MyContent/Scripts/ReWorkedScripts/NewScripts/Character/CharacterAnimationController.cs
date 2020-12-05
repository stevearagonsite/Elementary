using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    private TPPController _controller;

    public float landDistance = 0.2f;

    /// <summary>
    /// Initialize 
    /// </summary>
    void Start()
    {
        _cc = GetComponentInParent<CharacterController>();
        _anim = GetComponent<Animator>();
        _controller = GetComponentInParent<TPPController>();

        InputManager.instance.AddAction(InputType.Jump, Jump);
        InputManager.instance.AddAction(InputType.Movement, MoveAction);
        InputManager.instance.AddAction(InputType.Sprint, Sprint);

        UpdatesManager.instance.AddUpdate(UpdateType.FIXED, CheckLand);
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, CheckFall);
    }

    private void CheckFall()
    {
        if (_controller.isActive) 
        {
            var val = _controller.velocity.y < -5 && !Physics.Raycast(transform.position - transform.up * 1.05f, -transform.up, landDistance*2);
            _anim.SetBool("fall", val);
        }

        
    }

    /// <summary>
    /// Set Jump trigger
    /// </summary>
    void Jump() 
    {
        if (_controller.isActive)
        {
            if (_cc.isGrounded)
                _anim.SetTrigger("jump");
        }
    }

    /// <summary>
    /// Get Axis inputs
    /// </summary>
    /// <param name="dir"></param>
    private void MoveAction(Vector2 dir)
    {
        if(_controller.isActive)
            _anim.SetFloat("speed",Mathf.Clamp01(Math.Abs(dir.x) + Math.Abs(dir.y)));
    }

    private void CheckLand()
    {
        if (_controller.isActive)
        {
            var val = Physics.Raycast(transform.position - transform.up * 1.05f, -transform.up, landDistance) || _cc.isGrounded;
            _anim.SetBool("land", val);

        }
        
    }

    private void Sprint() 
    {
        if (_controller.isActive)
        {
            var speed = _anim.GetFloat("speed");
            if(speed > 0 && _controller.actualStamina > 0) 
            {
                _anim.SetFloat("speed", 2);
            }
        }
    }
}
