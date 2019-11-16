using System;
using System.Collections.Generic;
using System.Linq;
using Mewlist;
using UnityEngine;

public class MassiveCloudsScriptableScrollSample : MonoBehaviour
{

    [SerializeField] private MassiveClouds massiveClouds;

    [SerializeField] private float velocity = 500f;
    [SerializeField] private Vector3 direction = Vector3.forward;
    [Range(0f, 1f)]
    [SerializeField] private List<float> densities = new List<float>();

    private Vector3 currentOffset = Vector3.zero;
    private List<MassiveCloudsParameter> parameters;
    private bool initialized = false;

    void OnEnable()
    {
        parameters = massiveClouds.Parameters;
        densities = parameters.Select(x => x.Density).ToList();
        initialized = true;
    }
	
    void Update ()
    {
        var v = 1000f * velocity / 60f / 60f;
        currentOffset += Time.deltaTime * v * -direction.normalized;
        massiveClouds.SetOffset(currentOffset);
    }

    private void OnValidate()
    {
        if (!initialized) return;
        for (int i = 0; i < parameters.Count(); i++)
        {
            var parameter = parameters[i];
            parameter.Density = densities[i];
            parameter.ScrollVelocity = Vector3.zero;
            parameters[i] = parameter;
        }
        massiveClouds.SetParameters(parameters);
    }
}