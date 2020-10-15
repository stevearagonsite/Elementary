using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PostProcessingBehaviour))]
public class CameraController : MonoBehaviour {

    #region Global Variables
    //Calculate Zoom Cam Variables
    public bool isInTransition;
    public Transform player;

    private Vector3 zoomAuxPos;
    private Quaternion zoomAuxRot;

    //Constants
    public  float MAX_Y_ANGLE = 50f;
    public  float MIN_Y_ANGLE = -20f;
    //Camera target
    public Transform lookAt;
    public Transform zoomTransform;

    //Smooth camera movement Positions
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;
    [Range(0.1f, 1f)]
    public float positionSmoothness;
    [Range(0.1f,1f)]
    public float rotationSmoothness = 0.3f;
    [Range(0.5f, 5f)]
    public float speed = 1.8f;
    //Collision Manager
    public LayerMask collisionLayer;
    private bool _isColliding = false;
    private Vector3[] _adjustedCameraClipPoints;
    private Vector3[] _desiredCameraClipPoints;

    //Target locate Variables
    private Camera _cam;
    private Vector3 _cameraDirection;
    private RaycastHit _hit;
    private float _distance;
    [SerializeField]
    float unadjustedDistance;
    const int wallLayer = 12;

    //Camera Movement Variables
    private float _currentX;
    private float _currentY = 30.0f;

    //Referencia al gameInput
    private GameInput _I;

    //Post-Processing Variables
    // private PostProcessingProfile _postProcessing;
    private float _apertureNormal = 1.9f;
    private float _apertureZoom = 15f;

    //Fixed Camera Variables
    private bool _isInPosition;
    private float _angleSign;

    public static CameraController instance;

    #endregion

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            instance = null;
            instance = this;
        }

        //Variables Initialization
        _cam = Camera.main;
        _currentPosition = transform.position;
        _currentX = lookAt.eulerAngles.y;
        _distance = unadjustedDistance;
        _adjustedCameraClipPoints = new Vector3[5];
        _desiredCameraClipPoints = new Vector3[5];
        _I = GameInput.instance;
        isInTransition = false;
        //VFX (runtime change values of posproccesing)
        //_postProcessing = GetComponent<PostProcessingBehaviour>().profile;
        //UpdateFocusDistance();
        //UpdateDiaphragm(_apertureNormal);
    }

    #region VFX events

    /*public void UpdateFocusDistance()
    {
        var fd = _postProcessing.depthOfField.settings;
        fd.focusDistance = Vector3.Distance(transform.position, _targetPosition) * 1000;
        _postProcessing.depthOfField.settings = fd;
    }*/

    private void UpdateDiaphragm(float value = 7f)
    {
        // var d = _postProcessing.depthOfField.settings;
        // d.aperture = value;
        // _postProcessing.depthOfField.settings = d;
    }

    #endregion End VFX events

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


            if(Physics.Raycast(ray, dist,collisionLayer))
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

            ray = new Ray(target, _desiredCameraClipPoints[i]-target);

            if(Physics.Raycast(ray, out hit))
            {
                if (dist == -1) dist = hit.distance;
                else if (hit.distance < dist) dist = hit.distance;
            }
        }

        if (dist == -1) return 0;
        else return dist;


    }

    private void CheckColliding(Vector3 target)
    {
        _isColliding = (CollisionDetectedAtClipPoints(_desiredCameraClipPoints, lookAt.transform.position)) ? true : false;
    }
    #endregion

    public void NormalEnter()
    {
        _currentX = lookAt.eulerAngles.y;
        unadjustedDistance = 3.5f;
        //UpdateDiaphragm(_apertureNormal);
    }

    public void ZoomEnter()
    {
        unadjustedDistance = 0.5f;
        isInTransition = true;
        //UpdateDiaphragm(_apertureZoom);
    }

    public void ZoomUpdate()
    {
        //UpdateFocusDistance();

        if (isInTransition)
        {
            _currentPosition += (zoomTransform.position - _currentPosition) / 2;
            transform.position = _currentPosition;
            //transform.rotation = Quaternion.Slerp(transform.rotation, zoomTransform.rotation, rotationSmoothness / 2);
            transform.rotation = zoomTransform.rotation;
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f) isInTransition = false;
        }
        else
        {
            transform.position = zoomTransform.position;
            transform.rotation = zoomTransform.rotation;
        }
    }

    public void NormalUpdate()
    {
        //UpdateFocusDistance();
        var oldX = _currentX;
        var oldY = _currentY;
        _currentX += _I.cameraRotation * speed;
        _currentY += _I.cameraAngle * speed;
        _currentY = Mathf.Clamp(_currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);

        /*if(oldX != _currentX || oldY != _currentY)
        {
            positionSmoothness = 0.5f;
        }*/

        var rawDir = new Vector3(0, 0, -unadjustedDistance);
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);

        var rawPosition = lookAt.position + rotation * rawDir;

        //Calculate camera distance after checking collisions
        UpdateCameraClipPoints(transform.position, transform.rotation, ref _adjustedCameraClipPoints);
        UpdateCameraClipPoints(rawPosition, transform.rotation, ref _desiredCameraClipPoints);
        CheckColliding(lookAt.position);


        if (_isColliding)
        {
            _distance = GetAdjustedDistance(lookAt.position);
        }
        else
        {
            _distance = unadjustedDistance;
        }

        var dir = new Vector3(0, 0, -_distance);

        _targetPosition = lookAt.position + rotation * dir;

        //camera move
        //_currentPosition = _currentPosition + (_targetPosition - _currentPosition) / positionSmoothness;
        //transform.position = _currentPosition;
        //transform.position = Vector3.Lerp(_currentPosition, _targetPosition, Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, _targetPosition, positionSmoothness);
        //transform.position = _targetPosition;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAt.position - transform.position), rotationSmoothness);
        transform.rotation = Quaternion.LookRotation(lookAt.position - transform.position);
    }

    public void FixedCamEnter()
    {

    }

    public void FixedCamUpdate()
    {

    }

    public void ChangeDistance(float distance)
    {
        unadjustedDistance = distance;
    }

    public void ChangeSmoothness(float smooth)
    {
        smooth = Mathf.Clamp01(smooth);

        positionSmoothness = smooth;
    }
}
