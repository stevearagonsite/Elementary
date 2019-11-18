using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPCamera
{
    public class CameraFSM : MonoBehaviour
    {
        FSM<Inputs> _fsm;
        public FSM<Inputs> Fsm { get { return _fsm; } }

        //States
        public NormalState normalState { get { return _normalState; } }
        NormalState _normalState;
        FixedState _fixedState;
        StoryState _storyState;

        #region NormalState Variables
        [Header("Normal State Variables")]
        public Transform _lookAt;
        [Range(0.1f,1f)]
        public float positionSmoothness;
        [Range(0f, 5f)]
        public float speed = 1.8f;
        public float unadjustedDistance;
        public LayerMask collisionLayer;
        Camera _cam;
        GameInput _I;
        #endregion

        #region FixedState Variables
        #endregion

        void Awake()
        {
            _cam = GetComponent<Camera>();
            _I = GameInput.instance;
            #region FSM
            _normalState = new NormalState(_lookAt, transform, speed, positionSmoothness, unadjustedDistance, _cam, collisionLayer, _I);
            _fixedState = new FixedState(transform,_lookAt, unadjustedDistance);
            _storyState = new StoryState(_cam);


            var normalTransitions = new Dictionary<Inputs, IState<Inputs>>();
            normalTransitions.Add(Inputs.TO_FIXED, _fixedState);
            normalTransitions.Add(Inputs.TO_DEMO, _storyState);

            var fixedTransitions = new Dictionary<Inputs, IState<Inputs>>();
            fixedTransitions.Add(Inputs.TO_NORMAL, _normalState);
            fixedTransitions.Add(Inputs.TO_DEMO, _storyState);
            fixedTransitions.Add(Inputs.TO_FIXED, _fixedState);

            var storyTransitions = new Dictionary<Inputs, IState<Inputs>>();
            storyTransitions.Add(Inputs.TO_NORMAL, _normalState);
            storyTransitions.Add(Inputs.TO_FIXED, _fixedState);

            _normalState.Transitions = normalTransitions;
            _fixedState.Transitions = fixedTransitions;
            _storyState.Transitions = storyTransitions;

            _fsm = new FSM<Inputs>(_normalState);
            #endregion

            EventManager.AddEventListener(GameEvent.CAMERA_FIXPOS, ToFixed);
            EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, ToNormal);
            EventManager.AddEventListener(GameEvent.CAMERA_STORY, ToStory);
        }
        // Use this for initialization
        void Start ()
        {
            UpdatesManager.instance.AddUpdate(UpdateType.LATE, Execute);
	    }


        // Update is called once per frame
        void Execute () {
            _fsm.Execute();
            CheckInputs();
	    }

        void CheckInputs()
        {
            
        }

        private void ToStory(object[] parameterContainer)
        {
            //_storyState.update = (Action<Transform>)parameterContainer[0];
            var camTag = (string)parameterContainer[0];
            //CutScenesManager.instance.ActivateCutSceneCamera(camTag);
            _storyState.cutSceneCamera = CutScenesManager.instance.GetCamera(camTag);
            _fsm.ProcessInput(Inputs.TO_DEMO);
        }

        void ToFixed(object[] parameterContainer)
        {
            _fixedState.targetX = (float)parameterContainer[0];
            _fixedState.targetY = (float)parameterContainer[1];
            _fixedState.targetDistance = (float)parameterContainer[2];
            _fsm.ProcessInput(Inputs.TO_FIXED);
        }

        void ToNormal(object[] parameterContainer)
        {
            _fsm.ProcessInput(Inputs.TO_NORMAL);
        }

        private void OnDestroy()
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
            EventManager.RemoveEventListener(GameEvent.CAMERA_FIXPOS, ToFixed);
            EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, ToNormal);
            EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, ToStory);
        }
    }

}
