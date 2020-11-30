using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetClothColliders : MonoBehaviour
{
    private GameObject _character;
    private Cloth _cloth;
    // Start is called before the first frame update
    void Start()
    {
        _cloth = GetComponent<Cloth>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    // Update is called once per frame
    void Execute()
    {
       
        _character = GameObject.Find("Character");
        if(_character != null) 
        {
            _cloth.capsuleColliders = _character.GetComponentsInChildren<CapsuleCollider>();
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }
}
