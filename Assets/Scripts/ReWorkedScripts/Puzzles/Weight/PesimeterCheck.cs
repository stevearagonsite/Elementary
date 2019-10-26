using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PesimeterCheck : MonoBehaviour {

    bool isEmpty;
    public Weight w;

   	void Start ()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}

    void Execute()
    {
        if(!isEmpty)
        {
            isEmpty = true;
        }
        else
        {
            w.totalWeight = 0;
            w.RemoveAllObjectsToWeight();
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<ObjectToWeight>())
        {
            isEmpty = false;
        }
    }
}
