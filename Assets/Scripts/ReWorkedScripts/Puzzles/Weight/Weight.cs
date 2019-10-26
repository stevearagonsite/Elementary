using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Weight : MonoBehaviour {

    List<ObjectToWeight> _total;
    /// <summary>
    /// Executes when weight is reached
    /// </summary>
    public UnityEvent onWeight;

    /// <summary>
    /// Executes when new object enters the weight
    /// </summary>
    public UnityEvent onWeightEnter;

    /// <summary>
    /// Executes when an object leaves the weight
    /// </summary>
    public UnityEvent onWeightExit;


    public float actionWeight;
    float _totalWeight;

    private bool wasOnWeight;

    public float totalWeight{ set { _totalWeight = value; } }

    void Awake()
    {
        if (onWeight == null)
            onWeight = new UnityEvent();
        if (onWeightEnter == null)
            onWeightEnter = new UnityEvent();
        if (onWeightExit == null)
            onWeightExit = new UnityEvent();
    }

    private void Start()
    {
        _total = new List<ObjectToWeight>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        wasOnWeight = false;
    }

    void Execute()
    {
        _totalWeight = 0;
        foreach (var otw in _total)
        {
            _totalWeight += otw.mass;
        }
        if(_totalWeight >= actionWeight && onWeight != null)
        {
            onWeight.Invoke();
        }
	}

    public void AddToWeight(ObjectToWeight otw)
    {
        if (!_total.Contains(otw))
        {
            _total.Add(otw);
        }

        //EnterCallbacks
        float total = 0;
        foreach (var o in _total)
        {
            total += o.mass;
        }
        if(total >= actionWeight && onWeightEnter != null && !wasOnWeight)
        {
            onWeightEnter.Invoke();
            wasOnWeight = true;
        }

    }

    public void RemoveFromWeight(ObjectToWeight otw)
    {
        _total.Remove(otw);
        

        //ExitCallbacks
        float total = 0;
        foreach (var o in _total)
        {
            total += o.mass;
        }
        if (total <= actionWeight && onWeightExit != null && wasOnWeight)
        {
            DeactivateWeight();
        }
    }

    public void RemoveAllObjectsToWeight()
    {
        for (int i = _total.Count - 1; i >= 0; i--)
        {
            RemoveFromWeight(_total[i]);
        }
    }

    public void DeactivateWeight()
    {
        onWeightExit.Invoke();
        wasOnWeight = false;
    }



    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
