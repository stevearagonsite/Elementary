using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool isActive;
    public bool relateParent;
    public LayerMask layerMask;
    protected bool hasHero;

    private void OnTriggerEnter(Collider c)
    {
        if (relateParent && c.gameObject.layer == 9)
        {
            hasHero = true;
            c.transform.SetParent(transform);
            Debug.Log(c.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (relateParent && c.gameObject.layer == 9)
        {
            hasHero = false;
            var pf = c.gameObject.GetComponent<FollowPlatform>();
            if (pf != null)
            {
                c.transform.SetParent(pf.parent);
            }
            Debug.Log(c.gameObject.name);
        }
    }

}
