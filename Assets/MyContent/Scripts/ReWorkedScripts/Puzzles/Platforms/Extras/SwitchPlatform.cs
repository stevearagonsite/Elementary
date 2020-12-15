using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Platform))]
public class SwitchPlatform : MonoBehaviour 
{
    Platform[] platforms;
	// Use this for initialization
	void Start ()
    {
        platforms = GetComponents<Platform>();
        for (int i = 0; i < platforms.Length; i++)
        {

            platforms[i].isActive = false;
        }
	}

    public void SwitchOn()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].isActive = true;
        }
    }
}
