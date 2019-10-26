using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionFanRotator : MonoBehaviour {

    public Vector3 rot;

	void Start () {

        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	// Update is called once per frame
	void Execute () {
        transform.Rotate(rot * Time.deltaTime);
	}

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
