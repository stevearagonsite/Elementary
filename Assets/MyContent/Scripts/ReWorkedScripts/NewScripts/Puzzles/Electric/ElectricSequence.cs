using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElectricSwitch))]
public class ElectricSequence : MonoBehaviour
{
    private ElectricSwitch _eSwitch;

    public bool isActive;

    private void Start()
    {
        _eSwitch = GetComponent<ElectricSwitch>();
    }
    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        _eSwitch.Restart();
    }   
}
