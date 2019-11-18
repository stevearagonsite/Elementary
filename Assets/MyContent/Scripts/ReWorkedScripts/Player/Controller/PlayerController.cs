using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;
using Skills;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {

        [HideInInspector]
        public bool jumpForward;
        [HideInInspector]
        public bool land;

        [Header("Move Parameters")]

        public float speed;
        public float angleTurnTolerance;
        [Range(0, 0.9f)]
        public float idleTurnTolerance;
        [Range(0, 0.9f)]
        public float runningTurnSpeed;

        [Header("Jump Parameters")]
        public float jumpForce;
        public float jumpSpeed;
        public float jumpTolerance;

        [Header("Collision Parameters")]
        public float collisionDistance;
        public LayerMask lm;

        [Header("Fall Parameters")]
        public float fallDistance;
        public LayerMask fallLayer;

        //States
        IdleState idleState;
        MoveState moveState;
        JumpState jumpState;
        FallState fallState;
        LandState landState;
        AimingState aimState;

        Animator _anim;

        [Header("Camera Reference")]
        //public CameraFMS cam;
        public CameraFSM cam2;

        AnimatorEventsBehaviour _aEB;
        Rigidbody _rB;
        SkillController _skill;
        PlayerTemperature _temp;

        //Sensors
        LandChecker _lC;
        [HideInInspector]
        public ForwardChecker forwardCheck;
        int fallCount;

        FSM<Inputs> _fsm;
        public FSM<Inputs> Fsm { get { return _fsm; } }

        [HideInInspector]
        public bool isSkillLocked;
        [HideInInspector]
        public bool fixedCamera;

        bool isActive;

        void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
            _lC = GetComponentInChildren<LandChecker>();
            //_camController = cam.GetComponent<CameraController>();
            _aEB = GetComponentInChildren<AnimatorEventsBehaviour>();
            _rB = GetComponent<Rigidbody>();
            forwardCheck = GetComponentInChildren<ForwardChecker>();
            _skill = GetComponentInChildren<SkillController>();
            _temp = GetComponentInChildren<PlayerTemperature>();
            isSkillLocked = false;

            #region FSM
            idleState = new IdleState(this, _anim, cam2.transform, transform, cam2);
            moveState = new MoveState(cam2, transform, angleTurnTolerance, idleTurnTolerance, runningTurnSpeed, speed, this, _aEB, _anim);
            jumpState = new JumpState(_rB, cam2, this, _lC, _aEB, transform, _anim, jumpForce, jumpSpeed);
            fallState = new FallState(_rB, this, cam2, _lC, _aEB, transform, _anim, jumpSpeed);
            landState = new LandState(_anim, this, _aEB, cam2);
            aimState = new AimingState(transform, cam2, _anim, speed, this, _skill);

            //Fsm Transitions
            var idleTransitions = new Dictionary<Inputs, IState<Inputs>>();
            idleTransitions.Add(Inputs.Move, moveState);
            idleTransitions.Add(Inputs.Fall, fallState);
            idleTransitions.Add(Inputs.Jump, jumpState);

            var moveTransitions = new Dictionary<Inputs, IState<Inputs>>();
            moveTransitions.Add(Inputs.Idle, idleState);
            moveTransitions.Add(Inputs.Jump, jumpState);
            moveTransitions.Add(Inputs.Fall, fallState);
            moveTransitions.Add(Inputs.Aiming, aimState);

            var jumpTransitions = new Dictionary<Inputs, IState<Inputs>>();
            jumpTransitions.Add(Inputs.Land, landState);
            jumpTransitions.Add(Inputs.Fall, fallState);

            var fallTransitions = new Dictionary<Inputs, IState<Inputs>>();
            fallTransitions.Add(Inputs.Land, landState);

            var landTransitions = new Dictionary<Inputs, IState<Inputs>>();
            landTransitions.Add(Inputs.EndLand, idleState);
            landTransitions.Add(Inputs.Jump, jumpState);

            var aimTransitions = new Dictionary<Inputs, IState<Inputs>>();
            aimTransitions.Add(Inputs.NotAiming, moveState);
            aimTransitions.Add(Inputs.Idle, idleState);
            aimTransitions.Add(Inputs.Fall, fallState);
            aimTransitions.Add(Inputs.Jump, jumpState);

            idleState.Transitions = idleTransitions;
            moveState.Transitions = moveTransitions;
            jumpState.Transitions = jumpTransitions;
            fallState.Transitions = fallTransitions;
            landState.Transitions = landTransitions;
            aimState.Transitions = aimTransitions;

            
            #endregion

            fallCount = 0;
        }

        void Start ()
        {
            _fsm = new FSM<Inputs>(idleState);
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
            EventManager.AddEventListener(GameEvent.CAMERA_FIXPOS, ToFixedCamera);
            EventManager.AddEventListener(GameEvent.CAMERA_FIXPOS_END, onFixCameraTransitionEnd);
            EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, ToNormalCamera);
            EventManager.AddEventListener(GameEvent.CAMERA_STORY, ToDemoCamera);
            isActive = true;
        }

        private void ToDemoCamera(object[] parameterContainer)
        {
            isActive = false;
            //_fsm.ProcessInput(Inputs.Idle);
        }

        void ToFixedCamera(object[] parameterContainer)
        {
            fixedCamera = true;
            isActive = cam2.Fsm.Last.ToString() == "TPCamera.FixedState";

        }

        void ToNormalCamera(object[] parameterContainer)
        {
            fixedCamera = false;
            isActive = true;
        }

        void onFixCameraTransitionEnd(object[] parameterContainer)
        {
            isActive = true;
        }

        void Execute ()
        {    
            CheckInputs();
            _fsm.Execute();
        }

        void CheckInputs()
        {
            
            if (GameInput.instance.initialJumpButton && !land && CheckJump() && isActive) _fsm.ProcessInput(Inputs.Jump);

            if (CheckMove())
            {
                _fsm.ProcessInput(Inputs.Move);
            }
            else _fsm.ProcessInput(Inputs.Idle);

            if (land)
            {
                _fsm.ProcessInput(Inputs.Land);
                land = false;
            }

            if (_aEB.landEnd)
            {
                _fsm.ProcessInput(Inputs.EndLand);
            }

            
            if (CheckFall())
            {
                _fsm.ProcessInput(Inputs.Fall);
            }
            else if (((GameInput.instance.absorbButton && _skill.currentSkill == Skills.Skills.VACCUM) || GameInput.instance.blowUpButton) 
                  && !fixedCamera 
                  && !GameInput.instance.sprintButton 
                  && isActive)
            {
                _fsm.ProcessInput(Inputs.Aiming);
            }
            else
            {
                _fsm.ProcessInput(Inputs.NotAiming);
            }

        }

        public bool CheckMove()
        {
            return (Mathf.Abs(GameInput.instance.horizontalMove) > 0.1f || Mathf.Abs(GameInput.instance.verticalMove) > 0.1f) && isActive;
        }

        public bool CheckFall()
        {
            //Triple check for fall state
            if (_rB.velocity.y < -0.2f && !_lC.land) fallCount++;
            else fallCount = 0;
            return fallCount >= 2 && !Physics.Raycast(transform.position, -transform.up, fallDistance, fallLayer,QueryTriggerInteraction.Ignore);
        }

        bool CheckJump()
        {
            Debug.DrawLine(transform.position + transform.up * 1.6f, transform.position + transform.up * 1.6f + transform.up * jumpTolerance);
            return !Physics.Raycast(transform.position + transform.up * 1.6f, transform.up, jumpTolerance);
        }

        public void RespawnOnCheckPoint(Transform tr)
        {
            transform.position = tr.position;
            transform.rotation = tr.rotation;
            _rB.velocity = Vector3.zero;
            _temp.Restart();
            isActive = true;
        }

        private void OnDestroy()
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
            EventManager.RemoveEventListener(GameEvent.CAMERA_FIXPOS, ToFixedCamera);
            EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, ToNormalCamera);
            EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, ToDemoCamera);
            jumpState.Exit();
            moveState.Exit();
        }

    }



}
