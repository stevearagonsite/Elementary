using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SprintUIFill : MonoBehaviour
{
    TPPController _player;
    public Image sprintFill;

    private bool _isUpdating;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        var playerGO = GameObject.Find("Character");
        if(playerGO != null)
            _player = playerGO.GetComponent<TPPController>();

        InputManager.instance.AddAction(InputType.Sprint, UpdateSprintUI);
    }

    private void UpdateSprintUI()
    {
        
        if(_player == null)
        {
            var playerGO = GameObject.Find("Character");
            if (playerGO != null)
            {
                _player = playerGO.GetComponent<TPPController>();
            }
        }
        sprintFill.fillAmount = _player.staminaPercent;
        if (!_isUpdating)
        {
            _isUpdating = true;
            anim.Play("FadeIn");
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, UpdateSprintDecrease);
        }
    }

    private void UpdateSprintDecrease()
    {
        if (_player.staminaPercent < 1)
        {
            sprintFill.fillAmount = _player.staminaPercent;
        }
        else
        {
            anim.Play("FadeOut");
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, UpdateSprintDecrease);
            _isUpdating = false;
        }
    }


    
}
