using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandChecker : MonoBehaviour {

    public bool land;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 10)
        {
            land = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer != 10)
        {
            land = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            land = false;
        }
        
    }
}
