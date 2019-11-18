using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathCalculate : MonoBehaviour {

    //Line renderer to show Path
    public LineRenderer path;
    public GameObject crosshair;

    //Number of calculated segments
    public int segmentCount = 20;

    // Length scale for each segment
    private float _segmentScale;
    public float initialSegmentScale = 5;

    /*//Reference to arm rotator component (to get the throw angle)
    private Attractor _at;*/

    //Renderer color
    public Color col;

    //RigidBody array
    Rigidbody[] allRBs;
    Vector3[] allPositions;
    Quaternion[] allRotations;
    Vector3[] allVelocities;

    Transform originalParent;
    Vector3 originalPos;
    Quaternion originalRot;

    public Transform vacuumHoleTransform;
    public Vector3 segVelocity;
    public float farEnd;
    public float topAmplitude;
    private int _points;

    int armLayermask;
    int armLayer = 15;

    int vacuumLayerMask;
    const int vacuumInteractiveLayer = 8;
    const int vacuumLayer = 15;
    const int fogLayer = 17;

    private void Start()
    {
        armLayermask = ~((1 << armLayer)|(1<< fogLayer));
        vacuumLayerMask = ~((1 << vacuumInteractiveLayer)|(1<< vacuumLayer));
        path.positionCount = (segmentCount);
        DeactivatePath();
    }

    public void SimulatePath(Rigidbody rb)
    {
        ActivatePath();
        _segmentScale = initialSegmentScale *10 / rb.mass;
        Vector3[] segments = new Vector3[segmentCount];

        //First Point
        segments[0] = vacuumHoleTransform.position;

        //Calculo nuevo

        //Inicial velocity
        segVelocity = (Mathf.Sin(transform.localRotation.eulerAngles.z * Mathf.PI / 180) * transform.up * farEnd + Mathf.Cos(transform.localRotation.eulerAngles.z * Mathf.PI / 180) * transform.right * topAmplitude) / rb.mass;
        //segVelocity = vacuumHoleTransform.forward * 300/ rb.mass;

        for (int i = 1; i < segmentCount; i++)
        {
            float segTime = (segVelocity.sqrMagnitude != 0) ? _segmentScale / segVelocity.magnitude : 0;

            segVelocity = segVelocity + Physics.gravity * segTime;
            segments[i] = segments[i - 1] + segVelocity * segTime;
            var distance = Vector3.Distance(segments[i], segments[i - 1]);
            RaycastHit ray;
            _points = i;
            if (Physics.Raycast(segments[i - 1], segments[i], out ray, distance * 1.1f, vacuumLayerMask))
            {
                _points = i;
                i = segmentCount;

                //Rotation with hit normal VFX
                var targetRotation = ray.normal;
                var fromRotation = crosshair.transform.rotation;
                crosshair.transform.rotation = Quaternion.FromToRotation( fromRotation.eulerAngles, targetRotation );
            }
        }
        path.positionCount = (_points);
        path.SetPositions(segments);
        //VFX
        crosshair.transform.position = segments[_points];
        //crosshair.transform.localEulerAngles += new Vector3(0,3,0);
    }
    /*
    public void SimulatePath(Rigidbody rb, Vector3 force)
    {

        //Deactivate Physics AutoSimulation
        originalParent = rb.transform.parent;
        originalPos = rb.transform.position;
        originalRot = rb.transform.rotation;

        Physics.autoSimulation = false;
        rb.isKinematic = false;
        rb.transform.SetParent(null);

        //Initial rbs States
        allRBs = FindObjectsOfType<Rigidbody>().Where(r => !r.isKinematic).ToArray();
        allPositions = new Vector3[allRBs.Length];
        allRotations = new Quaternion[allRBs.Length];
        allVelocities = new Vector3[allRBs.Length];

        Debug.Log(allRBs.Length);
        for (int i = 0; i < allRBs.Length; i++)
        {
            allPositions[i] = allRBs[i].transform.position;
            allRotations[i] = allRBs[i].transform.rotation;
            allVelocities[i] = allRBs[i].velocity;
        }


        //Activate Line Renderer
        ActivatePath();

        //Segment array
        Vector3[] segments = new Vector3[segmentCount];

        //First Point
        segments[0] = vacuumHoleTransform.position;

        

        //Aply Velocity to RigidBody
        rb.velocity = force;

        for(int i = 1; i < segmentCount; i++)
        {
            Physics.Simulate(Time.fixedDeltaTime);
            segments[i] = rb.position;
        }

        //Set LineRenderer Points
        path.SetPositions(segments);


        //Reset RigidBodys to initial State
        for (int i = 0; i < allRBs.Length; i++)
        {
            allRBs[i].position = allPositions[i];
            allRBs[i].rotation = allRotations[i];
            allRBs[i].velocity = allVelocities[i];
        }

        rb.transform.position = originalPos;
        rb.transform.rotation = originalRot;
        rb.velocity = Vector3.zero;

        //Reactivate Physics Simulation
        rb.transform.SetParent(originalParent);
        rb.isKinematic = true;
        Physics.autoSimulation = true;
    }
    */
    public void SimulateStraightAim(Transform initialTransform)
    {
        ActivatePath();
        path.positionCount = 2;
        path.SetPosition(0, initialTransform.position);
        var raycastDistance = initialSegmentScale * segmentCount;
        RaycastHit ray;
        Vector3 secondPoint;
        if(Physics.Raycast(initialTransform.position, initialTransform.forward, out ray,raycastDistance, armLayermask))
        {
            secondPoint = initialTransform.position + initialTransform.forward * ray.distance;
            crosshair.transform.up = ray.normal;
        }
        else
        {
            secondPoint = initialTransform.position + initialTransform.forward * raycastDistance;
            crosshair.transform.up = -initialTransform.forward;
        }
        path.SetPosition(1, secondPoint);
        crosshair.transform.position = secondPoint;
    }

    public void ActivatePath()
    {
        path.enabled = true;
        crosshair.SetActive(true);
    }

    public void DeactivatePath()
    {
        path.enabled = false;
        crosshair.SetActive(false);
    }
	
	
}
