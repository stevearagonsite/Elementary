using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Skills;
using System;

public class SkilslUI : MonoBehaviour
{
    public Sprite windTexture;
    public Sprite fireTexture;
    public Sprite electricTexture;

    public Animator anim;

    public Image inSkill;
    public Image outSkill;

    private Skills.Skills _actualSkill;
    private SkillController _skillController;

    private Dictionary<Skills.Skills, Sprite> _texturDict;

    private void Start()
    {
        _texturDict = new Dictionary<Skills.Skills, Sprite>();
        _texturDict.Add(Skills.Skills.VACCUM, windTexture);
        _texturDict.Add(Skills.Skills.FIRE, fireTexture);
        _texturDict.Add(Skills.Skills.ELECTRICITY, electricTexture);

        InputManager.instance.AddAction(InputType.Skill_Up, ChangeSkill);
        InputManager.instance.AddAction(InputType.Skill_Down, ChangeSkill);
    }

    private void ChangeSkill()
    {
        StartCoroutine(ChangeSkillLate());
    }

    private IEnumerator ChangeSkillLate()
    {
        yield return new WaitForEndOfFrame();
        if (_skillController == null)
        {
            var playerGO = GameObject.Find("Character");
            if (playerGO != null)
            {
                _skillController = playerGO.GetComponent<SkillController>();
            }
        }
        outSkill.sprite = _texturDict[_actualSkill];
        inSkill.sprite = _texturDict[_skillController.skillAction];
        _actualSkill = _skillController.skillAction;

        anim.Play("transition");
    }
}
