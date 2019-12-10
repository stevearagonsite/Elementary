using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyShooter : MonoBehaviour {

	public IvyCaster ivyCaster;
	public Transform cameraTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			Ray ray = new Ray (cameraTransform.position + cameraTransform.forward * 0.5f, cameraTransform.forward);
			RaycastHit RC;
			if (Physics.Raycast (ray, out RC)) {
				ivyCaster.CastIvy (RC.point, RC.normal);
			}
		}
	}
}