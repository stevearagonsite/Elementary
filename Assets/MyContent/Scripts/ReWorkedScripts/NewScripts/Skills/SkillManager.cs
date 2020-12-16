using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class SkillManager : MonoBehaviour
    { 

        private static SkillManager _instance;
        public static SkillManager instance { get { return _instance; } }

        Dictionary<Skills, float> _skillAmount;
        Dictionary<Skills, float> _maxSkillAmount;

        TatooTool _tatooTool;

        /*[Header("Initial skill values")]
        public float initialFlameAmount;
        public float initialElectricityAmount;
        public float initialWaterAmount;
        public float initialIceAmount;
        float initialVacuumAmount = 1;

        //Debuging purpose only
        public float fireAmount;
        public float electricityAmount;
        public float waterAmount;
        public float iceAmount;*/

        SkillController _skillController;

        public bool isActive;

        void Awake()
        {
            if (_instance == null)
                _instance = this;

            _tatooTool = GetComponent<TatooTool>();

            _skillAmount = new Dictionary<Skills, float>();
            _skillAmount[Skills.VACCUM] = 1;
            _skillAmount[Skills.FIRE] = 0;
            _skillAmount[Skills.ELECTRICITY] = 0;
            _skillAmount[Skills.WATER] = 0;
            _skillAmount[Skills.ICE] = 0;

            //For HUD
            _maxSkillAmount = new Dictionary<Skills, float>();
            _maxSkillAmount[Skills.VACCUM] = 1;
            _maxSkillAmount[Skills.FIRE] = 1;
            _maxSkillAmount[Skills.ELECTRICITY] = 1;
            _maxSkillAmount[Skills.WATER] = 1;
            _maxSkillAmount[Skills.ICE] = 1;


            _skillController = GetComponent<SkillController>();
        }

        private void Start()
        {
            EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_FIRE, ActivateFire);
            EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_ELECTRIC, ActivateElectric);

            EventManager.AddEventListener(GameEvent.SKILL_DEACTIVATE_FIRE, DeactivateFire);
            EventManager.AddEventListener(GameEvent.SKILL_DEACTIVATE_ELECTRIC, DeactivateElectric);

            EventManager.AddEventListener(GameEvent.SKILL_ACTIVATE_VACUUM, Activate);
            EventManager.AddEventListener(GameEvent.SKILL_DEACTIVATE, Deactivate);
        }

        private void Deactivate(object[] parameterContainer)
        {
            isActive = false;
        }
        private void Activate(object[] parameterContainer)
        {
            isActive = true;
        }

        private void ActivateElectric(object[] parameterContainer)
        {
            _skillAmount[Skills.ELECTRICITY] = 1;
        }

        private void ActivateFire(object[] parameterContainer)
        {
            _skillAmount[Skills.FIRE] = 1;
        }

        private void DeactivateElectric(object[] parameterContainer)
        {
            _skillAmount[Skills.ELECTRICITY] = 0;
        }

        private void DeactivateFire(object[] parameterContainer)
        {
            _skillAmount[Skills.FIRE] = 0;
        }

        public bool CheckSkillAmount(Skills sk)
        {
            if (_skillAmount == null)
            {
                _skillAmount = new Dictionary<Skills, float>();
            }
            return _skillAmount.ContainsKey(sk) && _skillAmount[sk] > 0;
        }

        public void AddAmountToSkill(float amount, Skills sk)
        {
            if (_skillAmount == null)
            {
                _skillAmount = new Dictionary<Skills, float>();
            }
            if (!_skillAmount.ContainsKey(sk))
            {
                _skillAmount.Add(sk, 0);
            }
            if (_skillAmount[sk] < _maxSkillAmount[sk])
            {
                _skillAmount[sk] += amount;
            }
            else
            {
                Debug.Log("Max Amount Reached: " + sk);
            }
            _tatooTool.SetTatooFill(_skillAmount[sk] / _maxSkillAmount[sk]);
        }

        //We don't run out of skill anymore
        /*public void RemoveAmountToSkill(float amount, Skills sk)
        {
            if (_skillAmount == null)
            {
                _skillAmount = new Dictionary<Skills, float>();
            }
            if (!_skillAmount.ContainsKey(sk) || _skillAmount[sk] <= 0)
            {
                Debug.Log("No " + sk + " remaining");
                _skillController.NoMoreSkillAmount();
            }
            else
            {
                _skillAmount[sk] -= amount;
            }
            //_tatooTool.SetTatooFill(_skillAmount[sk] / _maxSkillAmount[sk]);
        }*/

        public float SkillActualAmount(Skills sk)
        {
            return _skillAmount[sk]/_maxSkillAmount[sk];
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_FIRE, ActivateFire);
            EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_ELECTRIC, ActivateElectric);

            EventManager.RemoveEventListener(GameEvent.SKILL_DEACTIVATE_FIRE, DeactivateFire);
            EventManager.RemoveEventListener(GameEvent.SKILL_DEACTIVATE_ELECTRIC, DeactivateElectric);

            EventManager.RemoveEventListener(GameEvent.SKILL_ACTIVATE_VACUUM, Activate);
            EventManager.RemoveEventListener(GameEvent.SKILL_DEACTIVATE, Deactivate);
        }
    }


}


