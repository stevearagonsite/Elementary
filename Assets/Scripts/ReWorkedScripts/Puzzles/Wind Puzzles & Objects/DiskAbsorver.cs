using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskAbsorver : MonoBehaviour, IVacuumObject {

    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    public bool isAbsorved { get { return _isAbsorved; } set { _isAbsorved = value; } }
    public bool isAbsorvable { get { return _isAbsorvable; } }
    public bool isBeeingAbsorved { get { return _isBeeingAbsorved; } set { _isBeeingAbsorved = value; } }
    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    Vector3 initialPosition;

    public float speedAttenuator;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        initialPosition = transform.position; 
	}
	
	void Execute()
    {
		if(!_isBeeingAbsorved)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.5f);
            transform.position = Vector3.Lerp(transform.position, initialPosition, 0.5f);
        }
        _isBeeingAbsorved = false;
    }

    public void SuckIn(Transform origin, float atractForce)
    {
        _isBeeingAbsorved = true;
        var dir = (transform.position - origin.position).normalized;
        var distance = Vector3.Distance(transform.position, origin.position);
        transform.position -= dir * atractForce/speedAttenuator * Time.deltaTime;
        if(Mathf.Abs(distance) < 1 && Mathf.Abs(distance) > 0.5f)
        {
            transform.localScale = Vector3.one * Mathf.Abs(distance);  
        }
    }

    public void BlowUp(Transform origin, float atractForce, Vector3 direction){}
    public void ReachedVacuum(){}
    public void Shoot(float shootForce, Vector3 direction){}
    public void ViewFX(bool active){}
    public void Exit(){}
}
