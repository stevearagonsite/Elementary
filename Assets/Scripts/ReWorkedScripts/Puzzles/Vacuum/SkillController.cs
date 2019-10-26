using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Player;

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
        public Vector3 aspireOffset;

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
        IHandEffect electricityVFX;
        [HideInInspector]
        public List<Transform> electricObjectsToInteract;
        #endregion

        #region HudVariables
        //Dictionary<Skills, typeSkill> hudSkill;
        #endregion

        #region HandVFXRegion
        public Mesh attractorMesh;
        public Mesh electricMesh;
        public Mesh waterMesh;
        public Mesh iceMesh;
        public Mesh fireMesh;
        public SkinnedMeshRenderer hand;

        public GameObject attractorHand;
        public GameObject electricHand;
        public GameObject waterHand;
        public GameObject iceHand;
        public GameObject fireHand;

        public GameObject[] lefthandFingers;

        Dictionary<Skills, Mesh> _meshDic;
        Dictionary<Skills, GameObject> _gODic;
        #endregion

        #region  Visual Effect
        [Header("VFX References")]
        public ParticleSystem aspireParticle;
        public ParticleSystem blowParticle;
        public ParticleSystem fireParticle;
        public ParticleSystem waterParticle;
        public ParticleSystem iceParticle;

        public Transform vacuumHoleTransform;
        public Transform particleParent;
        #endregion

        public Skills currentSkill;
        PlayerController _pC;
        LandChecker _lC;

        void Awake()
        {
            //-0.032 , 0.998
            //hudSkill = new Dictionary<Skills, typeSkill>();
            //hudSkill.Add(Skills.VACCUM, typeSkill.BlowAndAspire);

            //skillAction = new Skills();
            _pC = GetComponent<PlayerController>();
            _lC = GetComponentInChildren<LandChecker>();

            //Lists Initializing
            objectsToInteract = new List<IVacuumObject>();
            flamableObjectsToInteract = new List<IFlamableObjects>();
            wetObjectsToInteract = new List<IWaterObject>();
            electricObjectsToInteract = new List<Transform>();
            frozenObjectsToInteract = new List<IFrozenObject>();

            //Hand VFX Initializing
            aspireVFX = new VacuumVFX(aspireParticle, particleParent, vacuumHoleTransform, aspireOffset);
            blowVFX = new VacuumVFX(blowParticle, particleParent, vacuumHoleTransform);
            fireVFX = new VacuumVFX(fireParticle, particleParent, vacuumHoleTransform);
            waterVFX = new VacuumVFX(waterParticle, particleParent, vacuumHoleTransform);
            iceVFX = new VacuumVFX(iceParticle, particleParent, vacuumHoleTransform);

            electricityVFX = GetComponentInChildren<ElectricParticleEmitter>();
            var aux = GetComponentInChildren<ElectricParticleEmitter>();
            aux.Initialize(electricObjectsToInteract);

            //Strategy Initializing
            _attractor = new Attractor(atractForce, shootSpeed, vacuumHoleTransform, aspireVFX, blowVFX, objectsToInteract);
            _flameThrower= new FlameThrower(fireVFX, flamableObjectsToInteract);
            _waterLauncher = new WaterLauncher(waterVFX, wetObjectsToInteract);
            _electricity = new Electricity(electricityVFX, electricObjectsToInteract);
            _freezer = new Freezer(iceVFX, frozenObjectsToInteract);

            _skills = new Dictionary<Skills, ISkill>();
            _skills.Add(Skills.VACCUM, _attractor);
            _skills.Add(Skills.FIRE, _flameThrower);
            _skills.Add(Skills.WATER, _waterLauncher);
            _skills.Add(Skills.ELECTRICITY, _electricity);
            _skills.Add(Skills.ICE, _freezer);

            _meshDic = new Dictionary<Skills, Mesh>();
            _meshDic.Add(Skills.VACCUM, attractorMesh);
            _meshDic.Add(Skills.ICE, iceMesh);
            _meshDic.Add(Skills.FIRE, fireMesh);
            _meshDic.Add(Skills.WATER, waterMesh);
            _meshDic.Add(Skills.ELECTRICITY, electricMesh);

            _gODic = new Dictionary<Skills, GameObject>();
            _gODic.Add(Skills.VACCUM, attractorHand);
            _gODic.Add(Skills.ICE, iceHand);
            _gODic.Add(Skills.FIRE, fireHand);
            _gODic.Add(Skills.WATER, waterHand);
            _gODic.Add(Skills.ELECTRICITY, electricHand);

            actualAction = _skills[skillAction];
            actualAction.Enter();
            SkillSet();
        }

        void Start()
        {
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
            EventManager.DispatchEvent(GameEvent.ON_SKILL_CHANGE, currentSkill);
        }

        void Execute()
        {
            if (GameInput.instance.skillUp)
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
            else if (GameInput.instance.skillDown)
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

            if (!(GameInput.instance.crouchButton || GameInput.instance.sprintButton) && !_pC.isSkillLocked && _lC.land)
            {
                actualAction.Execute();
            }
            else
            {
                actualAction.Exit();
            }
        }

        private void SkillSet()
        {
            _gODic[currentSkill].SetActive(false);
            _gODic[skillAction].SetActive(true);
            currentSkill = skillAction;
            actualAction.Exit();
            actualAction = _skills[skillAction];
            actualAction.Enter();
            
            EventManager.DispatchEvent(GameEvent.ON_SKILL_CHANGE, currentSkill);
            //ChangeHandMesh();

        }

        private void ChangeHandMesh()
        {
            hand.sharedMesh = _meshDic[skillAction];
            if(skillAction == Skills.VACCUM)
            {
                for (int i = 0; i < lefthandFingers.Length; i++)
                {
                    lefthandFingers[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < lefthandFingers.Length; i++)
                {
                    lefthandFingers[i].SetActive(false);
                }
            }
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
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
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

