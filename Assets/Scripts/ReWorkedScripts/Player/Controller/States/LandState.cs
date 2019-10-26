using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;

namespace Player
{
    public class LandState : IState<Inputs>
    {

        Dictionary<Inputs, IState<Inputs>> _transitions;
        Animator _anim;
        PlayerController _pC;
        AnimatorEventsBehaviour _aEB;
        CameraFSM _cam;

        public LandState(Animator anim, PlayerController pC, AnimatorEventsBehaviour aEB, CameraFSM cam)
        {
            _anim = anim;
            _pC = pC;
            _aEB = aEB;
            _cam = cam;
        }

        public void Enter()
        {
            
            _anim.SetBool("toLand", true);
            _cam.normalState.unadjustedDistance = 2f;

        }

        public void Execute()
        {

            _anim.SetBool("toLand", true);
            
        }

        public void Exit()
        {

            _anim.SetBool("toLand", false);
            
            _aEB.LandEnd();

            _pC.isSkillLocked = false;
        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }

    }
}
