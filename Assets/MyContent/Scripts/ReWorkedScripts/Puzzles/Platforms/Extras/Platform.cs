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
            c.GetComponent<FollowPlatform>().SetPlatformToFollow(transform);
            hasHero = true;
            Debug.Log(c.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (relateParent && c.gameObject.layer == 9)
        {
            hasHero = false;
            c.GetComponent<FollowPlatform>().ReleasePlatform();
            Debug.Log(c.gameObject.name);
        }
    }

}
