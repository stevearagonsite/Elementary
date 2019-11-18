using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Skills;
using System;

public class HUDManager : MonoBehaviour {

    [Header("HUD Image References")]
    public Image firePower;
    public Image waterPower;
    public Image icePower;
    public Image windPower;
    public Image electricPower;
    public Image saveDisk;

    [Header("Hud Colors")]
    public Color fireColor;
    public Color waterColor;
    public Color iceColor;
    public Color windColor;
    public Color electricColor;

    [Header("Power Bar Reference")]
    public Image powerBar;
    public Image powerBarBack;
    public Text powerPercent;

    [Header("Skill Manager Reference")]
    public SkillManager skillMan;

    [Header("Animator References")]
    public Animator frameAnimator;
    public Animator timmerAnimator;

    [Header("Timmer Text Ref")]
    public Text timmerText;

    Dictionary<Skills.Skills, Image> imageDictionary;
    Dictionary<Skills.Skills, Color> colorDictionary;

    public Image currentImage;
    Skills.Skills currentSkill;

    SkillController _skillController;

    bool isInTransition;
    bool isGoingDown;
    float fill;
    float fillSign;

    [Header("Transition Speeds")]
    public float fillSpeed;
    public float colorChangeSpeed;

    //Singleton
    static HUDManager _instance;
    public static HUDManager instance { get { return _instance; } }

    void Awake()
    {
        if(instance == null)
        {
            _instance = this;
        }    
    }

    void Start()
    {
        imageDictionary = new Dictionary<Skills.Skills, Image>();
        imageDictionary.Add(Skills.Skills.ELECTRICITY, electricPower);
        imageDictionary.Add(Skills.Skills.FIRE, firePower);
        imageDictionary.Add(Skills.Skills.ICE, icePower);
        imageDictionary.Add(Skills.Skills.VACCUM, windPower);
        imageDictionary.Add(Skills.Skills.WATER, waterPower);

        colorDictionary = new Dictionary<Skills.Skills, Color>();
        colorDictionary.Add(Skills.Skills.ELECTRICITY, electricColor);
        colorDictionary.Add(Skills.Skills.FIRE, fireColor);
        colorDictionary.Add(Skills.Skills.ICE, iceColor);
        colorDictionary.Add(Skills.Skills.VACCUM, windColor);
        colorDictionary.Add(Skills.Skills.WATER, waterColor);

        saveDisk.enabled = false;
        fill = 1;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        EventManager.AddEventListener(GameEvent.ON_SKILL_CHANGE, ChangeCurrentSkill);
        _skillController = FindObjectOfType<SkillController>();

        currentSkill = _skillController.skillAction;
        ChangeImage();
        powerBar.color = colorDictionary[currentSkill];
        powerBarBack.color = colorDictionary[currentSkill];
        powerPercent.color = colorDictionary[currentSkill];

    }

    void ChangeCurrentSkill(object[] parameterContainer)
    {
        isInTransition = true;
        isGoingDown = true;
        currentSkill = (Skills.Skills)parameterContainer[0];
    }

    void Execute()
    {
        if (isInTransition)
        {
            RefreshPower();
            ChangeBarColor();
        }

        RefreshPowerBar();
    }

    void RefreshPowerBar()
    {
        var amount = skillMan.SkillActualAmount(currentSkill);
        powerPercent.text = (int)(amount * 100) + "%";
        powerBar.fillAmount = amount;
    }

    void ChangeBarColor()
    {
        var actualColor = Color.Lerp(powerBar.color, colorDictionary[currentSkill], colorChangeSpeed * Time.deltaTime);
        powerBar.color = actualColor;
        powerPercent.color = actualColor;
        if (currentSkill == Skills.Skills.VACCUM)
        {
            powerBarBack.color = actualColor;
        }
        else
        {
            powerBarBack.color = Color.Lerp(powerBarBack.color, Color.white, colorChangeSpeed * Time.deltaTime);
        }
    }

    void RefreshPower()
    {
        fillSign = isGoingDown ? -1 : 1;
        fill += fillSpeed * fillSign * Time.deltaTime;
        currentImage.fillAmount = fill;
        if(fill <= 0)
        {
            currentImage.fillAmount = 0;
            fill = 0;
            ChangeImage();
            isGoingDown = false;
        }
        if(fill > 1)
        {
            currentImage.fillAmount = 1;
            fill = 1;
            isInTransition = false;
        }
    }

    void ChangeImage()
    {
        currentImage.sprite = imageDictionary[currentSkill].sprite;
    }

    public void EnablePowerHUD()
    {
        frameAnimator.SetBool("IsOpen", true);
    }

    public void DisablePowerHUD()
    {
        frameAnimator.SetBool("IsOpen", false);
    }

    public void EnableTimmerHUD()
    {
        timmerAnimator.SetBool("IsOpen", true);
    }

    public void DisableTimmerHUD()
    {
        timmerAnimator.SetBool("IsOpen", false);
    }

    public void RefreshTimmerHUD(float timeLeft)
    {
        int mins = (int)timeLeft / 60;
        int secs = (int)timeLeft - mins * 60;

        timmerText.text = mins.ToString("00") + ":" + secs.ToString("00");
    }

    void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.ON_SKILL_CHANGE, ChangeCurrentSkill);
    }
}
