using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class Freezer : ISkill
    {
        List<IFrozenObject> _frozenObjectsToInteract;

        public Freezer(List<IFrozenObject> frozenObjectsToInteract)
        {
            _frozenObjectsToInteract = frozenObjectsToInteract;
        }

        public void Absorb()
        {
        }

        public void Eject()
        {
            foreach (var fo in _frozenObjectsToInteract)
            {
                fo.Freeze();
            }
            //SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.ICE);
        }

        public void Enter()
        {

        }

        public void Exit()
        {

        }


    }

}
