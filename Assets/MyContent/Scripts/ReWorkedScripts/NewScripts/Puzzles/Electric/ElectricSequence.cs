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
        Debug.Log("Activate Sequence: " + gameObject.name);
    }

    public void Deactivate()
    {
        isActive = false;
        _eSwitch.Restart();
        Debug.Log("Deactivate Sequence: " + gameObject.name);
    }   
}
