using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour
{
    public string[] tutorialText;
    public GameObject graphicsObject;
    public Image[] tutorialGraphics;
    public Text tutorialTextField;
    public Animator _anim;

    private void Start()
    {
        EventManager.AddEventListener(GameEvent.TRIGGER_TUTORIAL, OnTutorial);
        EventManager.AddEventListener(GameEvent.TRIGGER_TUTORIAL_STOP, OnTutorialStop);
        graphicsObject.SetActive(false);
        TurnOffGraphics();
    }

    private void OnTutorial(object[] p)
    {
        var tutorialNumber = (int)p[0];
        graphicsObject.SetActive(true);
        TurnOffGraphics();
        tutorialGraphics[tutorialNumber].enabled = true;
        tutorialTextField.text = tutorialText[tutorialNumber];
        _anim.Play("FadeIn");
    }

    private void OnTutorialStop(object[] p)
    {
        _anim.Play("FadeOut");
    }

    private void TurnOffGraphics()
    {
        for (int i = 0; i < tutorialGraphics.Length; i++)
        {
            tutorialGraphics[i].enabled = false;
        }
    }
}
