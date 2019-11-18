using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class FadeAwayPlatform : Platform {

    Renderer _renderer;
    BoxCollider _collider;

    [Header("Smooth Fade Away Transition")]
    public AnimationCurve curve;

    [Header("Period")]
    public float period;
    public float offset;

    float _offsetTick;
    float _tick;

	// Use this for initialization
	void Start ()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<BoxCollider>();
        _tick = 0;
        _offsetTick = 0;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	// Update is called once per frame
	void Execute ()
    {
        if (isActive)
        {
            if(_offsetTick > offset)
            {
                _tick += Time.deltaTime;
                if(_tick > period)
                {
                    _tick = 0;
                }

                var value = curve.Evaluate(_tick / period);
                var alpha = value < 0 ? 0 : value; 
                var col = _renderer.material.GetColor("_color");
                _renderer.material.SetColor("_color", new Color(col.r, col.g, col.b, alpha));

                if(alpha <= 0.2f)
                {
                    _collider.isTrigger = true;
                    gameObject.layer = 10;
                    if (hasHero)
                    {
                        var lc = GetComponentInChildren<LandChecker>();
                        if (lc != null) lc.land = false;
                    }
                }
                else
                {
                    _collider.isTrigger = false;
                    gameObject.layer = 8;
                    if(transform.childCount< 0)
                    {
                        var heroLC = GetComponentInChildren<LandChecker>();
                        heroLC.land = false;
                    }
                }
            }
            else
            {
                _offsetTick += Time.deltaTime;
            }
        }
        else
        {
            if(_tick > 0)
            {
                _tick -= Time.deltaTime;
                var value = curve.Evaluate(_tick / period);
                var alpha = value < 0 ? 0 : value;
                var col = _renderer.material.color;
                _renderer.material.color = new Color(col.r, col.g, col.b, alpha);

                _offsetTick = 0;
            }
        }

	}

    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
