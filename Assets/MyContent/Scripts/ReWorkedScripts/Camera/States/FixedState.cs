using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPCamera
{
    public class FixedState : IState<Inputs>
    {
        Dictionary<Inputs, IState<Inputs>> _transitions;

        float _targetX;
        public float targetX { set { _targetX = value; } }

        float _targetY;
        public float targetY { set{ _targetY = value; } }

        float _currentX;
        float _currentY;
        Transform transform;
        Transform _target;

        float _normalStateDistance;
        float _distance;
        float _targetDistance;
        float _positionSmoothness;

        float _transitionRotSpeed;
        float _rotSpeed = 160;
        bool _rotated;

        public float targetDistance { set { _targetDistance = value; } }

        public FixedState(Transform t ,Transform target, float normalStateDistance)
        {
            transform = t;
            _target = target;
            _normalStateDistance = normalStateDistance;
        }

        public void Enter()
        {
            _currentX = transform.eulerAngles.y;
            _currentY = transform.eulerAngles.x;
            _positionSmoothness = 0.05f;
            _rotated = false;
            while(_currentX < -180 || _currentX > 180)
            {
                _currentX = _currentX < 0 ? _currentX + 360: _currentX -360 ;
            }
            var auxT = (_targetX - _currentX) > 180 ? _targetX - _currentX - 180 : _targetX - _currentX;
            var aux1 = Mathf.Abs(0 - auxT);
            var aux2 = Mathf.Abs(180 - auxT);
            _transitionRotSpeed = aux1 < aux2 ? -_rotSpeed : _rotSpeed;

        }

        public void Execute()
        {
            float d;
            bool rotDiff = Mathf.Abs(transform.rotation.eulerAngles.y - _targetX) >=45;

            if (rotDiff && !_rotated )
            {
                _currentX += _transitionRotSpeed * Time.deltaTime;
                _distance = _normalStateDistance;
            }
            else
            {
                _currentX = _targetX;
                d = _positionSmoothness < 0.7f ? 10 : 1;
                 _positionSmoothness = _positionSmoothness < 1 ? _positionSmoothness + Time.deltaTime/d : 1;
                _distance = _targetDistance;
            }

            var dir = new Vector3(0, 0, -_distance);
            var rotation = Quaternion.Euler(_targetY, _currentX, 0);
            var targetPosition = _target.position + rotation * dir;

            transform.position = Vector3.Lerp(transform.position, targetPosition, _positionSmoothness);

            transform.LookAt(_target);

            if(Mathf.Abs(Vector3.Distance(transform.position, targetPosition)) < 0.5f && !rotDiff)
            {
                EventManager.DispatchEvent(GameEvent.CAMERA_FIXPOS_END);
                _rotated = true;
            }
        }

        public void Exit()
        {
            
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }
    }

}
