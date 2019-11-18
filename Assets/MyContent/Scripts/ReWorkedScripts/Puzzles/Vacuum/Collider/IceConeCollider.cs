using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class IceConeCollider : MonoBehaviour {

        SkillController _skill;
	    void Start ()
        {
            _skill = GetComponentInParent<SkillController>();
	    }

        #region Trigger Methods
        private void OnTriggerEnter(Collider c)
        {
            var obj = c.GetComponent<IFrozenObject>();
            if (obj != null)
                if (!_skill.frozenObjectsToInteract.Contains(obj))
                    _skill.frozenObjectsToInteract.Add(obj);
        }

        private void OnTriggerExit(Collider c)
        {

            var obj = c.GetComponent<IFrozenObject>();
            if (obj != null)
            {
                _skill.frozenObjectsToInteract.Remove(obj);
            }
        }

        private void OnTriggerStay(Collider c)
        {

            var obj = c.GetComponent<IFrozenObject>();
            if (obj != null)
                if (!_skill.frozenObjectsToInteract.Contains(obj) && (GameInput.instance.blowUpButton))
                    _skill.frozenObjectsToInteract.Add(obj);
        }
        #endregion
    }

}
