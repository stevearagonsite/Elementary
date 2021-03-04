using System;
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
    public float walkSpeed;
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

    public bool isActive { get { return _isActive; } set { _isActive = value;} }

    /**
     *Move Private Variables 
     **/

    private bool _isActive;
    private bool _disableGravity;

    private CharacterController _cc;
    private float _horizontal;
    private float _vertical;
    private Vector3 _velocity;
    private float _actualJumpFly = 0;
    private bool _isUsingPower = false;

    private PlayerTemperature _pT;
    private FollowPlatform _fP;
    private CharacterAnimationController _characterAnimation;

    /**
     * Sprint variables
     */
    private float _actualStamina;
    private float _minStamina = -0.5f;

    public Vector3 velocity { get => _velocity;}
    public float actualStamina { get => _actualStamina;}

    //For UI
    public float staminaPercent { get => Mathf.Clamp01(actualStamina / sprintStaminaInSeconds); }

    /// <summary>
    /// Initialize 
    /// </summary>
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _pT = GetComponentInChildren<PlayerTemperature>();
        _fP = GetComponentInChildren<FollowPlatform>();
        _characterAnimation = GetComponentInChildren<CharacterAnimationController>();
        _actualStamina = sprintStaminaInSeconds;
        InputManager.instance.AddAction(InputType.Jump, Jump);
        InputManager.instance.AddAction(InputType.Jump_Held, JumpHeld);
        InputManager.instance.AddAction(InputType.Movement, MoveAction);
        InputManager.instance.AddAction(InputType.Sprint, Sprint);
        InputManager.instance.AddAction(InputType.Walk, Walk);
        InputManager.instance.AddAction(InputType.Absorb, ExecutePower);
        InputManager.instance.AddAction(InputType.Reject, ExecutePower);
        InputManager.instance.AddAction(InputType.Stop, StopPower);
        InputManager.instance.AddAction(InputType.Test, OnTest);

        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);

        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, PrepareCharacterForSceneChange);
        EventManager.AddEventListener(GameEvent.PLAYER_DIE, DeactivateCharacter);
        EventManager.AddEventListener(GameEvent.STORY_START, DeactivateCharacter);
        EventManager.AddEventListener(GameEvent.STORY_END, ActivateCharacter);
        EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, ActivateCharacter);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_FINISH, ActivateCharacter);

        _isActive = false;
    }

    private void PrepareCharacterForSceneChange(object[] parameterContainer)
    {
        transform.position = new Vector3(0, -10000, 0);
        _disableGravity = true;
        DeactivateCharacter(null);
    }

    private void DeactivateCharacter(object[] p)
    {
        _isActive = false;
    }

    private void ActivateCharacter(object[] parameterContainer)
    {
        
        _disableGravity = false;
        _isActive = true;
    }

    private void OnTest()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopPower()
    {
        _isUsingPower = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void ExecutePower()
    {
        _isUsingPower = true;
    }

    /// <summary>
    /// Add movement so the sprints
    /// </summary>
    private void Sprint()
    {
        if(_actualStamina >= 0 && _isActive) 
        {
            if (_cc.isGrounded || _fP.isOnPlatform) 
            {
                var dir = (GetMoveDirection() * new Vector3(_horizontal, 0, _vertical)).normalized;
                var actualSpeed = sprintSpeed - speed;
                var movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * actualSpeed * Time.deltaTime;
                _cc.Move(dir * movementSpeed * _pT.lifeLeft);
            }
        }
        if(_actualStamina >= _minStamina) 
        {
            _actualStamina -= Time.deltaTime * (staminaLoseRatio + staminaRecoverRatio);
        }
    }

    private void Walk()
    {
        if (_isActive)
        {
            var dir = (GetMoveDirection() * new Vector3(_horizontal, 0, _vertical)).normalized;
            var actualSpeed = walkSpeed - speed;
            var movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * actualSpeed * Time.deltaTime;
            _cc.Move(dir * movementSpeed * _pT.lifeLeft);
        }
    }


    /// <summary>
    /// Execute every frame
    /// </summary>
    void Execute()
    {
        if (_isActive)
        {
            Move();
            RotateGFX();
            AddSprintStamina();
        }
        if (!_disableGravity)
        {
            ApplyGravity();
        }    
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
        var targetRotation = Quaternion.LookRotation((GetMoveDirection() * new Vector3(_horizontal, 0, _vertical)).normalized);
        if ((Mathf.Abs(_horizontal) > rotationOffset || Mathf.Abs(_vertical) > rotationOffset))
            gfx.rotation = Quaternion.Slerp(gfx.rotation, targetRotation, turnSpeed*Time.deltaTime);
        else if (_isUsingPower)
        {
            gfx.rotation = Quaternion.Slerp(gfx.rotation, GetMoveDirection(), turnSpeed * Time.deltaTime);
        }
        obstacleChecker.transform.rotation = targetRotation;
    }

    /// <summary>
    /// RotateGFX to gameObject initialPosition
    /// </summary>
    /// <param name="forward"></param>
    public void RotateToStartPosition(Vector3 forward)
    {
        gfx.rotation = Quaternion.LookRotation(forward);
    }

    /// <summary>
    /// Apply Gravity Force
    /// </summary>
    private void ApplyGravity()
    {
        Debug.Log(_cc.isGrounded);
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -speed/2;
        }
       
        if (_cc.isGrounded) 
        {
            _actualJumpFly = 0;
        }

        if (_fP.isOnPlatform)
        {
            _velocity.y = 0;
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
        
        var dir = (GetMoveDirection() * new Vector3(_horizontal, 0, _vertical)).normalized;
        var actualSpeed = _cc.isGrounded ? speed : jumpSpeed;
        var movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * actualSpeed * Time.deltaTime;
        _cc.Move(dir * movementSpeed * _pT.lifeLeft);
        
    }

    /// <summary>
    /// Get Direction without Y
    /// </summary>
    /// <returns></returns>
    private Quaternion GetMoveDirection()
    {
        var camForwardWithOutY = new Vector3(cameraT.transform.forward.x, 0, cameraT.transform.forward.z);
        var anglesign = Vector3.Cross(new Vector3(0, 0, 1), camForwardWithOutY).y > 0 ? 1 : -1;
        var angleCorrection = Vector3.Angle(new Vector3(0, 0, 1), camForwardWithOutY) * anglesign;
        
        //Get forward multiplying the input vector3 with the quaternion containing the camera angle
        return  (Quaternion.Euler(0f, angleCorrection, 0f));
      
    }

    /// <summary>
    /// Jump Action
    /// </summary>
    private void Jump()
    {
        if ((_cc.isGrounded || _fP.isOnPlatform) && _isActive)
        {
            _fP.ReleasePlatform();
            _velocity.y = jumpVelocity;
            _characterAnimation.Jump();

        }
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
        InputManager.instance.RemoveAction(InputType.Jump_Held, JumpHeld);
        InputManager.instance.RemoveAction(InputType.Sprint, Sprint);
        InputManager.instance.RemoveAction(InputType.Walk, Walk);
        InputManager.instance.RemoveAction(InputType.Absorb, ExecutePower);
        InputManager.instance.RemoveAction(InputType.Reject, ExecutePower);
        InputManager.instance.RemoveAction(InputType.Stop, StopPower);

        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, PrepareCharacterForSceneChange);
        EventManager.RemoveEventListener(GameEvent.PLAYER_DIE, DeactivateCharacter);
        EventManager.RemoveEventListener(GameEvent.STORY_START, DeactivateCharacter);
        EventManager.RemoveEventListener(GameEvent.STORY_END, ActivateCharacter);
        EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, ActivateCharacter);
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_FINISH, ActivateCharacter);

        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
