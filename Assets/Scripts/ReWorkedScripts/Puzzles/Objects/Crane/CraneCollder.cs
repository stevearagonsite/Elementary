using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneCollder : MonoBehaviour {

    CraneBehaviour _crane;

    void Start()
    {
        _crane = GetComponentInParent<CraneBehaviour>();
    }

    #region Trigger Methods
    private void OnTriggerEnter(Collider c)
    {
        var obj = c.GetComponent<CraneObject>();
        if (obj != null && obj.name != "CraneObjectToPickUp")
            if (!_crane.objectsToInteract.Contains(obj))
                _crane.objectsToInteract.Add(obj);
    }

    private void OnTriggerExit(Collider c)
    {
        var obj = c.GetComponent<CraneObject>();
        if (obj != null && obj.name != "CraneObjectToPickUp")
        {
            _crane.objectsToInteract.Remove(obj);
        }
    }

    private void OnTriggerStay(Collider c)
    {
        var obj = c.GetComponent<CraneObject>();
        if (obj != null && obj.name != "CraneObjectToPickUp")
            if (!_crane.objectsToInteract.Contains(obj))
                _crane.objectsToInteract.Add(obj);
    }
    #endregion
}
