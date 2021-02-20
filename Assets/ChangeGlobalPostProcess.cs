using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeGlobalPostProcess : MonoBehaviour
{
    public Volume inVolume;
    public Volume outVolume;
    public float transitionTime;
    private void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(EnterTriggerPostProcess(transitionTime));
    }

    private IEnumerator EnterTriggerPostProcess(float transitionTime)
    {
        var tick = 0f;
        while (tick < transitionTime)
        {
            tick += Time.deltaTime;
            inVolume.weight = Mathf.Clamp(tick, 0.05f, 1);
            outVolume.weight = Mathf.Clamp(1 - tick, 0.05f, 1);
            yield return null;
        }
        inVolume.weight = 1;
        outVolume.weight = 0.05f;
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(ExitTriggerPostProcess(transitionTime));
    }

    private IEnumerator ExitTriggerPostProcess(float transitionTime)
    {
        var tick = 0f;
        while (tick < transitionTime)
        {
            tick += Time.deltaTime;
            outVolume.weight = Mathf.Clamp(tick, 0.05f, 1);
            inVolume.weight = Mathf.Clamp(1 - tick, 0.05f, 1);
            yield return null;
        }
        outVolume.weight = 1;
        inVolume.weight = 0.05f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
