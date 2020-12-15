using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VacuumSwitchVisuals : MonoBehaviour {

    public ParticleSystem onIncrease;
    public ParticleSystem[] onActive;

    private void Start()
    {
        onIncrease.Stop();
        for (int i = 0; i < onActive.Length; i++)
        {
            onActive[i].Stop();
        }
    }

    public void IncreasingVisuals()
    {
        onIncrease.Play();
    }

    public void Decrease()
    {
        onIncrease.Stop();
    }

    public void ActivateVisuals()
    {
        for (int i = 0; i < onActive.Length; i++)
        {
            onActive[i].Play();
        }
        onIncrease.Stop();
    }
}
