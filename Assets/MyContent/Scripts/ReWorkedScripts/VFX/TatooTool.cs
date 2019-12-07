using UnityEngine;
using System.Collections.Generic;

namespace Skills
{
    public class TatooTool : MonoBehaviour
    {
        public Texture fireTatooTexture;
        public Texture windTatooTexture;
        public Texture electricTatooTexture;

        public Renderer[] tatoos;

        public float transitionSpeed = 1;

        Dictionary<Skills, Texture> tatooDictionary;

        Skills _actualSkill;
        Skills _targetSkill;

        bool _isFadingAway;
        float alpha;

        /// <summary>
        ///  Awake method, initialize dicionary and alpha value 
        /// </summary>
        void Awake()
        {
            tatooDictionary = new Dictionary<Skills, Texture>();
            tatooDictionary.Add(Skills.VACCUM, windTatooTexture);
            tatooDictionary.Add(Skills.FIRE, fireTatooTexture);
            tatooDictionary.Add(Skills.ELECTRICITY, electricTatooTexture);
            alpha = 1;
        }

        /// <summary>
        /// Set tatoo texture in material
        /// </summary>
        /// <param name="skill"></param>
        public void SetTatoo(Skills skill)
        {
            foreach (var r in tatoos)
            {
                r.material.SetTexture("_mainTexture", tatooDictionary[skill]);
            }
            _actualSkill = skill;
        }

        /// <summary>
        /// Swap tatoo to target skill
        /// </summary>
        /// <param name="skill"></param>
        public void SwapTatoos(Skills skill)
        {
            StopSwaping();
            _targetSkill = skill;
             _isFadingAway = true;
             UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Swaping);
            
        }

        /// <summary>
        /// Stop Transition
        /// </summary>
        public void StopSwaping()
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Swaping);
        }

        /// <summary>
        /// Transition from one tatoo to another
        /// </summary>
        public void Swaping()
        {
            if (_isFadingAway)
            {
                alpha -= Time.deltaTime * transitionSpeed;
                if(alpha <= 0)
                {
                    _isFadingAway = false;
                    SetTatoo(_targetSkill);
                    alpha = 0;
                    SetTatooFill(SkillManager.instance.SkillActualAmount(_targetSkill));
                }
            }
            else
            {
                alpha += Time.deltaTime * transitionSpeed;
                if (alpha >= 1)
                {
                    alpha = 1;
                }
            }
            foreach (var r in tatoos)
            {
                r.material.SetFloat("_alphaMultiplier", alpha);
            }
            if(alpha == 1)
            {
                StopSwaping();
            }
        }

        /// <summary>
        /// Set tatoos fill (for skill amount)
        /// </summary>
        /// <param name="value"></param>
        public void SetTatooFill(float value)
        {
            value = Mathf.Clamp01(value);
            foreach (var r in tatoos)
            {
                r.material.SetVector("_fill", new Vector2(0, value));
            }
        }

        /// <summary>
        /// Destroy 
        /// </summary>
        private void OnDestroy()
        {
            StopSwaping();
        }
        
    }
}
