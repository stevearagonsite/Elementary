using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    void Start()
    {
        CameraManager.instance.RegisterCamera(GetComponent<Camera>());
    }
}
