using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;

public class WaterLauncher : ISkill {

    IHandEffect _waterVFX;
    List<IWaterObject> _waterObjectsToInteract;

    public WaterLauncher(IHandEffect water, List<IWaterObject> waterObjects)
    {
        _waterVFX = water;
        _waterObjectsToInteract = waterObjects;

        _waterVFX.StopEffect();
        _waterVFX.TerminateEffect();
    }

    public void Absorb()
    {
    }

    public void Eject()
    {
        _waterVFX.StartEffect();
        foreach (var obj in _waterObjectsToInteract)
        {
            obj.WetThis();
        }
        SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.Skills.WATER);
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        _waterVFX.StopEffect();
    }
}
