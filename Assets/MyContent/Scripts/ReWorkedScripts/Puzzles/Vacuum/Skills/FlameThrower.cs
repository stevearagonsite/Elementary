using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class FlameThrower : ISkill
    {

        IHandEffect _flameVFX;
        List<IFlamableObjects> _flamableObjectsToInteract;

        public FlameThrower(IHandEffect flameVFX, List<IFlamableObjects> flamableObjectsToInteract)
        {
            _flameVFX = flameVFX;
            _flamableObjectsToInteract = flamableObjectsToInteract;

            _flameVFX.StopEffect();
            _flameVFX.TerminateEffect();
    }

        public void Enter()
        {
            //_flameVFX.StartEffect();
        }

        public void Execute()
        {
            if (GameInput.instance.blowUpButton)
            {
                _flameVFX.StartEffect();
                foreach (var fo in _flamableObjectsToInteract)
                {
                    fo.SetOnFire();
                }
                SkillManager.instance.RemoveAmountToSkill(0.2f, Skills.FIRE);
            }
            else
            {
                _flameVFX.StopEffect();
            }
        }

        public void Exit()
        {
            _flameVFX.StopEffect();
            _flameVFX.TerminateEffect();
        }
    }

}
