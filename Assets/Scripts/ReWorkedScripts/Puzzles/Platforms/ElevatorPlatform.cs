using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlatform : Platform, IVacuumObject
{

    bool goUp;

    public float speed;
    public float lerpValue;
    float _currentSpeed;

    public float minY;
    public float maxY;

    public float maxYOffset;

    public float gravityValue;

    #region VacuumObject
    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    public bool isAbsorved { get { return _isAbsorved; } set { _isAbsorved = value; } }
    public bool isAbsorvable { get { return _isAbsorvable; } }
    public bool isBeeingAbsorved { get { return _isBeeingAbsorved; } set { _isBeeingAbsorved = value; } }
    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    public float currentSpeed { get { return _currentSpeed; } set { _currentSpeed = value; } }

    public void SuckIn(Transform origin, float atractForce){}

    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        if (origin.position.y > transform.position.y)
        {
            ActivateElevate();
        }
    }
    public void Exit(){}
    public void ReachedVacuum(){}
    public void Shoot(float shootForce, Vector3 direction){}
    public void ViewFX(bool active){}

    public void ActivateElevate()
    {
        goUp = true;
    }
    #endregion

    

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	void Execute ()
    {
        if (goUp)
        {
            if (transform.position.y <= maxY - maxYOffset)
                _currentSpeed = Mathf.Lerp(_currentSpeed, speed, lerpValue);
            else if (transform.position.y <= maxY)
                _currentSpeed = Mathf.Lerp(_currentSpeed, gravityValue, lerpValue);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, lerpValue);
        }

        if(transform.position.y >= minY && !goUp)
        {
            transform.position -= transform.up * gravityValue * Time.deltaTime;
            
        }
        
        if(transform.position.y <= maxY)
        {
            transform.position += transform.up * Time.deltaTime * _currentSpeed;
        }

        _rb.isKinematic = true;
        goUp = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, minY, transform.position.z), 0.5f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, maxY, transform.position.z), 0.5f);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
