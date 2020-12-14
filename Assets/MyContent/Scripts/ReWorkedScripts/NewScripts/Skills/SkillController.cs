using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Player;
using UnityEngine.VFX;

namespace Skills
{
    public class SkillController : MonoBehaviour
    {

        #region Strategy
        ISkill actualAction;
        Dictionary<Skills, ISkill> _skills;
        [Header("Set this for initial skill")]
        public Skills skillAction;

        // BulletShoot _bulletShoot;
        Attractor _attractor;
        FlameThrower _flameThrower;
        WaterLauncher _waterLauncher;
        Electricity _electricity;
        Freezer _freezer;

        public Attractor attractor { get { return _attractor; } }

        #endregion

        #region Atractor Variables
        //Atractor Variables
        public List<IVacuumObject> objectsToInteract;

        [Header("Attractor Variables")]
        public float atractForce;
        public float shootSpeed;
        public Transform vacuumHoleTransform;

        bool _isStuck;
        IHandEffect aspireVFX;
        IHandEffect blowVFX;

        #endregion

        #region FireVariables
        IHandEffect fireVFX;
        public List<IFlamableObjects> flamableObjectsToInteract;
        #endregion

        #region IceVariables
        IHandEffect iceVFX;
        public List<IFrozenObject> frozenObjectsToInteract;
        #endregion

        #region WaterVariables
        IHandEffect waterVFX;
        public List<IWaterObject> wetObjectsToInteract;
        #endregion

        #region ElectricityVariables
        ElectricityManager electricityVFX;
        [HideInInspector]
        public List<Transform> electricObjectsToInteract;
        #endregion

        #region HudVariables
        //Dictionary<Skills, typeSkill> hudSkill;
        #endregion

        private CharacterController _cc;

        private TPPController _tppC;

        public Skills currentSkill;

        TatooTool _tatooTool;

        void Awake()
        {
            //Component initializing
            _tatooTool = GetComponent<TatooTool>();
            _cc = GetComponent<CharacterController>();
            _tppC = GetComponent<TPPController>();
            //Lists Initializing
            objectsToInteract = new List<IVacuumObject>();
            flamableObjectsToInteract = new List<IFlamableObjects>();
            wetObjectsToInteract = new List<IWaterObject>();
            electricObjectsToInteract = new List<Transform>();
            frozenObjectsToInteract = new List<IFrozenObject>();

            electricityVFX = GetComponentInChildren<ElectricityManager>();
            var aux = GetComponentInChildren<ElectricParticleEmitter>();
            aux.Initialize(electricObjectsToInteract);

            //Strategy Initializing
            _attractor = new Attractor(atractForce, shootSpeed, vacuumHoleTransform, objectsToInteract);
            _flameThrower= new FlameThrower(flamableObjectsToInteract);
            _waterLauncher = new WaterLauncher(wetObjectsToInteract);
            _electricity = new Electricity(electricObjectsToInteract, electricityVFX);
            _freezer = new Freezer(frozenObjectsToInteract);

            _skills = new Dictionary<Skills, ISkill>();
            _skills.Add(Skills.VACCUM, _attractor);
            _skills.Add(Skills.FIRE, _flameThrower);
            _skills.Add(Skills.WATER, _waterLauncher);
            _skills.Add(Skills.ELECTRICITY, _electricity);
            _skills.Add(Skills.ICE, _freezer);

            actualAction = _skills[skillAction];
            actualAction.Enter();
            SkillSet();
        }

        void Start()
        {
            EventManager.DispatchEvent(GameEvent.ON_SKILL_CHANGE, currentSkill);
            InputManager.instance.AddAction(InputType.Skill_Down, OnSkillDown);
            InputManager.instance.AddAction(InputType.Skill_Up, OnSkillUp);
            InputManager.instance.AddAction(InputType.Absorb, OnAbsorb);
            InputManager.instance.AddAction(InputType.Reject, OnReject);
            InputManager.instance.AddAction(InputType.Stop, OnStop);
        }

        void OnAbsorb()
        {
            if (_cc.isGrounded && _tppC.isActive)
                actualAction.Absorb();
            else
                OnStop();
        }

        void OnReject()
        {
            if(_cc.isGrounded && _tppC.isActive)
                actualAction.Eject();
            else
                OnStop();
        }

        void OnStop()
        {
            actualAction.Exit();
        }

        private void SkillSet()
        {
            currentSkill = skillAction;
            actualAction.Exit();
            actualAction = _skills[skillAction];
            actualAction.Enter();
            
            EventManager.DispatchEvent(GameEvent.ON_SKILL_CHANGE, currentSkill);
            ChangeHandMesh();

        }

        private void ChangeHandMesh()
        {
            //_tatooTool.SwapTatoos(skillAction);
        }

        private void OnSkillDown()
        {
            if (skillAction > 0)
            {
                skillAction--;
                RecuCheckAmount(skillAction, false);
            }
            else
            {
                skillAction = Skills.LAST;
                skillAction--;
                RecuCheckAmount(skillAction, false);
            }

            SkillSet();
        }

        private void OnSkillUp()
        {
            if (skillAction + 1 != Skills.LAST)
            {
                skillAction++;
                RecuCheckAmount(skillAction, true);
                if (skillAction == Skills.LAST) skillAction = Skills.VACCUM;
            }
            else skillAction = Skills.VACCUM;

            SkillSet();
        }

        void RecuCheckAmount(Skills skill, bool sign)
        {
            if (skill != Skills.VACCUM && skill != Skills.LAST)
            {
                if (SkillManager.instance.CheckSkillAmount(skill))
                {
                    skillAction = skill;
                }
                else
                {
                    if (sign)
                    {
                        skillAction++;
                        RecuCheckAmount(skillAction, true);
                    }
                    else
                    {
                        skillAction--;
                        RecuCheckAmount(skillAction, false);
                    }
                }
            }
        }

        public void NoMoreSkillAmount()
        {
            skillAction = Skills.VACCUM;
            SkillSet();
        }

        private void OnDestroy()
        {
            InputManager.instance.RemoveAction(InputType.Skill_Down, OnSkillDown);
            InputManager.instance.RemoveAction(InputType.Skill_Up, OnSkillUp);
            InputManager.instance.RemoveAction(InputType.Absorb, OnAbsorb);
            InputManager.instance.RemoveAction(InputType.Reject, OnReject);
            InputManager.instance.RemoveAction(InputType.Stop, OnStop);
        }
    }

    public enum Skills
    {
        VACCUM,
        FIRE,
        WATER,
        ICE,
        ELECTRICITY,
        LAST
    }
}

