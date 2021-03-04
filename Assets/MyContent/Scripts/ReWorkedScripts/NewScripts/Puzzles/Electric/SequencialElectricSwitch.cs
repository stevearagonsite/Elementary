using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequencialElectricSwitch : MonoBehaviour
{
    [Header("Put this in order!")]
    public ElectricSequence[] electricSequence;
    public UnityEvent onSequencePass;

    private int _activatedSwitches = 0;

    public void CheckSequence()
    {
        _activatedSwitches++;
        bool pass = true;
        for (int i = 0; i < _activatedSwitches; i++)
        {
            if (!electricSequence[i].isActive)
            {
                pass = false;
                break;
            }
        }
        if (pass && _activatedSwitches == electricSequence.Length)
        {
            onSequencePass.Invoke();
        }
        else if (!pass)
        {
            for (int i = 0; i < electricSequence.Length; i++)
            {
                electricSequence[i].Deactivate();
                _activatedSwitches = 0;
            }
        }
    }

    
}
