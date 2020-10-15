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

        UpdatesManager.instance.AddUpdate(UpdateType.FIXED, CheckLand);
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, CheckFall);
    }

    private void CheckFall()
    {
        
        var val = _controller.velocity.y < -5 && !Physics.Raycast(transform.position - transform.up * 1.05f, -transform.up, landDistance*2);
        _anim.SetBool("fall", val);

        
    }

    /// <summary>
    /// Set Jump trigger
    /// </summary>
    void Jump() 
    {
        if (_cc.isGrounded)
            _anim.SetTrigger("jump");
    }

    /// <summary>
    /// Get Axis inputs
    /// </summary>
    /// <param name="dir"></param>
    private void MoveAction(Vector2 dir)
    {
        _anim.SetFloat("speed",Math.Abs(dir.x) + Math.Abs(dir.y));
        Debug.Log("move");
    }

    private void CheckLand()
    {
        
         var val = Physics.Raycast(transform.position - transform.up * 1.05f, -transform.up, landDistance) || _cc.isGrounded;
         _anim.SetBool("land", val);
        
    }
}
