using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectiveManager : MonoBehaviour
{
    private static GameObjectiveManager _instance;
    public static GameObjectiveManager instance { get { return _instance; } set { _instance = value; } }

    //In case we want more than one goal, we can do this a Dictionary 
    private bool _hasGoals;

    void Awake()
    {
        if (_instance == null) _instance = this;
    }


    public bool CheckEndOfLevelGoals()
    {
        return _hasGoals;
    }

    public void ActivateKeyHold()
    {
        _hasGoals = true;
    }

    public void ResetKeyHold()
    {
        _hasGoals = false;
    }

}
