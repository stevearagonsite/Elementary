using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;

public class WaterLauncher : ISkill {


    List<IWaterObject> _waterObjectsToInteract;

    public WaterLauncher(List<IWaterObject> waterObjects)
    {
        _waterObjectsToInteract = waterObjects;
    }

    public void Absorb()
    {
    }

    public void Eject()
    {
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
        
    }
}
