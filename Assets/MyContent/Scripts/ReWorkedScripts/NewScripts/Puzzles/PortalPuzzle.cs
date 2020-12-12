using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalPuzzle : MonoBehaviour
{
    public PortalPuzzle tweenPortal;
    public Transform spawnDirection;
    public ParticleSystem touchParticle;

    [HideInInspector]
    public float spawnForce;

    private void Start()
    {
        if (touchParticle != null)
            touchParticle.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.isTrigger = true;
        var otherRB = other.GetComponent<Rigidbody>();
        other.transform.position = tweenPortal.spawnDirection.position;
     

        otherRB.velocity = tweenPortal.spawnDirection.forward * otherRB.velocity.magnitude;
        if (touchParticle != null)
            touchParticle.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        other.isTrigger = false;
    }

}
