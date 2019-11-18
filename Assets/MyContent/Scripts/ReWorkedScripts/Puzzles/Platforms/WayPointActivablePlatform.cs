using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointActivablePlatform : Platform {

    public Waypoint activeWaypoint;
    public Waypoint pasiveWaypoint;

    Waypoint targetWaypoint;

    public AnimationCurve motionCurve;

    public float period;
    float _curveTick;

    public PlayAnimation playAnimation;
    bool isPlayerUnder;

    public bool SetActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            if (value)
            {
                targetWaypoint = activeWaypoint;
            }
            else
            {
                targetWaypoint = pasiveWaypoint;
            }
        }
    }

    void Start ()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        SetActive = false;
	}

    void Execute()
    {
        var actualDistance = Mathf.Abs((targetWaypoint.transform.position - transform.position).magnitude);
        var dir = (targetWaypoint.transform.position - transform.position).normalized;
        var speed = motionCurve.Evaluate(_curveTick / period * 2) < 0.01f ? 0.01f: motionCurve.Evaluate(_curveTick / period * 2);

        if (actualDistance < 0.2f)
        {
            speed = 0;
            _curveTick = 0;
            playAnimation.toggleAnim(SetActive);
        }
        else
        {
            _curveTick += Time.deltaTime;
        }
        if(dir.y < 0 && isPlayerUnder)
        {
            dir = Vector3.zero;
            _curveTick -= Time.deltaTime;
        }
        transform.position += dir * speed;
        isPlayerUnder = false;
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            isPlayerUnder = true;
        }
    }

}
