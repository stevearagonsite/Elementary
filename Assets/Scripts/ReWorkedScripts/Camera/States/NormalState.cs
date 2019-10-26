using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPCamera
{
    public class NormalState : IState<Inputs>
    {
        Dictionary<Inputs, IState<Inputs>> _transitions;

        //Constants
        const float MAX_Y_ANGLE = 50f;
        const float MIN_Y_ANGLE = -20f;

        //Camera target
        Transform _lookAt;
        Transform transform;

        //Keyboard-Joystick Inputs
        GameInput _I;

        //Camera Stats
        float _currentX;
        float _currentY = 30.0f;
        float _speed;
        float _unadjustedDistance;
        float _actualDistance;
        float _collisionOffset;

        public float unadjustedDistance {
            get { return _unadjustedDistance; }
            set {_unadjustedDistance = value; }
        }
        float _distance;
        bool _isColliding;

        //Camera Smooth Positions Variables
        Vector3 _targetPosition;
        float _positionSmoothness;

        /// <summary>
        /// Value between 0 and 1
        /// </summary>
        public float positionSmoothness {
            get { return _positionSmoothness; }
            set {
                var undist = value;
                undist = Mathf.Clamp01(undist);
                _positionSmoothness = undist;
                }
            }

        //Camera Collision Check vartiables
        Vector3[] _adjustedCameraClipPoints;
        Vector3[] _desiredCameraClipPoints;
        LayerMask _collisionLayer;
        LayerMask _distanceCollisionLayer;
        Camera _cam;

        public NormalState(Transform lookAt, Transform t, float speed, float positionSmoothness, float unadjustedDistance, Camera cam, LayerMask collisionLayer, GameInput I)
        {
            _lookAt = lookAt;
            transform = t;
            _speed = speed;
            _positionSmoothness = positionSmoothness;
            _cam = cam;
            _collisionLayer = collisionLayer;
            _distanceCollisionLayer = ~_collisionLayer;
            _unadjustedDistance = unadjustedDistance;
            _I = I;
            _collisionOffset = 0.2f;

            //Variables Initialization
            _currentX = _lookAt.eulerAngles.y;
            _distance = _unadjustedDistance;
            _adjustedCameraClipPoints = new Vector3[5];
            _desiredCameraClipPoints = new Vector3[5];

        }

        public void Enter()
        {
            _currentX = transform.eulerAngles.y;
            _currentY = transform.eulerAngles.x;
            _actualDistance = Vector3.Distance(transform.position, _lookAt.position);
        }

        public void Execute()
        {
            
            if (_I != null)
            {
                _currentX += _I.cameraRotation * _speed;
                _currentY += _I.cameraAngle * _speed;
            }
            else _I = GameInput.instance;
            _currentY = Mathf.Clamp(_currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
            _actualDistance = Mathf.Lerp(_actualDistance, _unadjustedDistance, Time.deltaTime * 3);

            var rawDir = new Vector3(0, 0, -_actualDistance);
            Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);

            var rawPosition = _lookAt.position + rotation * rawDir;

            //Calculate camera distance after checking collisions
            UpdateCameraClipPoints(transform.position, transform.rotation, ref _adjustedCameraClipPoints);
            UpdateCameraClipPoints(rawPosition, transform.rotation, ref _desiredCameraClipPoints);
            CheckColliding(_lookAt.position);


            if (_isColliding)
            {
                _distance = GetAdjustedDistance(_lookAt.position) - _collisionOffset;
            }
            else
            {
                _distance = _actualDistance;
            }

            var dir = new Vector3(0, 0, -_distance);

            _targetPosition = _lookAt.position + rotation * dir;

            //camera move
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _positionSmoothness);
            transform.rotation = Quaternion.LookRotation(_lookAt.position - transform.position);
        }

        public void Exit()
        {
            
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }

        public float currentY { get { return _currentY; } }

        public void SetInitialPosition(Transform tr)
        {
            transform.position = tr.position;
            _currentX = tr.eulerAngles.y;
            _currentY = tr.eulerAngles.x;
            _actualDistance = Vector3.Distance(tr.position, _lookAt.position);
        }

        #region Camera Collision Manager
        private void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            intoArray = new Vector3[5];

            var z = _cam.nearClipPlane;
            var x = Mathf.Tan(_cam.fieldOfView / Mathf.PI) * z;
            var y = x / _cam.aspect;

            //corners
            //top left
            intoArray[0] = (atRotation * new Vector3(-x, y, z) + cameraPosition);
            //top right
            intoArray[1] = (atRotation * new Vector3(x, y, z) + cameraPosition);
            //down left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z) + cameraPosition);
            //down right
            intoArray[3] = (atRotation * new Vector3(x, -y, z) + cameraPosition);
            //cam position
            intoArray[4] = cameraPosition - _cam.transform.forward;
        }

        private bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 target)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray;
                float dist;
                ray = new Ray(target, clipPoints[i] - target);
                dist = Vector3.Distance(clipPoints[i], target);


                if (Physics.Raycast(ray, dist, _collisionLayer))
                {
                    return true;
                }
            }
            return false;
        }

        private float GetAdjustedDistance(Vector3 target)
        {
            float dist = -1;

            for (int i = 0; i < _desiredCameraClipPoints.Length; i++)
            {
                Ray ray;
                RaycastHit hit;

                ray = new Ray(target, _desiredCameraClipPoints[i] - target);

                if (Physics.Raycast(ray, out hit,_unadjustedDistance + 1 ,_collisionLayer,QueryTriggerInteraction.Ignore))
                {
                    if (dist == -1) dist = hit.distance;
                    else if (hit.distance < dist) dist = hit.distance;
                }
            }


            if (dist == -1) return _unadjustedDistance;
            else return dist;


        }

        private void CheckColliding(Vector3 target)
        {
            _isColliding = (CollisionDetectedAtClipPoints(_desiredCameraClipPoints, _lookAt.transform.position)) ? true : false;
        }
        #endregion
    }

}
