using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatesManager : MonoBehaviour {

    public delegate void _Updates();

    private static Dictionary<UpdateType, _Updates> _updates;

    private static UpdatesManager _instance;
    public static UpdatesManager instance { get{ return _instance; } }

    bool isActive;

    // Use this for initialization
    void Awake () {
        if (_instance == null) _instance = this;
        StartCoroutine(CoroutineUpdate());
        isActive = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        _updates = new Dictionary<UpdateType, _Updates>();
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            if(_updates == null)
            {
                return;
            }
            if (_updates.ContainsKey(UpdateType.UPDATE) && _updates[UpdateType.UPDATE] != null)  _updates[UpdateType.UPDATE]();

        }
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            if (_updates == null)
            {
                return;
            }
            if(_updates.ContainsKey(UpdateType.FIXED) && _updates[UpdateType.FIXED] != null) _updates[UpdateType.FIXED]();

        }
    }

    void LateUpdate()
    {
        if (isActive)
        {
            if (_updates == null)
            {
                return;
            }
            if (_updates.ContainsKey(UpdateType.LATE) && _updates[UpdateType.LATE] != null)  _updates[UpdateType.LATE]();

        }
    }

    private IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            if (_updates == null)
                yield return null;
            if (_updates.ContainsKey(UpdateType.COROUTINES))
                _updates[UpdateType.COROUTINES]();

            yield return null;
        }
    }

    public void AddUpdate(UpdateType uT, _Updates listener)
    {
        if(_updates == null)
        {
            _updates = new Dictionary<UpdateType, _Updates>();
        }
        if (!_updates.ContainsKey(uT))
        {
            _updates.Add(uT, null);
        }
        _updates[uT] += listener;
    }

    public void RemoveUpdate(UpdateType uT, _Updates listener)
    {
        if(_updates != null)
        {
            if(_updates.ContainsKey(uT))
            {
                _updates[uT] -= listener;
            }
        }
    }

    public void StopUpdates()
    {
        isActive = false;
    }
    
}
public enum UpdateType
{
    UPDATE,
    FIXED,
    LATE,
    COROUTINES
}
