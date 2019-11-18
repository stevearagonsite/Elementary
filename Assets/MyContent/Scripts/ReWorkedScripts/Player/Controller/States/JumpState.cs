using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

namespace Player
{
    public class JumpState : IState<Inputs>
    {

        public Dictionary<Inputs, IState<Inputs>> _transitions;

        private Collider _col;
        private Rigidbody _rb;
        private LandChecker _lc;

        float _jumpSpeed;
        float _jumpForce;

        //Fall multiplier Variables
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        private float _horizontal;
        private float _vertical;

        bool fixCamera;

        [HideInInspector]
        public bool forwardJump;

        //CameraController _cam;
        CameraFSM _cam;
        PlayerController _pC;
        AnimatorEventsBehaviour _aES;
        //private CharacterMove _cm;

        Transform transform;
        Animator _anim;

        Vector3 initialForward;

        public JumpState(Rigidbody rb, CameraFSM cam, PlayerController pC, LandChecker lc, AnimatorEventsBehaviour aES, Transform t, Animator anim, float jumpForce, float jumpSpeed)
        {
            _rb = rb;
            _cam = cam;
            _pC = pC;
            _lc = lc;
            //_fall = GetComponent<FallState>();
            _aES = aES;
            //_cm = GetComponent<CharacterMove>();
            transform = t;
            _anim = anim;

            _jumpForce = jumpForce;
            _jumpSpeed = jumpSpeed;
            fixCamera = true;

            EventManager.AddEventListener(GameEvent.CAMERA_FIXPOS, OnFixCameraEnter);
            EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, OnNormalCameraEnter);
        }

        private void OnNormalCameraEnter(object[] parameterContainer)
        {
            fixCamera = true;
        }

        private void OnFixCameraEnter(object[] parameterContainer)
        {
            fixCamera = false;
        }

        public void Enter()
        {
            //Apply jump force
            _rb.velocity = Vector3.up * _jumpForce;
            forwardJump = false;

            forwardJump = _pC.Fsm.Last == typeof(MoveState);
            _pC.jumpForward = forwardJump;
            //Set Animator Parameter

            _anim.SetBool("toJump", true);
            _anim.SetBool("toLand", false);
            
            //_aES.landEnd = false;

            _cam.normalState.unadjustedDistance = 3f;
            _cam.normalState.positionSmoothness = 0.1f;
            _pC.isSkillLocked = true;

            initialForward = GetCorrectedForward(true);

            _pC.forwardCheck.SetCollider(ForwardChecker.FowardSizes.AIR);
        }

        public void Execute()
        {
            if (_rb.velocity.y <= -0.1) _pC.land = _lc.land;

            if (fixCamera)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(initialForward), 0.5f);

            if (forwardJump)
            {
                transform.position += transform.forward * _jumpSpeed * Time.deltaTime;
            }

            if (Mathf.Abs(GameInput.instance.horizontalMove) > 0.1f || Mathf.Abs(GameInput.instance.verticalMove) > 0.1f)
            {
                var newDirection = GetCorrectedForward(false);

                //AirMove
                if(!_pC.forwardCheck.isForwardObstructed)
                    transform.position += newDirection * Time.deltaTime * _jumpSpeed / 2;
            }

            
            if (!GameInput.instance.jumpButton)
            {
                //Speed up fall
                _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        public void Exit()
        {
            _anim.SetBool("toJump", false);
        }

        Vector3 GetCorrectedForward(bool transition)
        {
            //Get inputs
            _horizontal = GameInput.instance.horizontalMove;
            _vertical = GameInput.instance.verticalMove;
            var camForwardWithOutY = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z);
            if (!transition)
            {
                //Get cameraForward(2D floor plain)
                
                var anglesign = Vector3.Cross(new Vector3(0, 0, 1), camForwardWithOutY).y > 0 ? 1 : -1;
                var _angleCorrection = Vector3.Angle(new Vector3(0, 0, 1), camForwardWithOutY) * anglesign;

                //Get forward multiplying the input vector3 with the quaternion containing the camera angle
                return (Quaternion.Euler(0f, _angleCorrection, 0f) * new Vector3(_horizontal, 0, _vertical)).normalized;
            }
            else
            {
                return camForwardWithOutY;
            }
        }

        public void OnDestroy()
        {
            EventManager.RemoveEventListener(GameEvent.CAMERA_FIXPOS, OnFixCameraEnter);
            EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, OnNormalCameraEnter);
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }

    }
}
