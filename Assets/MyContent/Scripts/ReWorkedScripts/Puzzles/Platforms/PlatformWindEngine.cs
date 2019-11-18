using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWindEngine : MonoBehaviour {

    public ElevatorPlatform plat;

    Vector3 _rotation;

	void Start ()
    {
        plat = GetComponentInParent<ElevatorPlatform>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	void Execute ()
    {
        _rotation.z = plat.currentSpeed;
        transform.Rotate(_rotation);
	}

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
