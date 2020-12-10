using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class Electricity : ISkill
    {
        List<Transform> _electricObjectsToInteract;
        ElectricityManager _electricFX;
        public Electricity(List<Transform> electricObjectsToInteract, ElectricityManager electricFX)
        {
            _electricObjectsToInteract = electricObjectsToInteract;
            _electricFX = electricFX;
        }

        public void Enter()
        {
            
        }

        public void Absorb()
        {

        }

        public void Eject()
        {
            if(_electricObjectsToInteract.Count > 0)
            {
                foreach (var eo in _electricObjectsToInteract)
                {
                    eo.GetComponent<IElectricObject>().Electrify();
                   // SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.ELECTRICITY);
                }
                

            }
            _electricFX.SetTargets(_electricObjectsToInteract);
        }

        public void Exit()
        {
        }


    }

}
