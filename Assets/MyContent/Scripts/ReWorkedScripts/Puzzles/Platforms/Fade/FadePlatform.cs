using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePlatform : Platform {


    public float offset;
    public float period;
    float _tick;
    Animator anim;
    float opacity;

    [Range(0,1)]
    public float activeTime;
    // Use this for initialization

    public Renderer rend;
    public Collider col;
    Material mat;
    public Transform rotator;
    public Vector3 targetRotation;
    Vector3 rotation;


	void Start ()
    {
        anim = GetComponentInChildren<Animator>();
		UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, WarmUp);
        mat = rend.material;
        opacity = 0;
        mat.SetFloat("_Opacity", opacity);
	}

    void WarmUp()
    {
        if (_tick < offset)
        {
            _tick += Time.deltaTime;
        }
        else
        {
            _tick = 0;
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, WarmUp);
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
        
    }

    void Execute()
    {
        if(_tick < period * activeTime)
        {
            opacity += 5 * Time.deltaTime;
            opacity = Mathf.Clamp01(opacity);
            mat.SetFloat("_Opacity", opacity);

            rotation = Vector3.Lerp(rotation, targetRotation, 0.2f);
            rotator.Rotate(rotation * Time.deltaTime);

            anim.SetBool("open", true);

        }
        else if(_tick < period)
        {
            rotation = Vector3.Lerp(rotation, Vector3.zero, 0.2f);
            rotator.Rotate(rotation * Time.deltaTime);
            anim.SetBool("open",false);

            opacity -= 5 * Time.deltaTime;
            opacity = Mathf.Clamp01(opacity);
            mat.SetFloat("_Opacity", opacity);

        }
        else
        {
            _tick = 0;
        }
        _tick += Time.deltaTime;

        if(opacity < 0.2f)
        {
            var heroLC = GetComponentInChildren<LandChecker>();
            if(heroLC != null)
                heroLC.land = false;
            col.isTrigger = true;
        }
        else
        {
            col.isTrigger = false;
        }

    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, WarmUp);
    }
}
