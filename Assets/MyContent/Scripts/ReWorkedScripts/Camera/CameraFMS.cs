using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraFMS : MonoBehaviour {

    #region FMS
    public enum States {Normal, Zoom, Fixed}
    public enum Inputs {ToNormal, ToZoom, ToFixed}

    private Dictionary<States, Action> _updates;
    private Dictionary<States, Action> _enters;

    private Dictionary<States, Dictionary<Inputs, States>> _transitions;

    public States currentState;
    #endregion

    //Public Inputs
    public bool aimToogle;

    private CameraController _cc;
    
    void Awake ()
    {
        _enters = new Dictionary<States, Action>();
        _enters.Add(States.Normal, NormalEnter);
        _enters.Add(States.Zoom, ZoomEnter);
        _enters.Add(States.Fixed, FixedEnter);

        _updates = new Dictionary<States, Action>();
        _updates.Add(States.Normal, Normal);
        _updates.Add(States.Zoom, Zoom);
        _updates.Add(States.Fixed, Fixed);

        var normalTransitions = new Dictionary<Inputs, States>();
        normalTransitions.Add(Inputs.ToFixed, States.Fixed);
        normalTransitions.Add(Inputs.ToZoom, States.Zoom);

        var fixedTransitions = new Dictionary<Inputs, States>();
        fixedTransitions.Add(Inputs.ToNormal, States.Normal);

        var zoomTransitions = new Dictionary<Inputs, States>();
        zoomTransitions.Add(Inputs.ToNormal, States.Normal);
        zoomTransitions.Add(Inputs.ToFixed, States.Fixed);


        _transitions = new Dictionary<States, Dictionary<Inputs, States>>();
        _transitions.Add(States.Normal, normalTransitions);
        _transitions.Add(States.Fixed, fixedTransitions);
        _transitions.Add(States.Zoom, zoomTransitions);

        _cc = GetComponent<CameraController>();

        
	}

    void Start()
    {
        //Was Fixed
        UpdatesManager.instance.AddUpdate(UpdateType.LATE, LateExecute);
    }

    void LateExecute() {
        CheckInputs();
        _updates[currentState]();
	}

    /// <summary>
    /// Check for new Inputs to translate to the FSM
    /// </summary>
    private void CheckInputs()
    {
        if(aimToogle && currentState != States.Zoom)
        {
            ProcessInput(Inputs.ToZoom);
            aimToogle = false;
        }
        else if(aimToogle && currentState != States.Normal)
        {
            ProcessInput(Inputs.ToNormal);
            aimToogle = false;
        }
    }

    /// <summary>
    /// Procces the input given
    /// </summary>
    /// <param name="input"> Inputs enum: for FSM transitions </param>
    void ProcessInput(Inputs input)
    {
        var currentStateTransitions = _transitions[currentState];

        if (currentStateTransitions.ContainsKey(input) && currentState != currentStateTransitions[input])
        {
            currentState = currentStateTransitions[input];
            _enters[currentState]();
        }
    }

    #region enters
    void NormalEnter()
    {
        _cc.NormalEnter();
    }

    void ZoomEnter()
    {
        _cc.ZoomEnter();
    }

    void FixedEnter()
    {

    }
    #endregion

    #region updates
    void Normal()
    {
        _cc.NormalUpdate();
    }

    void Zoom()
    {
        _cc.ZoomUpdate();
    }

    void Fixed()
    {

    }
    #endregion

    #region FixCameraMerhods
    public void FixCamera()
    {
        ProcessInput(Inputs.ToFixed);
    }

    public void UnFixCamera()
    {
        ProcessInput(Inputs.ToNormal);
    }
    #endregion

    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, LateExecute);
    }
}
