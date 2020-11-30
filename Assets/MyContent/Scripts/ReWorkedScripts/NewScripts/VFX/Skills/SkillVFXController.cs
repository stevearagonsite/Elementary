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

    [HideInInspector]
    public ElectricityManager electricFX;

    private Dictionary<Skills.Skills, IHandEffect> _skillDictionary;
    private SkillController _skillController;
    private bool _isStuck;

    private CharacterController _cc;
    void Start()
    {
        _skillController = GetComponent<SkillController>();
        _cc = GetComponent<CharacterController>();

        var absorbFX = new VacuumVFX(absorbParticle, blowParticle);
        var fireFX = new FireVFX(fireParticle);
        electricFX = GetComponentInChildren<ElectricityManager>();


        _skillDictionary = new Dictionary<Skills.Skills, IHandEffect>();

        _skillDictionary.Add(Skills.Skills.VACCUM, absorbFX);
        _skillDictionary.Add(Skills.Skills.FIRE, fireFX);
        _skillDictionary.Add(Skills.Skills.ELECTRICITY, electricFX);

        InputManager.instance.AddAction(InputType.Absorb, Absorb);
        InputManager.instance.AddAction(InputType.Reject, Reject);
        InputManager.instance.AddAction(InputType.Stop, Stop);
        InputManager.instance.AddAction(InputType.Skill_Up, Stop);
        InputManager.instance.AddAction(InputType.Skill_Down, Stop);

        EventManager.AddEventListener(GameEvent.VACUUM_STUCK, StuckVacuum);
        EventManager.AddEventListener(GameEvent.VACUUM_FREE, FreeVacuum);
        EventManager.AddEventListener(GameEvent.ON_SKILL_CHANGE, onSkillChange);

    }

    private void onSkillChange(object[] parameterContainer)
    {
        foreach(var sk in _skillDictionary)
        {
            sk.Value.StopEffect();
        }
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
        if(!_isStuck && _cc.isGrounded)
            _skillDictionary[_skillController.skillAction].StartEffect();
        else
            _skillDictionary[_skillController.skillAction].StopEffect();
    }

    void Reject()
    {
        if (_cc.isGrounded)
            _skillDictionary[_skillController.skillAction].StartEjectEffect();
        else
            _skillDictionary[_skillController.skillAction].StopEffect();
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
        EventManager.RemoveEventListener(GameEvent.ON_SKILL_CHANGE, onSkillChange);

    }
}
