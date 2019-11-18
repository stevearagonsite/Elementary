using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<InputT>{

    private IState<InputT> currentState;
    public IState<InputT> Current { get { return currentState; } }

    private IState<InputT> lastState;
    public IState<InputT> Last { get { return lastState; } }

    public FSM(IState<InputT> initialState)
    {
        initialState.Enter();
        currentState = initialState;
        lastState = currentState;
    }

    public void Execute()
    {
        currentState.Execute();
    }

    public void ProcessInput(InputT input)
    {
        var currentStateTransitions = currentState.Transitions;
        if (currentStateTransitions.ContainsKey(input))
        {
            currentState.Exit();
            lastState = currentState;
            currentState = currentStateTransitions[input];
            currentState.Enter();
        }
    }

}
