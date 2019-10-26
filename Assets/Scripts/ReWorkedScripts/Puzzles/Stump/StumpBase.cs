using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpBase : MonoBehaviour{

    Renderer rend;
    FireSwitch fSwitch;

    bool _isBurned;

    public bool isBurned { get { return _isBurned; } }

    private void Start()
    {
        rend = GetComponent<Renderer>();
        fSwitch = GetComponent<FireSwitch>();
        fSwitch.AddOnSwitchEvent(IsBurned);
        fSwitch.AddOnSwitchIncreaseEvent(IsOnFire);
    }

    void IsOnFire() 
    {
        rend.material.color = new Color(1 - fSwitch.life / 1000, 0, 0);
    }

    void IsBurned() 
    {
        _isBurned = true;
    }

}
