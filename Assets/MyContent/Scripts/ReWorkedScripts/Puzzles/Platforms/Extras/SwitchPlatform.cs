using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Platform))]
public class SwitchPlatform : MonoBehaviour {

    public VacuumSwitch vacuumSwitch;

    Platform[] platforms;
	// Use this for initialization
	void Start ()
    {
        platforms = GetComponents<Platform>();
        vacuumSwitch.AddOnSwitchEvent(SwitchOn);
        for (int i = 0; i < platforms.Length; i++)
        {

            platforms[i].isActive = false;
        }
	}
	
	// Update is called once per frame
	void Execute ()
    {
		
	}

    void SwitchOn()
    {
        for (int i = 0; i < platforms.Length; i++)
        {

            platforms[i].isActive = true;
        }
    }

    void OnDestroy()
    {
        vacuumSwitch.RemoveOnSwitchEvent(SwitchOn);
    }
}
