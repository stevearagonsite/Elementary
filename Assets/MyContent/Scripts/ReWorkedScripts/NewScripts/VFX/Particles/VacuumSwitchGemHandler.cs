using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumSwitchGemHandler : MonoBehaviour
{
    private Renderer _ren;
    private VacuumSwitch _switch;

    void Start()
    {
        _ren = GetComponent<Renderer>();
        _switch = GetComponentInParent<VacuumSwitch>();
    }

    public void OnChangeCharge()
    {
        _ren.material.SetFloat("_emissive", _switch.currentAmountOfAir/_switch.maxAmountOfAir * 5);

    }
}
