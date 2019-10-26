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
    PlayerController player;
    SkillController skills;

    Animator blackOut;

    void Start()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        player = GetComponentInParent<PlayerController>();
        skills = GetComponentInParent<SkillController>();
        blackOut = GameObject.Find("BlackIn").GetComponent<Animator>();
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
        player.enabled = false;
        skills.enabled = false;
        blackOut.SetTrigger("FadeOutLose");

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
