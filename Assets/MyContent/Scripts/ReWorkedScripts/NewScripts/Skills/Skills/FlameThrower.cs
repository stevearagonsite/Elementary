using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class FlameThrower : ISkill
    {
        List<IFlamableObjects> _flamableObjectsToInteract;

        public FlameThrower(List<IFlamableObjects> flamableObjectsToInteract)
        {
            _flamableObjectsToInteract = flamableObjectsToInteract;
        }

        public void Enter()
        {
        }

        public void Absorb()
        {
            Eject();
        }

        public void Eject()
        {

            foreach (var fo in _flamableObjectsToInteract)
            {
                fo.SetOnFire();
            }
            //SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.FIRE);
        }

        public void Exit()
        {

        }
    }

}
