using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{

    private static SceneLoadManager _instance;
    public static SceneLoadManager instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null) _instance = this;
    }
}
