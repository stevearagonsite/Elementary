using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPCamera
{
    public class CameraFSM : MonoBehaviour
    {

        [Header("Normal State Variables")]
        public Transform _lookAt;
        [Range(0f,1f)]
        public float positionSmoothness;
        [Range(0f, 5f)]
        public float speed = 1.8f;
        public float unadjustedDistance;
        public LayerMask collisionLayer;
        Camera _cam;
        GameInput _I;

        //Constants
        const float MAX_Y_ANGLE = 50f;
        const float MIN_Y_ANGLE = -20f;

        //Camera Stats
        float _currentX;
        float _currentY;
        float _actualDistance;
        float _collisionOffset;

        float _distance;
        bool _isColliding;

        //Camera Smooth Positions Variables
        Vector3 _targetPosition;
        float _actualPositionSmoothness;

        //Camera Collision Check vartiables
        Vector3[] _adjustedCameraClipPoints;
        Vector3[] _desiredCameraClipPoints;


        void Awake()
        {
            _cam = GetComponent<Camera>();
            _I = GameInput.instance;
        }
        // Use this for initialization
        void Start ()
        {
            UpdatesManager.instance.AddUpdate(UpdateType.LATE, Execute);
            CameraManager.instance.RegisterMainCamera(this);
            InputManager.instance.AddAction(InputType.Cursor, UpdateMouse);
	    }


       
        void Execute () {
            _currentY = Mathf.Clamp(_currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);
            _actualDistance = Mathf.Lerp(_actualDistance, unadjustedDistance, Time.deltaTime * 3);

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
            if (_isColliding)
            {
                _actualPositionSmoothness = positionSmoothness;
            }
            else
            {
                if (_actualPositionSmoothness < 1)
                    _actualPositionSmoothness += Time.deltaTime;
                //We need this when we exit terrain collision
            }
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _actualPositionSmoothness);

            transform.rotation = Quaternion.LookRotation(_lookAt.position - transform.position);
        }

        void UpdateMouse(Vector2 cursor)
        {
            _currentX += cursor.x * speed;
            _currentY -= cursor.y * speed;
        }

        public void GoToStartPosition()
        {
            _currentX = 0;
            _currentY = 0;
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


                if (Physics.Raycast(ray, dist, collisionLayer))
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

                if (Physics.Raycast(ray, out hit, unadjustedDistance + 1, collisionLayer, QueryTriggerInteraction.Ignore))
                {
                    if (dist == -1) dist = hit.distance;
                    else if (hit.distance < dist) dist = hit.distance;
                }
            }


            if (dist == -1) return unadjustedDistance;
            else return dist;


        }

        private void CheckColliding(Vector3 target)
        {
            _isColliding = (CollisionDetectedAtClipPoints(_desiredCameraClipPoints, _lookAt.transform.position)) ? true : false;
        }
        #endregion
        private void OnDestroy()
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
            CameraManager.instance.UnregisterMainCamera();
        }
    }

}
