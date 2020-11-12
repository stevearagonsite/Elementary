using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Skills;
using System;

public class SkillVFXController : MonoBehaviour
{
    public VisualEffect absorbParticle;
    public VisualEffect blowParticle;
    public VisualEffect fireParticle;

    private Dictionary<Skills.Skills, IHandEffect> _skillDictionary;
    private SkillController _skillController;
    private bool _isStuck;
    void Start()
    {
        _skillController = GetComponent<SkillController>();

        var absorbFX = new VacuumVFX(absorbParticle, blowParticle);
        var fireFX = new FireVFX(fireParticle);


        _skillDictionary = new Dictionary<Skills.Skills, IHandEffect>();

        _skillDictionary.Add(Skills.Skills.VACCUM, absorbFX);
        _skillDictionary.Add(Skills.Skills.FIRE, fireFX);

        InputManager.instance.AddAction(InputType.Absorb, Absorb);
        InputManager.instance.AddAction(InputType.Reject, Reject);
        InputManager.instance.AddAction(InputType.Stop, Stop);
        InputManager.instance.AddAction(InputType.Skill_Up, Stop);
        InputManager.instance.AddAction(InputType.Skill_Down, Stop);

        EventManager.AddEventListener(GameEvent.VACUUM_STUCK, StuckVacuum);
        EventManager.AddEventListener(GameEvent.VACUUM_FREE, FreeVacuum);

    }

    private void FreeVacuum(object[] parameterContainer)
    {
        _isStuck = false;
    }

    private void StuckVacuum(object[] parameterContainer)
    {
        _isStuck = true;
    }

    void Absorb() 
    {
        if(!_isStuck)
            _skillDictionary[_skillController.skillAction].StartEffect();
    }

    void Reject() 
    {
        _skillDictionary[_skillController.skillAction].StartEjectEffect();
    }

    void Stop() 
    {
        _skillDictionary[_skillController.skillAction].StopEffect();
    }

    private void OnDestroy()
    {
        InputManager.instance.RemoveAction(InputType.Absorb, Absorb);
        InputManager.instance.RemoveAction(InputType.Reject, Reject);
        InputManager.instance.RemoveAction(InputType.Stop, Stop);
        InputManager.instance.RemoveAction(InputType.Skill_Up, Stop);
        InputManager.instance.RemoveAction(InputType.Skill_Down, Stop);

        EventManager.RemoveEventListener(GameEvent.VACUUM_STUCK, StuckVacuum);
        EventManager.RemoveEventListener(GameEvent.VACUUM_FREE, FreeVacuum);
    }
}
