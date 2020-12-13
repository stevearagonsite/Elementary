using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopInitPlayer : MonoBehaviour
{
    private void Start()
    {
        
    }
    void Update()
    {
        var go = GameObject.Find("Character");
        if(go != null)
        {
            go.GetComponent<TPPController>().isActive = true;
            EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_DEMO);
            Destroy(this);
        }
    }
}
