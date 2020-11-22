﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

[RequireComponent(typeof(CharacterController))]
public class TPPController : MonoBehaviour
{
    [Header("References")]
    public CameraFSM cameraT;

    [Header("Move Variables")]
    public float speed;
    public float sprintSpeed;
    public float sprintStaminaInSeconds;
    public float staminaRecoverRatio;
    public float staminaLoseRatio;


    public Transform gfx;
    public float turnSpeed;

    [Header("Obstacle Checker Variables")]
    public MoveObstacleChecker obstacleChecker;
    public float rotationOffset = 0.1f;

    [Header("Jump Variables")]
    public float gravity = -9.81f;
    public float jumpVelocity = 11; //To Calculate: Sqrt(jumpHeigh * -2 * gravity)
    public float jumpSpeed = 5;
    public float jumpFlyTime = 2;
    public float jumpFlyPower = 0.8f;

    /**
     *Move Private Variables 
     **/
    private CharacterController _cc;
    private float _horizontal;
    private float _vertical;
    private Vector3 _velocity;
    private float _actualJumpFly = 0;

    /**
     * Sprint variables
     */
    private float _actualStamina;
    private float _minStamina = -0.5f;

    public Vector3 velocity { get => _velocity;}

    /// <summary>
    /// Initialize 
    /// </summary>
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _actualStamina =
        _actualStamina = sprintStaminaInSeconds;
        InputManager.instance.AddAction(InputType.Jump, Jump);
        InputManager.instance.AddAction(InputType.Jump_Held, JumpHeld);
        InputManager.instance.AddAction(InputType.Movement, MoveAction);
        InputManager.instance.AddAction(InputType.Sprint, Sprint);
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }
    /// <summary>
    /// Add movement so the sprints
    /// </summary>
    private void Sprint()
    {
        if(_actualStamina >= 0) 
        {
            if (_cc.isGrounded) 
            {
                var dir = GetMoveDirection();
                var actualSpeed = sprintSpeed - speed;
                var movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * actualSpeed * Time.deltaTime;
                _cc.Move(dir * movementSpeed);
            }
        }
        if(_actualStamina >= _minStamina) 
        {
            _actualStamina -= Time.deltaTime * (staminaLoseRatio + staminaRecoverRatio);
        }
    }


    /// <summary>
    /// Execute every frame
    /// </summary>
    void Execute()
    {
        Move();
        ApplyGravity();
        RotateGFX();
        AddSprintStamina();
    }
    /// <summary>
    /// Constantly add stamina, so it can recover
    /// </summary>
    private void AddSprintStamina()
    {
        if(_actualStamina <= sprintStaminaInSeconds) 
        {
            _actualStamina += Time.deltaTime * staminaRecoverRatio;
        }
    }

    /// <summary>
    /// Rotate GFX in case we dont run straight
    /// </summary>
    private void RotateGFX()
    {
        var targetRotation = Quaternion.LookRotation(GetMoveDirection());
        if ((Mathf.Abs(_horizontal) > rotationOffset || Mathf.Abs(_vertical) > rotationOffset))
            gfx.rotation = Quaternion.Slerp(gfx.rotation, targetRotation, turnSpeed*Time.deltaTime);

        obstacleChecker.transform.rotation = targetRotation;
    }

    /// <summary>
    /// Apply Gravity Force
    /// </summary>
    private void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
        if(_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -speed/2;
        }
        if (_cc.isGrounded) 
        {
            _actualJumpFly = 0;
        }
    }

    /// <summary>
    /// Move Character
    /// </summary>
    private void Move()
    {
        if(obstacleChecker.CheckObstacle() && !_cc.isGrounded) 
        {
            return;
        }
        
        var dir = GetMoveDirection();
        var actualSpeed = _cc.isGrounded ? speed : jumpSpeed;
        var movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * actualSpeed * Time.deltaTime;
        _cc.Move(dir * movementSpeed);
        
    }

    /// <summary>
    /// Get Direction without Y
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMoveDirection()
    {
        var camForwardWithOutY = new Vector3(cameraT.transform.forward.x, 0, cameraT.transform.forward.z);
        var anglesign = Vector3.Cross(new Vector3(0, 0, 1), camForwardWithOutY).y > 0 ? 1 : -1;
        var angleCorrection = Vector3.Angle(new Vector3(0, 0, 1), camForwardWithOutY) * anglesign;
        
        //Get forward multiplying the input vector3 with the quaternion containing the camera angle
        return  (Quaternion.Euler(0f, angleCorrection, 0f) * new Vector3(_horizontal, 0, _vertical)).normalized;
      
    }

    /// <summary>
    /// Jump Action
    /// </summary>
    private void Jump()
    {
        if(_cc.isGrounded)
            _velocity.y = jumpVelocity; 
    }

    /// <summary>
    /// Held Jump Action
    /// </summary>
    private void JumpHeld()
    {
        if (!_cc.isGrounded)
        {
            if (_velocity.y > 0)
            {
                _velocity.y += -gravity / 3 * Time.deltaTime;
            }
            else if (_actualJumpFly < jumpFlyTime)
            {
                _velocity.y += -gravity * jumpFlyPower * Time.deltaTime;
                _actualJumpFly += Time.deltaTime;
            }
        }
    }


    /// <summary>
    /// Get Axis inputs
    /// </summary>
    /// <param name="dir"></param>
    private void MoveAction(Vector2 dir)
    {
        _horizontal = dir.x;
        _vertical = dir.y;
    }

    /// <summary>
    /// Remove Listeners 
    /// </summary>
    private void OnDestroy()
    {
        InputManager.instance.RemoveAction(InputType.Jump, Jump);
        InputManager.instance.RemoveAction(InputType.Movement, MoveAction);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
