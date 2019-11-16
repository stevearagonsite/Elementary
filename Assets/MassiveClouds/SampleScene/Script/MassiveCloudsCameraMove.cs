using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassiveCloudsCameraMove : MonoBehaviour
{

	public float velocity = 100;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey("w")) transform.position += transform.forward * velocity;
		if (Input.GetKey("a")) transform.position -= transform.right * velocity;
		if (Input.GetKey("s")) transform.position -= transform.forward * velocity;
		if (Input.GetKey("d")) transform.position += transform.right * velocity;
		if (Input.GetKey("e")) transform.position += transform.up * velocity;
		if (Input.GetKey("q")) transform.position -= transform.up * velocity;
	}
}
