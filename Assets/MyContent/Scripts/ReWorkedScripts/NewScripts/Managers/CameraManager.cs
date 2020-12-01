using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    void Start()
    {
        
    }

    
}
