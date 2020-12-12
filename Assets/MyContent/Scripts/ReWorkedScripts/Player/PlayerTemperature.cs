using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using Player;

public class PlayerTemperature : MonoBehaviour,IHeat
{
    [SerializeField]
    float _temperature;


    public float temperature { get { return _temperature; } }
    public Transform Transform { get { return transform; } }

    public float life;
    float _life;

    bool _setToDieByLaser;

    void Start()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        _life = life;
    }

    public void Restart()
    {
        _setToDieByLaser = false;
        _life = life;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute()
    {
        if (_setToDieByLaser)
        {
            Die();
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }

    private void Die()
    {
        Debug.Log("Deberias morir wey " +  this.name);
    }

    public void Hit(float damage)
    {
        if (!_setToDieByLaser)
        {
            _life -= damage * Time.deltaTime;
            if (_life < 0)
            {
                _setToDieByLaser = true;
            }
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
