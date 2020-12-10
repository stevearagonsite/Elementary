using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalPuzzle : MonoBehaviour
{
    public PortalPuzzle tweenPortal;
    public Transform spawnDirection;

    [HideInInspector]
    public float spawnForce;

    private void OnTriggerEnter(Collider other)
    {
        other.isTrigger = true;
        var otherRB = other.GetComponent<Rigidbody>();
        other.transform.position = tweenPortal.spawnDirection.position;
     

        otherRB.velocity = tweenPortal.spawnDirection.forward * otherRB.velocity.magnitude;
    }

    private void OnTriggerExit(Collider other)
    {
        other.isTrigger = false;
    }

}
