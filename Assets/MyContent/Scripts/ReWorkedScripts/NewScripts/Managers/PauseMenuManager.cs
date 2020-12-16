using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PauseMenuManager : MonoBehaviour
{
    Canvas _canvas;

    bool _isEnabled;

    public float blurAmount = 0.003f;
    public Material blur;
    private void Start()
    {
        BlurImage(false);
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        InputManager.instance.AddAction(InputType.Pause, OnPause);
    }

    private void OnPause()
    {
        _isEnabled = !_isEnabled;
        _canvas.enabled = _isEnabled;
        BlurImage( _isEnabled);
        Cursor.visible = _isEnabled;
        Cursor.lockState = _isEnabled ? CursorLockMode.None: CursorLockMode.Locked;

    }

    private void BlurImage(bool val)
    {
        if (val)
        {
            blur.SetFloat("_BlurValue", blurAmount);
        }
        else
        {
            blur.SetFloat("_BlurValue", 0);
        }
    }

    public void Resume()
    {
        InputManager.instance.UnPauseGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        //Go to main menu
    }
}
