using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDisk : MonoBehaviour {

    public Vector3 rotation;
    public ParticleSystem aura;
    Material mat;
    bool isDisolving;
    float disolveLerp = 0;


	void Start () {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        mat = GetComponent<MeshRenderer>().material;
	}
	
	void Execute () {
        transform.Rotate(rotation* Time.deltaTime);
        if (isDisolving)
        {
            disolveLerp += Time.deltaTime;
            mat.SetFloat("_Cutoff", disolveLerp);
            if(disolveLerp >= 1)
            {
                Destroy(gameObject);
                aura.Stop();
                HUDManager.instance.saveDisk.enabled = true;
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            isDisolving = true;
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
