using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sampler : MonoBehaviour {

    public GameObject[] samples;
    public Vector3 rotation;


    private void Update()
    {
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i].transform.Rotate(rotation * Time.deltaTime);
        }
    }
}
