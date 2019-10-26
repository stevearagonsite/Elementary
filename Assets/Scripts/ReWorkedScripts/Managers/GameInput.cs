using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using XInputDotNetPure;

public class GameInput : MonoBehaviour
{

    private static GameInput _instance;
    public static GameInput instance { get { return _instance; } }

    #region buttons
    //Buttons
    [HideInInspector]
    public bool jumpButton;
    [HideInInspector]
    public bool initialJumpButton;
    [HideInInspector]
    public bool sprintButton;
    [HideInInspector]
    public bool crouchButton;
    [HideInInspector]
    public bool skillButton;
    [HideInInspector]
    public bool absorbButton;
    [HideInInspector]
    public bool blowUpButton;
    [HideInInspector]
    public bool aimButton;
    [HideInInspector]
    public bool shootButton;
    private bool axisToKeydown = false;
    [HideInInspector]
    public bool aimToShootButton;
    [HideInInspector]
    public bool swapUp;
    [HideInInspector]
    public bool swapDown;
    //Move Axis
    [HideInInspector]
    public float horizontalMove;
    [HideInInspector]
    public float verticalMove;
    [HideInInspector]
    public bool chatButton;


    [HideInInspector]
    public bool onDirectionChange;

    //Cammera
    [HideInInspector]
    public float cameraRotation;
    [HideInInspector]
    public float cameraAngle;
    [HideInInspector]
    public bool centrateCamera;

    //Skills Switch
    [HideInInspector]
    public bool skillUp;
    [HideInInspector]
    public bool skillDown;

    //Debug
    [HideInInspector]
    public bool consoleButton;
    #endregion

    //future configurations
    [HideInInspector]
    public bool config1;
    [HideInInspector]
    public bool config2;

    //use joystick or not
    public static bool joystick;

    //Camera Speed
    const float joystickSpeed = 4;
    const float keyboardSpeed = 1.8f;


    //Buttons
    public int _numSkill;

    //LockFeatures
    Dictionary<Features, bool> _featuresDic;

    public GameObject debugConsole;
    public DebugConsole console;
    bool consoleToggle;


	void Awake ()
    {
        if(_instance == null)
            _instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        /*_featuresDic = new Dictionary<Features, bool>();

        _featuresDic.Add(Features.JUMP, false);
        _featuresDic.Add(Features.SKILL_CHANGE, true);
        _featuresDic.Add(Features.STEALTH, false);*/

        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        consoleToggle = false;
        ToggleConsole();
    }

    void Execute()
    {
        //CheckIfJoystickConnected();
        //UpdateCameraValues();//Camera update (Change observer)
        /*if (joystick)
        {
            sprintButton = Input.GetButton("Sprint");
            if (_featuresDic[Features.STEALTH])
                crouchButton = Input.GetButton("Crouch");
            //centrateCamera = Input.GetKeyDown(KeyCode.Joystick1Button8);
            //skillButton = Input.GetKeyDown(KeyCode.Joystick1Button2);
            if (_featuresDic[Features.JUMP])
            {
                initialJumpButton = Input.GetButtonDown("Jump");
                jumpButton = Input.GetButton("Jump");
            }
            absorbButton =   Input.GetAxisRaw("Aspire") > 0;
            blowUpButton = Input.GetAxisRaw("Blow") > 0;
            cameraRotation = Mathf.Abs(Input.GetAxis("HorizontalJ2")) > 0.1f ? Input.GetAxis("HorizontalJ2"): 0;
            cameraAngle = Input.GetAxis("VerticalJ2");
            aimButton = Input.GetButtonDown("Aim");
            horizontalMove = Input.GetAxisRaw("HorizontalJ1");
            verticalMove = Input.GetAxisRaw("VerticalJ1");

            aimToShootButton = Input.GetAxisRaw("Aspire") > 0;
            shootButton = AxisToGetKeyDown("Blow");

            if (_featuresDic[Features.SKILL_CHANGE])
            {
                skillUp = Input.GetButtonDown("Skills");
                skillDown = Input.GetButtonDown("Skills");
            }

            swapUp = Input.GetAxis("Swap") > 0;
            swapDown = Input.GetAxis("Swap") < 0;

            chatButton = Input.GetButtonDown("Jump");
        }
        else
        {*/
            sprintButton = Input.GetKey(KeyCode.LeftShift);
            //if(_featuresDic[Features.STEALTH])
                crouchButton = Input.GetKey(KeyCode.LeftControl);
            //centrateCamera = Input.GetKeyDown(KeyCode.Mouse2);
            //skillButton = Input.GetKeyDown(KeyCode.E);
            //if (_featuresDic[Features.JUMP])
            //{
                initialJumpButton = Input.GetKeyDown(KeyCode.Space);
                jumpButton = Input.GetKey(KeyCode.Space);

            //}
            absorbButton = Input.GetKey(KeyCode.Mouse1);
            blowUpButton = Input.GetKey(KeyCode.Mouse0);
            cameraRotation = Input.GetAxis("Mouse X");
            cameraAngle = -Input.GetAxis("Mouse Y");
            aimButton = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse2);//Alternative input.

            aimToShootButton = Input.GetKey(KeyCode.Mouse1);
            shootButton = Input.GetKeyDown(KeyCode.Mouse0);
            
            horizontalMove = Input.GetAxis("Horizontal");
            verticalMove = Input.GetAxis("Vertical");

            //if (_featuresDic[Features.SKILL_CHANGE])
            //{
                skillUp = Input.GetKeyDown(KeyCode.E);
                skillDown = Input.GetKeyDown(KeyCode.Q);
            //}

            swapUp = Input.GetAxis("Mouse ScrollWheel") > 0;
            swapDown = Input.GetAxis("Mouse ScrollWheel") < 0;

            chatButton = Input.GetKeyDown(KeyCode.Space);
        //}


        consoleButton = Input.GetKeyDown(KeyCode.F11);
        if (consoleButton)
        {
            consoleToggle = !consoleToggle;
            ToggleConsole();
        }

        onDirectionChange = (Math.Abs(horizontalMove) <= 0.2f && Math.Abs(verticalMove) <= 0.2f);
    }

    private void ToggleConsole()
    {
        if (!consoleToggle)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        debugConsole.SetActive(consoleToggle);
        console.enabled = consoleToggle;
    }

    private void UpdateCameraValues()//Replace observer
    {
        if (joystick)
        {
            CameraController.instance.speed = 4;
            //CameraController.instance.rotationSmoothness = 0.9f;
        }
        else
        {
            CameraController.instance.speed = 1.8f;
            //CameraController.instance.rotationSmoothness = 0.5f;
        }
    }

    private void CheckIfJoystickConnected()
    {
        if (Input.GetJoystickNames().Length > 0)//State selection input.
            if (jostickInputListener() || Input.anyKeyDown)
                joystick = jostickInputListener() ? true : false;
    }

    private static bool jostickInputListener()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            return false;

        for (int i = 330; i <= 369; i++)//Xbox controller keycode.
            if (Input.GetKeyDown((KeyCode)i))
                return true;

        if (Input.GetAxis("HorizontalJ1") != 0 || Input.GetAxis("VerticalJ1") != 0)
            return true;
        if (Input.GetAxis("HorizontalJ2") != 0 || Input.GetAxis("VerticalJ2") != 0)
            return true;
        if (Input.GetAxis("Blow") != 0 || Input.GetAxis("Aspire") != 0)
            return true;
        if (Input.GetAxis("Skills") != 0 || Input.GetAxis("Swap") != 0)
            return true;

        return false;
    }

    bool AxisToGetKeyDown(string axis)
    {
        if (Input.GetAxisRaw(axis) > 0.1f)
        {
            if (!axisToKeydown)
            {
                axisToKeydown = true;
                return true;
            }

        }
        else
        {
            axisToKeydown = false;
        }
        return false;
    }

    public void ChangeLockFeature(Features ft, bool active)
    {
        _featuresDic[ft] = active;
    }

    public bool GetLockFeature(Features ft)
    {
        return _featuresDic[ft];
    }

    public enum Features
    {
        NONE,
        JUMP,
        SKILL_CHANGE,
        STEALTH
    }
}
