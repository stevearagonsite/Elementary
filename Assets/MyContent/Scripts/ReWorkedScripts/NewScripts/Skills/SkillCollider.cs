using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Skills;

public class SkillCollider : MonoBehaviour
{
    public float firstSphereRadius = 0.3f;
    public float secondSphereRadius = 0.6f;
    public float secondSherePosOffset;
    public Transform cam;
    public Skills.Skills skill;

    private SkillController skillController;

    void Start()
    {
        InputManager.instance.AddAction(InputType.Absorb, GetObjects);
        InputManager.instance.AddAction(InputType.Reject, GetObjects);
        InputManager.instance.AddAction(InputType.Stop, Stop);

        skillController = GetComponentInParent<SkillController>();
    }

    void GetObjects()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, firstSphereRadius);
        Collider[] col2 = Physics.OverlapSphere(transform.position + cam.forward * secondSherePosOffset, secondSphereRadius);
        skill = skillController.skillAction;

        switch (skill)
        {
            case Skills.Skills.VACCUM:
                var aux = col.Concat(col2)
                            .Where(x => x.GetComponent<IVacuumObject>() != null)
                            .Select(x => x.GetComponent<IVacuumObject>())
                            .ToList();
                skillController.objectsToInteract.Clear();
                foreach (var item in aux)
                {
                    skillController.objectsToInteract.Add(item);
                }
                break;
            case Skills.Skills.FIRE:
                var auxF = col.Concat(col2)
                        .Where(x => x.GetComponent<IFlamableObjects>() != null)
                        .Select(x => x.GetComponent<IFlamableObjects>())
                        .ToList();
                skillController.flamableObjectsToInteract.Clear();
                foreach (var item in auxF)
                {
                    skillController.flamableObjectsToInteract.Add(item);
                }

                break;
            case Skills.Skills.ELECTRICITY:
                var auxE = col.Concat(col2)
                            .Where(x => x.GetComponent<IElectricObject>() != null)
                            .Select(x => x.GetComponent<IElectricObject>())
                            .Select(x => x.transform)
                            .ToList();
                skillController.electricObjectsToInteract.Clear();
                foreach (var item in auxE)
                {
                    skillController.electricObjectsToInteract.Add(item);
                }
                break;
            case Skills.Skills.ICE:
                var auxI = col.Concat(col2)
                                .Where(x => x.GetComponent<IFrozenObject>() != null)
                                .Select(x => x.GetComponent<IFrozenObject>())
                                .ToList();
                skillController.frozenObjectsToInteract.Clear();
                foreach (var item in auxI)
                {
                    skillController.frozenObjectsToInteract.Add(item);
                }
                break;
            case Skills.Skills.WATER:
                /*skillController.waterObjectToInteract = col.Union(col2)
                                                       .Where(x => x.GetComponent<IWaterObject>() != null)
                                                       .Select(x => x.GetComponent<IWaterObject>())
                                                       .ToList();*/
                break;
            default:
                break;
        }
    }

    void Stop()
    {
        //skillController.frozenObjectsToInteract.Clear();
        //skillController.electricObjectsToInteract.Clear();
        //skillController.flamableObjectsToInteract.Clear();
        //skillController.objectsToInteract.Clear();
    }

    private void OnDestroy()
    {
        InputManager.instance.RemoveAction(InputType.Absorb, GetObjects);
        InputManager.instance.RemoveAction(InputType.Reject, GetObjects);
        InputManager.instance.RemoveAction(InputType.Stop, Stop);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, firstSphereRadius);
        Gizmos.DrawSphere(transform.position + cam.forward * secondSherePosOffset, secondSphereRadius);

    }
}
