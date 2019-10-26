using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;

public class CraneBehaviour : MonoBehaviour {

    public bool isActive;
    bool attract;
    public Transform craneHead;
    public List<CraneObject> objectsToInteract;
    Animator _anim;
    CraneCollder craneCollider;
    Vector3 craneColliderInitialPosition;


    void Start ()
    {
        objectsToInteract = new List<CraneObject>();
        craneCollider = GetComponentInChildren<CraneCollder>();
        craneColliderInitialPosition = craneCollider.transform.position;
        _anim = GetComponent<Animator>();
        if (isActive)
        {
            StartCrane();
        }
    }

    void Execute()
    {
        if(objectsToInteract.Count > 0)
        {
            _anim.SetBool("Action", true);
           
        }
        else
        {
            _anim.SetBool("Action", false);
        }

        if (attract)
        {
            if(objectsToInteract.Count > 0)
                objectsToInteract[0].transform.position = craneHead.position;
            craneCollider.transform.position = craneHead.position;
        }
        else
        {
            craneCollider.transform.position = craneColliderInitialPosition;
        }
       
    }


    public void Catch()
    {
        attract = true;
    }

    public void Release()
    {
        attract = false;
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    public void StartCrane()
    {
        _anim.SetTrigger("PowerON");
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    public void ShutDownCrane()
    {
        _anim.SetTrigger("PowerOFF");
        _anim.SetBool("Action", false);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        isActive = false;
    }

}
