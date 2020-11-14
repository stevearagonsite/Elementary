﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
    public string[] scenesToLoadAtStart;
    void Awake()
    {
        for (int i = 0; i <scenesToLoadAtStart.Length; i++)
        {
            SceneManager.LoadScene(scenesToLoadAtStart[i], LoadSceneMode.Additive);
        }
    }
}

