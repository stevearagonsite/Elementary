using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class Electricity : ISkill
    {
        IHandEffect _electricityVFX;
        List<Transform> _electricObjectsToInteract;

        public Electricity(IHandEffect electricityVFX, List<Transform> electricObjectsToInteract)
        {
            _electricityVFX = electricityVFX;
            _electricObjectsToInteract = electricObjectsToInteract;
        }

        public void Enter()
        {
            
        }

        public void Execute()
        {
            if (GameInput.instance.blowUpButton)
            {
                _electricityVFX.StartEffect();
                foreach (var eo in _electricObjectsToInteract)
                {
                    eo.GetComponent<IElectricObject>().Electrify();
                    SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.ELECTRICITY);
                }
            }
            else
            {
                _electricityVFX.StopEffect();
            }
        }

        public void Exit()
        {
            _electricityVFX.StopEffect();
            _electricityVFX.TerminateEffect();
        }


    }

}
