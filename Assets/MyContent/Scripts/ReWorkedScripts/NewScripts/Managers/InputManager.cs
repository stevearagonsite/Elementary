using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public delegate void Actions();
    public delegate void Move(Vector2 dir);

    //Singleton
    private static InputManager _instance;
    public static InputManager instance { get { return _instance; } }

    private Dictionary<InputType, Actions> _actions;
    private Dictionary<InputType, Move> _moveActions;

    [Range(0,0.3f)]
    public float axisOffset = 0.1f;

    public KeyCode jump;
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
    }

    /// <summary>
    /// Execute Every frame
    /// </summary>
    void Execute()
    {
        if (Input.GetKeyDown(jump))
        {
            if (_actions.ContainsKey(InputType.Jump))
            {
                _actions[InputType.Jump]();
            }
        }

        if (Input.GetKey(jump))
        {
            if (_actions.ContainsKey(InputType.Jump_Held))
            {
                _actions[InputType.Jump_Held]();
            }
        }

        
        if (_moveActions.ContainsKey(InputType.Movement))
        {
            _moveActions[InputType.Movement](new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        if (_moveActions.ContainsKey(InputType.Cursor) && (mouseX != 0 || mouseY != 0))
        {
            _moveActions[InputType.Cursor](new Vector2(mouseX, mouseY));
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

}
    
public enum InputType
{
    Jump,
    Movement,
    Jump_Held,
    Cursor
}



