using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatTransfer : MonoBehaviour
{

    IFlamableObjects fo;

	void Start ()
    {
        fo = GetComponent<IFlamableObjects>();
	}

    private void OnCollisionStay(Collision collision)
    {
        if (fo.isOnFire)
        {
            var otherFo = collision.gameObject.GetComponent<IFlamableObjects>();
            if(otherFo != null)
            {
                otherFo.SetOnFire();
            }
        }
    }
}
