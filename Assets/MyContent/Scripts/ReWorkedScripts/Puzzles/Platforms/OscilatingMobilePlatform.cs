using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OscilatingMobilePlatform : Platform {

    [Header("Move Constrains")]
    public bool x;
    public bool y;
    public bool z;

    [Header("Movement Speeds")]
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    [Header("Period")]
    public float period;

    float _tick;

    [Header("Animation Move Curve")]
    public AnimationCurve moveCurve;

	void Start ()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	void Execute ()
    {
        if (isActive)
        {
            _tick += Time.deltaTime;
            if(_tick > period)
            {
                _tick = 0;
            }
            var moveX = x ? moveCurve.Evaluate(_tick / period) * xSpeed : 0f;
            var moveY = y ? moveCurve.Evaluate(_tick / period) * ySpeed : 0f;
            var moveZ = z ? moveCurve.Evaluate(_tick / period) * zSpeed : 0f;
            transform.position += (transform.forward * moveX + transform.right * moveZ + transform.up * moveY) * Time.deltaTime;
        }

	}
    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
