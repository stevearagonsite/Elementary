using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class VacuumConeCollider : MonoBehaviour {

        SkillController _skill;

        void Start () {
            _skill = GetComponentInParent<SkillController>();
	    }

        #region Trigger Methods
        private void OnTriggerEnter(Collider c)
        {
            var obj = c.GetComponent<IVacuumObject>();
            if (obj != null)
                if (!_skill.objectsToInteract.Contains(obj))
                    _skill.objectsToInteract.Add(obj);
        }

        private void OnTriggerExit(Collider c)
        {
            var obj = c.GetComponent<IVacuumObject>();
            if(obj != null)
            {
                obj.rb.isKinematic = false;
                obj.isAbsorved = false;
                obj.Exit();
                _skill.objectsToInteract.Remove(obj);
            }
        }

        private void OnTriggerStay(Collider c)
        {
            var obj = c.GetComponent<IVacuumObject>();
            if (obj != null)
                if (!_skill.objectsToInteract.Contains(obj) && (GameInput.instance.blowUpButton))
                    _skill.objectsToInteract.Add(obj);
        }
        #endregion
    }

}
