using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    private TPPController _controller;
    private CinematicPlayerController _cinematicController;
    private bool _isGamePaused;

    public float landDistance = 0.2f;
    public LayerMask landLayer;



    /// <summary>
    /// Initialize 
    /// </summary>
    void Start()
    {
        _cc = GetComponentInParent<CharacterController>();
        _anim = GetComponent<Animator>();
        _controller = GetComponentInParent<TPPController>();
        _cinematicController = GetComponentInParent<CinematicPlayerController>();

        InputManager.instance.AddAction(InputType.Jump, Jump);
        InputManager.instance.AddAction(InputType.Movement, MoveAction);
        InputManager.instance.AddAction(InputType.Sprint, Sprint);
        InputManager.instance.AddAction(InputType.Pause, Pause);

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

    private void Pause()
    {
        _isGamePaused = !_isGamePaused;
        float speed = _isGamePaused ? 0:1;
        _anim.SetFloat("animationSpeed", speed);
    }

    /// <summary>
    /// Get Axis inputs
    /// </summary>
    /// <param name="dir"></param>
    public void MoveAction(Vector2 dir)
    {
        if(_controller.isActive || _cinematicController.isActive)
            _anim.SetFloat("speed",Mathf.Clamp01(Math.Abs(dir.x) + Math.Abs(dir.y)));
    }

    private void CheckLand()
    {
        if (_controller.isActive)
        {
            var val = Physics.Raycast(transform.position - transform.up * 1.05f, -transform.up, landDistance, landLayer) || _cc.isGrounded;
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
