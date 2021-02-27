using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumSwitchGemHolderHandler : MonoBehaviour
{
    private VacuumSwitch _switch;
    public float rotationSpeed;
    private float actualSpeed;
    void Start()
    {
        _switch = GetComponentInParent<VacuumSwitch>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Rotate);
    }

    public void OnChangeCharge()
    {
        actualSpeed = _switch.currentAmountOfAir / _switch.maxAmountOfAir * rotationSpeed * Time.deltaTime;
    }
    public void Rotate()
    {
        transform.Rotate(0,0,actualSpeed);
    }
    public void OnActivation()
    {
        actualSpeed = rotationSpeed;
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Rotate);
    }
}
