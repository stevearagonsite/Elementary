using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopInitPlayer : MonoBehaviour
{
    void Update()
    {
        var go = GameObject.Find("Character");
        if(go != null)
        {
            go.GetComponent<TPPController>().isActive = true;
        }
    }
}
