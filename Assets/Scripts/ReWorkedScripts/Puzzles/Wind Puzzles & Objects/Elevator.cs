using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elevator : MonoBehaviour {

    public VacuumSwitch elevatorSwitch;
    public bool isActive;
    public ParticleSystem activeParticles;

    public string cutSceneTag;

	void Start ()
    {
        elevatorSwitch.AddOnSwitchEvent(SwitchOn);
        isActive = false;
        activeParticles.Stop();
    }

    void SwitchOn()
    {
        isActive = true;
        elevatorSwitch.RemoveOnSwitchEvent(SwitchOn);
        activeParticles.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActive && other.gameObject.layer == 9)
        {
            EventManager.DispatchEvent(GameEvent.CAMERA_STORY, cutSceneTag);
        }
    }

    
}
