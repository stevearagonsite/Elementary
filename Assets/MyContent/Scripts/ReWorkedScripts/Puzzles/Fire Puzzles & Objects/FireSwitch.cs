using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwitch : MonoBehaviour, IFlamableObjects {

    #region Delegates

    public delegate void OnSwitch();
    public delegate void OnSwitchIncrease();

    OnSwitch callbacks;
    OnSwitchIncrease increaseCallbacks;

    public void AddOnSwitchEvent(OnSwitch callback) {
        callbacks += callback;
    }

    public void RemoveOnSwitchEvent(OnSwitch callback) {
        callbacks -= callback;
    }

    public void AddOnSwitchIncreaseEvent(OnSwitchIncrease callback) {
        increaseCallbacks += callback;
    }

    public void RemoveOnSwitchIncreaseEvent(OnSwitchIncrease callback) {
        increaseCallbacks -= callback;
    }

    #endregion

    bool _isOnFire;
    public float life;
    public float burnSpeed;

    public bool isBurned;

    public bool isOnFire {
        get { return _isOnFire; }
        set { _isOnFire = value; }
    }

    public void SetOnFire() 
    {
        if (!isBurned) 
        {
            _isOnFire = true;
            if(increaseCallbacks != null)
                increaseCallbacks();
            life -= Time.deltaTime * burnSpeed;
            if (life < 0) 
            {
                isBurned = true;
                if(callbacks != null)   
                    callbacks();
            }
        }
    }



}
