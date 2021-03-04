using UnityEngine;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour
{
    public delegate void Actions();
    public delegate void Move(Vector2 dir);

    //Singleton
    private static InputManager _instance;
    public static InputManager instance { get { return _instance; } }

    private Dictionary<InputType, Actions> _actions;
    private Dictionary<InputType, Move> _moveActions;

    private bool _isGamePaused;
    private bool _canGameBePaused;

    [Range(0,0.3f)]
    public float axisOffset = 0.1f;

    public KeyCode jump;
    public KeyCode absorb;
    public KeyCode reject;
    public KeyCode skillUp;
    public KeyCode skillDown;
    public KeyCode sprint;
    public KeyCode walk;
    public KeyCode pause;
    public KeyCode test;
    /// <summary>
    /// Initialize
    /// </summary>
    void Awake()
    {
        if (_instance == null) _instance = this;
        _actions = new Dictionary<InputType, Actions>();
        _moveActions = new Dictionary<InputType, Move>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);

        EventManager.AddEventListener(GameEvent.STORY_START, CantBePaused);
        EventManager.AddEventListener(GameEvent.START_LOAD_SCENE, CantBePaused);
        EventManager.AddEventListener(GameEvent.START_LEVEL_TRANSITION, CantBePaused);


        EventManager.AddEventListener(GameEvent.STORY_END, CanBePaused);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_FINISH, CanBePausedAfterChangeLevel);
        EventManager.AddEventListener(GameEvent.START_GAME, CanBePaused);
    }

    private void CantBePaused(object[] parameterContainer)
    {
        _canGameBePaused = false;
    }

    private void CanBePausedAfterChangeLevel(object[] p)
    {
        if(GameObject.Find("Character") != null)
        {
            CanBePaused(null);
        }
    }

    private void CanBePaused(object[] parameterContainer)
    {
        _canGameBePaused = true;
    }

    /// <summary>
    /// Execute Every frame
    /// </summary>
    void Execute()
    {
        
        if (Input.GetKeyDown(jump))
        {
            if (_actions.ContainsKey(InputType.Jump) && !_isGamePaused)
            {
                _actions[InputType.Jump]();
            }
        }

        if (Input.GetKey(jump))
        {
            if (_actions.ContainsKey(InputType.Jump_Held) && !_isGamePaused)
            {
                _actions[InputType.Jump_Held]();
            }
        }

        
        if (_moveActions.ContainsKey(InputType.Movement))
        {
            if(!_isGamePaused)
                _moveActions[InputType.Movement](new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            else
                _moveActions[InputType.Movement](new Vector2(0, 0));
        }
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        if (_moveActions.ContainsKey(InputType.Cursor) && (mouseX != 0 || mouseY != 0))
        {
            if (!_isGamePaused)
                _moveActions[InputType.Cursor](new Vector2(mouseX, mouseY));
            else
                _moveActions[InputType.Cursor](new Vector2(0, 0));

        }

        if (Input.GetKey(absorb)) 
        {
            if (_actions.ContainsKey(InputType.Absorb) && !_isGamePaused)
            {
                _actions[InputType.Absorb]();
            }
        }
        if (Input.GetKey(reject))
        {
            if (_actions.ContainsKey(InputType.Reject) && !_isGamePaused)
            {
                _actions[InputType.Reject]();
            }
        }
        if ((Input.GetKeyUp(absorb) || Input.GetKeyUp(reject)) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Stop))
            {
                _actions[InputType.Stop]();
            }
        }
        if (Input.GetKeyDown(skillUp) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Skill_Up))
            {
                _actions[InputType.Skill_Up]();
            }
        }

        if (Input.GetKeyDown(skillDown) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Skill_Down))
            {
                _actions[InputType.Skill_Down]();
            }
        }

        if (Input.GetKey(sprint) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Sprint))
            {
                _actions[InputType.Sprint]();
            }
        }

        if(Input.GetKey(walk) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Walk))
            {
                _actions[InputType.Walk]();
            }
        }

        if (Input.GetKeyDown(test) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Test))
            {
                _actions[InputType.Test]();
            }
        }
        if(Input.GetKeyDown(reject) && !_isGamePaused)
        {
            if (_actions.ContainsKey(InputType.Skip_Dialogue) && _actions[InputType.Skip_Dialogue] != null)
            {
                _actions[InputType.Skip_Dialogue]();
            }
        }
    }

    //Things we must check even if the game is paused
    private void Update()
    {
        if (_canGameBePaused && Input.GetKeyDown(pause))
        {
            _isGamePaused = !_isGamePaused;
            if (_actions.ContainsKey(InputType.Pause))
            {
                _actions[InputType.Pause]();
            }
        }
    }

    /// <summary>
    /// Add Action to delegate dictionary
    /// </summary>
    /// <param name="input"></param>
    /// <param name="listener"></param>
    public void AddAction(InputType input, Actions listener)
    {
        if (_actions == null)
        {
            _actions = new Dictionary<InputType, Actions>();
        }
        if (!_actions.ContainsKey(input))
        {
            _actions.Add(input, null);
        }
        _actions[input] += listener;
    }

    /// <summary>
    /// Add Action to move delegate dictionary
    /// </summary>
    /// <param name="input"></param>
    /// <param name="listener"></param>
    public void AddAction(InputType input, Move listener)
    {
        if (_moveActions == null)
        {
            _moveActions = new Dictionary<InputType, Move>();
        }
        if (!_moveActions.ContainsKey(input))
        {
            _moveActions.Add(input, null);
        }
        _moveActions[input] += listener;
    }

    /// <summary>
    /// Remove Action from deleate
    /// </summary>
    /// <param name="input"></param>
    /// <param name="listener"></param>
    public void RemoveAction(InputType input, Actions listener)
    {
        if (_actions != null)
        {
            if (_actions.ContainsKey(input))
            {
                _actions[input] -= listener;
            }
        }
    }

    /// <summary>
    /// Remove Move Action from delegate
    /// </summary>
    /// <param name="input"></param>
    /// <param name="listener"></param>
    public void RemoveAction(InputType input, Move listener)
    {
        if (_moveActions != null)
        {
            if (_moveActions.ContainsKey(input))
            {
                _moveActions[input] -= listener;
            }
        }
    }

    public void UnPauseGame()
    {
        if (_isGamePaused)
        {
            _isGamePaused = !_isGamePaused;
            if (_actions.ContainsKey(InputType.Pause))
            {
                _actions[InputType.Pause]();
            }
        }
    }

}
    
public enum InputType
{
    Jump,
    Movement,
    Jump_Held,
    Cursor,
    Absorb,
    Reject,
    Stop,
    Skill_Down,
    Skill_Up,
    Sprint,
    Walk,
    Pause,
    Skip_Dialogue,
    Test
}



