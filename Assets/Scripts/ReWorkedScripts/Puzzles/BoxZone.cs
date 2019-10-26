using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxZone : MonoBehaviour {

    List<MediumSizeObject> windObjCurrentList;
    List<MediumSizeObject> windObjList;

    int actualBox;
    bool respawning;

    private void Start()
    {
        windObjCurrentList = new List<MediumSizeObject>();
        windObjList = new List<MediumSizeObject>();
        actualBox = 0;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute()
    {
        if(windObjCurrentList.Count == 0 && !respawning)
        {
            windObjList[actualBox].SpawnVFXActivate(false);
            respawning = true;
            actualBox++;
            if (actualBox >= windObjList.Count - 1) actualBox = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var w = other.GetComponent<MediumSizeObject>();
        if(w)
        {
            if (!windObjCurrentList.Contains(w))
            {
                windObjCurrentList.Add(w);
                respawning = false;
            }
            if (!windObjList.Contains(w))
            {
                windObjList.Add(w);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        var w = other.GetComponent<MediumSizeObject>();
        if (w)
        {
            if (windObjCurrentList.Contains(w))
            {
                windObjCurrentList.Remove(w);
            }
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

}
