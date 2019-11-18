using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumSwitch : MonoBehaviour, IVacuumObject
{
    bool isActive;

    #region VacuumObject Implementation
    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    public bool isAbsorved {
        get { return _isAbsorved; }
        set { _isAbsorved = value; }
    }

    public bool isAbsorvable { get { return _isAbsorvable; } }

    public bool isBeeingAbsorved {
        get {return _isBeeingAbsorved; }
        set { _isBeeingAbsorved = value; }
    }

    public Rigidbody rb {
        get { return _rb; }
        set { _rb = value; }
    }


    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        if (isActive)
        {
            if (currentAmountOfAir < maxAmountOfAir)
                currentAmountOfAir += 60*Time.deltaTime;
            else
            {
                currentAmountOfAir = maxAmountOfAir;
                if(callbacks != null)
                {
                    callbacks();
                    isActive = false;
                    UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
                }
            }
            if(increaseCallbacks != null)
                increaseCallbacks();
        }
    }

    public void SuckIn(Transform origin, float atractForce)
    {
        if (isActive)
        {
            if(currentAmountOfAir > 0)
                currentAmountOfAir -= 1;
            if(decreaseCallbacks != null)
                decreaseCallbacks();
        }
    }

    //Unused Interface Methods
    public void Exit(){}
    public void ReachedVacuum(){}
    public void Shoot(float shootForce, Vector3 direction){}
    public void ViewFX(bool active){}

    #endregion

    #region Delegate Implementation
    public delegate void OnSwitch();
    public delegate void OnSwitchIncrease();
    public delegate void OnSwitchDecrease();

    OnSwitch callbacks;
    OnSwitchIncrease increaseCallbacks;
    OnSwitchDecrease decreaseCallbacks;


    public void AddOnSwitchEvent(OnSwitch callback)
    {
        callbacks += callback;
    }

    public void RemoveOnSwitchEvent(OnSwitch callback)
    {
        callbacks -= callback;
    }

    public void AddOnSwitchIncreaseEvent(OnSwitchIncrease callback)
    {
        increaseCallbacks += callback;
    }

    public void RemoveOnSwitchIncreaseEvent(OnSwitchIncrease callback)
    {
        increaseCallbacks -= callback;
    }

    public void AddOnSwitchDecreaseEvent(OnSwitchDecrease callback)
    {
        decreaseCallbacks += callback;
    }

    public void RemoveOnSwitchDecreaseEvent(OnSwitchDecrease callback)
    {
        decreaseCallbacks -= callback;
    }


    #endregion

    #region VacuumSwitch Implementation

    public float maxAmountOfAir;
    public float currentAmountOfAir;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currentAmountOfAir = 0;
        isActive = true;
        _isAbsorvable = false;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, RemainKinematic);
    }

    void Execute()
    {
        if(decreaseCallbacks != null)
        {
            decreaseCallbacks();
        }
        if(currentAmountOfAir > 0)
            currentAmountOfAir -= 30* Time.deltaTime;
        
    }

    //TODO: Select if you want kinematic to return to false when you stop blowing up IVacuumObjects
    void RemainKinematic()
    {
        _rb.isKinematic = true;
    }

    #endregion

    public float GetCurrentProgressPercent()
    {
        return isActive ? currentAmountOfAir / maxAmountOfAir: 1;
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, RemainKinematic);
    }

}
