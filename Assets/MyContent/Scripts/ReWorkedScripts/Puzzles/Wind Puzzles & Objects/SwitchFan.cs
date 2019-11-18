using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchFan : MonoBehaviour {

	public AnimationCurve curve;
	public Vector3 rotation;
    public VacuumSwitch windSwitch;

	float _powerValue;
	float _time;

	void Start()
	{
		_time = 0;
		_powerValue = 0;
		UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        windSwitch.AddOnSwitchEvent(ReachMaxPower);
        windSwitch.AddOnSwitchIncreaseEvent(IncreasingPower);
        windSwitch.AddOnSwitchDecreaseEvent(DecreasingPower);
	}

	void Execute()
	{
		transform.Rotate(rotation * Time.deltaTime * _powerValue);
	}

	public void IncreasingPower()
	{
		if(_time<1) 
		{
			_time += Time.deltaTime*2;
		}
		_powerValue = curve.Evaluate(_time);
	}

	public void DecreasingPower()
	{
		if (_time > 0) {
			_time -= Time.deltaTime;
		}
		_powerValue = curve.Evaluate(_time);
	}

	public void ReversePower()
	{
		if (_time > 0) {
			_time -= Time.deltaTime;
		}
		_powerValue = curve.Evaluate(_time);
	}

	public void ReachMaxPower()
	{
        _powerValue = 1;
        _time = 1;
	}

    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);    
    }

}
