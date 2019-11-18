using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

namespace Player
{
    public class MoveState : IState<Inputs>
    {
        public Dictionary<Inputs, IState<Inputs>> _transitions;

        #region Global Variables
        float _speed;
        float cameraSmoothness;
        CameraFSM _cam;
        Transform transform;

        //Turn Variables
        Vector3 _oldDirection;
        Vector3 _newDirection;
        Vector3 _desiredForward;
        bool _turn;
        bool cameraChange;

        float _angleTurnTolerance;

        [Range(0.1f, 0.9f)]
        float _idleTurnSpeed;

        [Range(0.1f, 0.9f)]
        float _runingTurnSpeed;

        //Move Variables
        float _horizontal;
        float _vertical;
        float _movementSpeed;
        float _angleCorrection;

        bool forwardCollision;


        //Player Controller
        PlayerController _pC;
        AnimatorEventsBehaviour _aEB;
        Animator _anim;
        #endregion

        public MoveState(CameraFSM cam, Transform t, float angleTurnTolerance, float idleTurnSpeed, float runingTurnSpeed,float speed, PlayerController pC,
                        AnimatorEventsBehaviour aEB, Animator anim)
        {
            _cam = cam;
            transform = t;
            _angleTurnTolerance = angleTurnTolerance;
            _idleTurnSpeed = idleTurnSpeed;
            _runingTurnSpeed = runingTurnSpeed;
            _speed = speed;
            _pC = pC;
            _aEB = aEB;
            _anim = anim;

            _oldDirection.x = transform.forward.x;
            _oldDirection.z = transform.forward.z;
            _oldDirection = _oldDirection.normalized;

            cameraSmoothness = 0.3f;
            EventManager.AddEventListener(GameEvent.CAMERA_FIXPOS, OnFixCameraEnter);
            EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, OnNormalCameraEnter);
        }

        private void OnNormalCameraEnter(object[] parameterContainer)
        {
            cameraChange = false;
        }

        private void OnFixCameraEnter(object[] parameterContainer)
        {
            cameraChange = true;
        }

        public void Enter()
        {
            GetCorrectedForward();
            //In case you have to pivot from idle
            if (_oldDirection != _newDirection)
            {
                var angle = Vector3.Angle(_oldDirection, _newDirection);
                _turn = Mathf.Abs(angle) > _angleTurnTolerance;
            }
            //Set Animator Transition
            _anim.SetFloat("speed", 1);
            _pC.forwardCheck.SetCollider(ForwardChecker.FowardSizes.WALK);

        }

        public void Execute()
        {
            if (cameraChange)
            {
                cameraChange = !GameInput.instance.onDirectionChange;
            }
            if (_turn)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_newDirection), _idleTurnSpeed);
                var angle = Vector3.Angle(transform.forward, _newDirection);
                if (Mathf.Abs(angle) < 5)
                {
                    _turn = false;
                    transform.forward = _newDirection;
                }
            }
            else
            {
                GetCorrectedForward();
                //Rotate to the new forward
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_newDirection), _runingTurnSpeed);

                //MoveForward
                if (_aEB.landEnd && !_pC.forwardCheck.isForwardObstructed)
                {
                    if (GameInput.instance.sprintButton)
                    {

                        _anim.SetBool("sprint", true);
                        
                        _movementSpeed = 1.2f * _speed;
                        _pC.fallDistance = 0.7f;

                        cameraSmoothness = 0.1f;
                        //_cam.ChangeSmoothness(cameraSmoothness);
                        _cam.normalState.positionSmoothness = cameraSmoothness;
                    }
                    else
                    {
                        
                        _anim.SetBool("sprint", false);
                        
                        _movementSpeed = _speed / 1.2f;
                        _pC.fallDistance = 0.5f;

                        
                        _cam.normalState.unadjustedDistance = 2f;

                        if (cameraSmoothness < 0.3f)
                        {
                            cameraSmoothness += 0.1f * Time.deltaTime;
                        }
                        //_cam.ChangeSmoothness(cameraSmoothness);
                        //_cam.normalState.positionSmoothness = cameraSmoothness;
                    }

                    transform.position += transform.forward * Time.deltaTime * _movementSpeed;
                }

                
                
            }
        }

        public void Exit()
        {
            _oldDirection.x = transform.forward.x;
            _oldDirection.z = transform.forward.z;
            _oldDirection = _oldDirection.normalized;


            _anim.SetFloat("speed", 0);
            _anim.SetBool("sprint", false);
            EventManager.RemoveEventListener(GameEvent.CAMERA_FIXPOS, OnFixCameraEnter);
            EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, OnNormalCameraEnter);
        }

        /// <summary>
        /// Get the new Forward refered to the camera
        /// </summary>
        void GetCorrectedForward()
        {
            
                //Get inputs
                _horizontal = GameInput.instance.horizontalMove;
                _vertical = GameInput.instance.verticalMove;
                _movementSpeed = new Vector2(_horizontal, _vertical).normalized.magnitude * _speed;

            //Get cameraForward(2D floor plain)
            if (!cameraChange)
            {
                var camForwardWithOutY = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z);
                var anglesign = Vector3.Cross(new Vector3(0, 0, 1), camForwardWithOutY).y > 0 ? 1 : -1;
                _angleCorrection = Vector3.Angle(new Vector3(0, 0, 1), camForwardWithOutY) * anglesign;
            }
                //Get forward multiplying the input vector3 with the quaternion containing the camera angle
                _newDirection = (Quaternion.Euler(0f, _angleCorrection, 0f) * new Vector3(_horizontal, 0, _vertical)).normalized;
            
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }
        
    }

}
        
