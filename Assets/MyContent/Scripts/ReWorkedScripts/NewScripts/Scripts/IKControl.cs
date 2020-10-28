using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    public Animator anim;
    public Transform obj;
    public Transform lookObj;
    public bool ikActive;
    public float blend;
    public AvatarIKGoal avatarIKGoal;

    private float _force= 0;
    public float test;

    private void OnAnimatorIK()
    {
        if (anim) 
        {
            if (ikActive) 
            {
                _force += Time.deltaTime * blend;
            }
            else 
            {
                _force -= Time.deltaTime * blend;
            }
            _force = Mathf.Clamp01(_force);
            if (lookObj != null)
            {
                anim.SetLookAtWeight(_force);
                anim.SetLookAtPosition(lookObj.position);
            }

            if (obj != null)
            {
                anim.SetIKPositionWeight(avatarIKGoal, _force);
                anim.SetIKRotationWeight(avatarIKGoal, _force);
                anim.SetIKPosition(avatarIKGoal, obj.position);
                anim.SetIKRotation(avatarIKGoal, obj.rotation);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
