using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWindEngine : MonoBehaviour {

    public ElevatorPlatform plat;
    public float speedMultiplier;
    public float maxSpeed;
    Vector3 _rotation;

	void Start ()
    {
        plat = GetComponentInParent<ElevatorPlatform>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	void Execute ()
    {
        _rotation.z = plat.currentSpeed * speedMultiplier * Time.deltaTime;
        _rotation.z = Mathf.Clamp(_rotation.z, 0, maxSpeed);
        transform.Rotate(_rotation);
	}

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
