using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TPCamera
{
    public class StoryState : IState<Inputs>
    {
        Dictionary<Inputs, IState<Inputs>> _transitions;

        Camera _cam;
        string _cutSceneTag;
        Camera _cutSceneCamera;
        public Camera cutSceneCamera
        {
            set
            {
                _cutSceneCamera = value;
                _cutSceneTag = _cutSceneCamera.GetComponent<CutSceneCamera>().tag;
            }
        }

        public StoryState(Camera cam)
        {
            _cam = cam;
        }


        public void Enter()
        {
            _cam.enabled = false;
        }

        public void Execute()
        {
            
        }

        public void Exit()
        {
            _cam.transform.position = _cutSceneCamera.transform.position;
            _cam.transform.rotation = _cutSceneCamera.transform.rotation;
            _cam.enabled = true;

        }

        public Dictionary<Inputs, IState<Inputs>> Transitions
        {
            get { return _transitions; }
            set { _transitions = value; }
        }
    }

}
