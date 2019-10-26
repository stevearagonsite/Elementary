using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Skills
{
    public class FireConeCollider : MonoBehaviour
    {

        SkillController _skill;
        // Use this for initialization
        void Start()
        {
            _skill = GetComponentInParent<SkillController>();
        }

        #region Trigger Methods
        private void OnTriggerEnter(Collider c)
        {
            var flameObj = c.GetComponent<IFlamableObjects>();
            if (flameObj != null)
                if (!_skill.flamableObjectsToInteract.Contains(flameObj))
                    _skill.flamableObjectsToInteract.Add(flameObj);
        }

        private void OnTriggerExit(Collider c)
        {
            var flameObj = c.GetComponent<IFlamableObjects>();
            if (flameObj != null)
            {
                _skill.flamableObjectsToInteract.Remove(flameObj);
            }
        }

        private void OnTriggerStay(Collider c)
        {
            var flameObj = c.GetComponent<IFlamableObjects>();
            if (flameObj != null)
                if (!_skill.flamableObjectsToInteract.Contains(flameObj) && (GameInput.instance.blowUpButton))
                    _skill.flamableObjectsToInteract.Add(flameObj);
        }
        #endregion
    }

}
