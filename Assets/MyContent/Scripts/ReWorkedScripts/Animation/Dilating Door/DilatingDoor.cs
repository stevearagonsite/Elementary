using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DilatingDoor : MonoBehaviour {

    public bool isActive;
    Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        UpdatesManager.instance.AddUpdate(UpdateType.LATE, Execute);
    }
    
    void Execute()
    {
        if (_anim.GetBool("open"))
        {
            _anim.SetBool("open", false);
        }
    }

    public void ActivateDoor()
    {
        isActive = true;
    }

    public void DeactivateDoor()
    {
        isActive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 9 && isActive)
            _anim.SetBool("open", true);
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.LATE, Execute);
    }
}
