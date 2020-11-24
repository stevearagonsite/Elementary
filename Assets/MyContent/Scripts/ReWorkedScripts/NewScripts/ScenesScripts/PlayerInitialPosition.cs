using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialPosition : MonoBehaviour
{
    GameObject _hero;
    // Start is called before the first frame update
    void Start()
    {
        _hero = GameObject.Find("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if(_hero == null) 
        {
            _hero = GameObject.Find("Character");
        }
        else 
        {
            _hero.transform.position = transform.position;
            _hero.transform.rotation = transform.rotation;
            Destroy(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
