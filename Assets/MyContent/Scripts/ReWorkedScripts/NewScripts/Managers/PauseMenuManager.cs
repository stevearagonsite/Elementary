using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    Canvas _canvas;

    bool _isEnabled;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        InputManager.instance.AddAction(InputType.Pause, OnPause);
    }

    private void OnPause()
    {
        _isEnabled = !_isEnabled;
        _canvas.enabled = _isEnabled;
        Cursor.visible = _isEnabled;
        Cursor.lockState = _isEnabled ? CursorLockMode.None: CursorLockMode.Locked;

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
