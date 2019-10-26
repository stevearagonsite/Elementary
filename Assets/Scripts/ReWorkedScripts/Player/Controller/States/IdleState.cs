using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using TPCamera;

namespace Player
{
    public class IdleState : IState<Inputs>
    {
        Dictionary<Inputs, IState<Inputs>> _transitions;

        PlayerController _pC;
        Animator _anim;
        Transform _mainCamera;
        Transform transform;
        CameraFSM _cam;
        SkillController _skill;

        public IdleState(PlayerController pC, Animator anim, Transform mainCamera, Transform t, CameraFSM cam)
        {
            _pC = pC;
            _anim = anim;
            _mainCamera = mainCamera;
            transform = t;
            _cam = cam;
            _skill = _pC.GetComponentInChildren<SkillController>();
        }

        public void Enter()
        {
            _anim.SetBool("toJump", false);
            _anim.SetBool("toLand", true);
            //_cam.ChangeSmoothness(0.3f);
            _cam.normalState.positionSmoothness = 0.3f;
            _anim.SetTrigger("toIdle");
            
        }

        public void Execute()
        {
            
            if (((GameInput.instance.absorbButton && _skill.currentSkill == Skills.Skills.VACCUM )|| GameInput.instance.blowUpButton )&& !_pC.fixedCamera)
            {
                var camYRotation = Quaternion.Euler(0, _mainCamera.eulerAngles.y, 0);
                transform.rotation = camYRotation;
            }
        }

        public void Exit()
        {

        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }
    }

}
