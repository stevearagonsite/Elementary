using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool isActive;
    public bool relateParent;

    protected bool hasHero;

    void OnCollisionEnter(Collision c)
    {
        if (relateParent)
        {
            c.transform.SetParent(transform);
            hasHero = true;
        }
    }

    void OnCollisionStay(Collision c)
    {
        if(relateParent && c.transform.parent != transform)
        {
            c.transform.SetParent(transform);
            hasHero = true;
        }
    }

    void OnCollisionExit(Collision c)
    {
        if (relateParent)
        {
            c.transform.SetParent(null);
            hasHero = false;
        }
    }

}
