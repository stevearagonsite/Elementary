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
        int fireMaxAmount = 300;
        int electricityMaxAmount = 300;
        int waterMaxAmount = 300;
        int iceMaxAmount = 300;
        [Header("Initial skill values")]
        public float initialFlameAmount;
        public float initialElectricityAmount;
        public float initialWaterAmount;
        public float initialIceAmount;
        float initialVacuumAmount = 1;

        //Debuging purpose only
        public float fireAmount;
        public float electricityAmount;
        public float waterAmount;
        public float iceAmount;

        SkillController _skillController;

        void Awake()
        {
            if (_instance == null)
                _instance = this;

            _skillAmount = new Dictionary<Skills, float>();
            _skillAmount[Skills.VACCUM] = initialVacuumAmount;
            _skillAmount[Skills.FIRE] = initialFlameAmount;
            _skillAmount[Skills.ELECTRICITY] = initialElectricityAmount;
            _skillAmount[Skills.WATER] = initialWaterAmount;
            _skillAmount[Skills.ICE] = initialIceAmount;

            //For HUD
            _maxSkillAmount = new Dictionary<Skills, float>();
            _maxSkillAmount[Skills.VACCUM] = 1;
            _maxSkillAmount[Skills.FIRE] = fireMaxAmount;
            _maxSkillAmount[Skills.ELECTRICITY] = electricityMaxAmount;
            _maxSkillAmount[Skills.WATER] = waterMaxAmount;
            _maxSkillAmount[Skills.ICE] = iceMaxAmount;


            _skillController = GetComponent<SkillController>();
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
        }

        public void RemoveAmountToSkill(float amount, Skills sk)
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
        }

        public float SkillActualAmount(Skills sk)
        {
            return _skillAmount[sk]/_maxSkillAmount[sk];
        }
    }


}


