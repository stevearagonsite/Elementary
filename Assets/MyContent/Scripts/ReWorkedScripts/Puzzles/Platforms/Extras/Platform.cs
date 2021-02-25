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
        Debug.Log(c.gameObject.name);
        if (relateParent && c.gameObject.GetComponent<FollowPlatform>() != null)
        {
            c.gameObject.GetComponent<FollowPlatform>().SetPlatformToFollow(transform);
            hasHero = true;
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (relateParent && c.gameObject.GetComponent<FollowPlatform>() != null)
        {
            hasHero = false;
            c.gameObject.GetComponent<FollowPlatform>().ReleasePlatform();
            Debug.Log(c.gameObject.name);
        }
    }

}
