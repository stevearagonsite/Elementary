using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cloth))]
public class ClothPause : MonoBehaviour
{
    private Cloth _cloth;
    private bool _isGamePaused;
    void Start()
    {
        _cloth = GetComponent<Cloth>();
        InputManager.instance.AddAction(InputType.Pause, OnPause);
    }

    private void OnPause()
    {
        _isGamePaused = !_isGamePaused;
        _cloth.enabled = !_isGamePaused;
    }
}
