using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using System;
using Player;

public class ArmAngle : MonoBehaviour {

    const float MAX_Y_ANGLE = -40f;
    const float MIN_Y_ANGLE = -110f;

    float _currentY;
    Animator _anim;
    SkillController _skill;

    bool isActive;

    void Start ()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        _anim = GetComponent<Animator>();
        _skill = GetComponentInParent<SkillController>();
        EventManager.AddEventListener(GameEvent.CAMERA_STORY, ToStoryCam);
        EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, ToNormalCam);
        _currentY = GetComponentInParent<PlayerController>().cam2.normalState.currentY - 90;
	}

    private void ToNormalCam(object[] parameterContainer)
    {
        isActive = true;
    }

    private void ToStoryCam(object[] parameterContainer)
    {
        isActive = false;
    }

    void Execute ()
    {
        _currentY += GameInput.instance.cameraAngle;
        _currentY = Mathf.Clamp(_currentY, MIN_Y_ANGLE, MAX_Y_ANGLE);

        _anim.SetFloat("armAngle", _currentY);
        _anim.SetBool("isAbsorbing", (GameInput.instance.absorbButton && _skill.currentSkill == Skills.Skills.VACCUM) || GameInput.instance.blowUpButton);
    }

    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, ToStoryCam);
        EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, ToNormalCam);
    }
}
