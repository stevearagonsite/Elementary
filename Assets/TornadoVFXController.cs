using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoVFXController : MonoBehaviour
{
    public float maxAlpha;
    private Renderer _rend;
    // Start is called before the first frame update
    void Start()
    {
        _rend = GetComponent<Renderer>();
    }

    public void StartEffect()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, FadeIn);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, FadeOut);
    }

    private void FadeIn()
    {
        var actualAlpha = _rend.material.GetFloat("_alpha");
        if(actualAlpha < maxAlpha)
        {
            actualAlpha += Time.deltaTime;
            _rend.material.SetFloat("_alpha", actualAlpha);
        }
        else
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, FadeIn);
        }
    }

    public void StopEffect()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, FadeOut);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, FadeIn);
    }

    private void FadeOut()
    {
        var actualAlpha = _rend.material.GetFloat("_alpha");
        if (actualAlpha > 0)
        {
            actualAlpha -= Time.deltaTime;
            _rend.material.SetFloat("_alpha", actualAlpha);
        }
        else
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, FadeOut);
        }
    }
}
