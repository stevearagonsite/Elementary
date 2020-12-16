using System;
using System.Collections.Generic;
using UnityEngine;
using Skills;

[RequireComponent(typeof(Collider))]
public class ActivateSkill : MonoBehaviour
{
    //In case we want to deactivate powers ahead
    private bool deactivate;
    public bool oneTime;

    public Skills.Skills skillToActivate;
    private Dictionary<Skills.Skills, GameEvent> _skillActivateEvent;
    private Dictionary<Skills.Skills, GameEvent> _skillDeactivateEvent;

    void Start()
    {
        _skillActivateEvent = new Dictionary<Skills.Skills, GameEvent>();
        _skillDeactivateEvent = new Dictionary<Skills.Skills, GameEvent>();

        _skillActivateEvent.Add(Skills.Skills.VACCUM, GameEvent.SKILL_ACTIVATE_VACUUM);
        _skillActivateEvent.Add(Skills.Skills.FIRE, GameEvent.SKILL_ACTIVATE_FIRE);
        _skillActivateEvent.Add(Skills.Skills.ELECTRICITY, GameEvent.SKILL_ACTIVATE_ELECTRIC);

        _skillDeactivateEvent.Add(Skills.Skills.VACCUM, GameEvent.SKILL_DEACTIVATE);
        _skillDeactivateEvent.Add(Skills.Skills.FIRE, GameEvent.SKILL_DEACTIVATE_FIRE);
        _skillDeactivateEvent.Add(Skills.Skills.ELECTRICITY, GameEvent.SKILL_DEACTIVATE_ELECTRIC);
    }

    private void OnTriggerEnter(Collider other)
    {
        var gameEvent = deactivate ? _skillDeactivateEvent[skillToActivate] : _skillActivateEvent[skillToActivate];
        EventManager.DispatchEvent(gameEvent);
        if (oneTime)
            Destroy(this);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        if(deactivate)
            Gizmos.color = new Color(200, 0, 200, 0.7f); 
        else
            Gizmos.color = new Color(0, 200, 200, 0.7f);

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
