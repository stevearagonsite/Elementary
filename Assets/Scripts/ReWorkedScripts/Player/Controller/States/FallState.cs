using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

namespace Player
{
    public class FallState : IState<Inputs> {

        public Dictionary<Inputs, IState<Inputs>> _transitions;


        //Implementar lo que viene del JUMP!
       // private Jumper _jump;


        //Fall multiplier Variables
        float fallMultiplier = 2.5f;
        float lowJumpMultiplier = 2f;

        float _horizontal;
        float _vertical;


        //Camera Reference
        //Player controller Reference
        //private PlayerController _pc;


        Rigidbody _rb;
        LandChecker _lc;
        CameraFSM _cam;
        AnimatorEventsBehaviour _aES;
        Animator _anim;
        PlayerController _pC;
        Transform transform;

        [HideInInspector]
        public bool land;

        private int landCount = 0;
        private float ypos;

        bool _isJumpingForward;
        float _jumpSpeed;

        public FallState(Rigidbody rb, PlayerController pC, CameraFSM cam, LandChecker lc, AnimatorEventsBehaviour aES, Transform t, Animator anim, float jumpSpeed)
        {
            _cam = cam;
            _rb = rb;
            _pC = pC;
            _lc = lc;
            _aES = aES;
            transform = t;
            _anim = anim;
            _jumpSpeed = jumpSpeed;
            /*_jump = GetComponent<Jumper>();
            _pc = GetComponent<PlayerController>();
            _cam = GetComponent<CharacterMove>().cam;
            //Initialize LandChecker 
            _lc = GetComponentInChildren<LandChecker>();
            _aES = GetComponentInChildren<AnimatorEventsBehaviour>();*/

        }

        public void Enter()
        {
            //isJumpingForward = _jump.forwardJump;
            land = false;
            //_aES.landEnd = false;
            landCount = 0;
            ypos = transform.position.y;
            _isJumpingForward = _pC.jumpForward;
            //_aES.landEnd = false;
            _anim.SetBool("toLand", false);
            _pC.forwardCheck.SetCollider(ForwardChecker.FowardSizes.AIR);
        }

        public void Execute()
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

            var ydif = Mathf.Abs(ypos - transform.position.y);
            var ydifComp = _rb.velocity.y * Time.deltaTime;
            if (ydif <= ydifComp) landCount++;
            else landCount = 0;

            ypos = transform.position.y;

            if (Mathf.Abs(_rb.velocity.y) <= 0.2) landCount++;
            else landCount = 0;

            if (landCount >= 2)
            {
                _pC.land = true;
                landCount = 0;
            }
            else
            {
                _pC.land = _lc.land;
            }

            if (_isJumpingForward)
            {
                transform.position += transform.forward * _jumpSpeed * Time.deltaTime;
            }

            var newDirection = GetCorrectedForward();

            //AirMove
            if (!_pC.forwardCheck.isForwardObstructed)
                transform.position += newDirection * Time.deltaTime * _jumpSpeed / 2;

            _anim.SetFloat("velocityY", _rb.velocity.y);

        }

        public void Exit()
        {

            _anim.SetFloat("velocityY", 0);
            _anim.SetBool("toLand", true);
            
        }

        Vector3 GetCorrectedForward()
        {
            //Get inputs
            _horizontal = GameInput.instance.horizontalMove;
            _vertical = GameInput.instance.verticalMove;

            //Get cameraForward(2D floor plain)
            var camForwardWithOutY = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z);
            var anglesign = Vector3.Cross(new Vector3(0, 0, 1), camForwardWithOutY).y > 0 ? 1 : -1;
            var _angleCorrection = Vector3.Angle(new Vector3(0, 0, 1), camForwardWithOutY) * anglesign;

            //Get forward multiplying the input vector3 with the quaternion containing the camera angle
            return (Quaternion.Euler(0f, _angleCorrection, 0f) * new Vector3(_horizontal, 0, _vertical)).normalized;
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }
    }

}
