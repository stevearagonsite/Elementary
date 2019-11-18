using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpMobile : MonoBehaviour {

    public StumpBase sBase;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 12)
        {
            if (sBase != null && sBase.isBurned)
            {
                anim.SetTrigger("Play");
            }else if(sBase == null)
            {
                anim.SetTrigger("Play");
            }

        }
        
    }
}
