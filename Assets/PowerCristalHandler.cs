using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCristalHandler : MonoBehaviour
{
    private MeshRenderer _mesh;
    private Animator _anim;
    private AudioClipHandler audioClipHandler;
    public ParticleSystem explotion;
    public ParticleSystem magic;
    public ParticleSystem sparks;
    public Light light;

    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _anim = GetComponent<Animator>();
        audioClipHandler = GetComponent<AudioClipHandler>();
        EventManager.AddEventListener(GameEvent.GET_POWER_EVENT, OnGetPower);
        EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_VACUUM, OnSkillActivation);
        EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_FIRE, OnSkillActivation);
        EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_ELECTRIC, OnSkillActivation);
    }

    private void OnSkillActivation(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_VACUUM, OnSkillActivation);
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_FIRE, OnSkillActivation);
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_ELECTRIC, OnSkillActivation);
        _anim.Play("GetPower");
    }

    private void OnGetPower(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.GET_POWER_EVENT, OnGetPower);
        _mesh.enabled = false;
        _anim.enabled = false;
        explotion.Play();
        magic.Stop();
        sparks.Stop();
        audioClipHandler.Play();
        StartCoroutine(TurnLightOff());
    }

    private IEnumerator TurnLightOff()
    {
        var tick = 0f;
        while(tick < 1)
        {
            tick += Time.deltaTime;
            light.intensity = 1 - tick;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.GET_POWER_EVENT, OnGetPower);
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_VACUUM, OnSkillActivation);
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_FIRE, OnSkillActivation);
        EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_ELECTRIC, OnSkillActivation);
    }
}
