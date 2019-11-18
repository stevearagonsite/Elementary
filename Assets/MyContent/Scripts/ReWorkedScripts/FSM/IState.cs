using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<InputT> {

    void Enter();
    void Execute();
    void Exit();
    Dictionary<InputT, IState<InputT>> Transitions { get; }

}
