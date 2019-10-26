using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelProximity : MonoBehaviour {

    public Animator switchAnimations;
    public Canvas can;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            switchAnimations.SetBool("On", true);
            switchAnimations.SetBool("Off", false);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            switchAnimations.SetBool("Off", true);
            switchAnimations.SetBool("On", false);
        }
    }
}
