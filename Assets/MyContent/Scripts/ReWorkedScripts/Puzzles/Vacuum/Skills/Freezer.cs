using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class Freezer : ISkill
    {
        IHandEffect _freezeVFX;
        List<IFrozenObject> _frozenObjectsToInteract;

        public Freezer(IHandEffect freezeVFX, List<IFrozenObject> frozenObjectsToInteract)
        {
            _freezeVFX = freezeVFX;
            _frozenObjectsToInteract = frozenObjectsToInteract;

            _freezeVFX.StopEffect();
            _freezeVFX.TerminateEffect();
        }

        public void Enter()
        {

        }

        public void Execute()
        {
            if (GameInput.instance.blowUpButton)
            {
                _freezeVFX.StartEffect();
                foreach (var fo in _frozenObjectsToInteract)
                {
                    fo.Freeze();
                }
                SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.ICE);
            }
            else
            {
                _freezeVFX.StopEffect();
            }
        }

        public void Exit()
        {
            _freezeVFX.StopEffect();
            _freezeVFX.TerminateEffect();
        }


    }

}
