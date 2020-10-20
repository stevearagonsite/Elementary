using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    public Animator anim;
    public Transform obj;
    public Transform lookObj;
    public bool ikActive;

    public AvatarIKGoal avatarIKGoal;

    private void OnAnimatorIK()
    {
        Debug.Log("IK");
        if (anim) 
        {
            if (ikActive) 
            {
                if(lookObj != null) 
                {
                    anim.SetLookAtWeight(1);
                    anim.SetLookAtPosition(lookObj.position);
                }

                if(obj != null) 
                {
                    anim.SetIKPositionWeight(avatarIKGoal, 1);
                    anim.SetIKRotationWeight(avatarIKGoal, 1);
                    anim.SetIKPosition(avatarIKGoal, obj.position);
                    anim.SetIKRotation(avatarIKGoal, obj.rotation);
                }
            }
            else 
            {
                anim.SetIKPositionWeight(avatarIKGoal, 0);
                anim.SetIKRotationWeight(avatarIKGoal, 0);
                anim.SetLookAtWeight(0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
