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
                var aux= col.Union(col2)
                            .Where(x => x.GetComponent<IVacuumObject>() != null)
                            .Select(x => x.GetComponent<IVacuumObject>())
                            .ToList();
                skillController.objectsToInteract.Clear();
                foreach (var item in aux)
                {
                    skillController.objectsToInteract.Add(item);
                }
                Debug.Log("Collider amount: " + skillController.objectsToInteract.Count);
                break;
            case Skills.Skills.FIRE:
                skillController.flamableObjectsToInteract = col.Union(col2)
                                                       .Where(x => x.GetComponent<IFlamableObjects>() != null)
                                                       .Select(x => x.GetComponent<IFlamableObjects>())
                                                       .ToList();

                break;
            case Skills.Skills.ELECTRICITY:
                skillController.electricObjectsToInteract = col.Union(col2)
                                                       .Where(x => x.GetComponent<IElectricObject>() != null)
                                                       .Select(x => x.GetComponent<IElectricObject>())
                                                       .Select(x => x.transform)
                                                       .ToList();
                break;
            case Skills.Skills.ICE:
                skillController.frozenObjectsToInteract = col.Union(col2)
                                                       .Where(x => x.GetComponent<IFrozenObject>() != null)
                                                       .Select(x => x.GetComponent<IFrozenObject>())
                                                       .ToList();
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

    private void OnDestroy()
    {
        InputManager.instance.RemoveAction(InputType.Absorb, GetObjects);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, firstSphereRadius);
        Gizmos.DrawWireSphere(transform.position + cam.forward * secondSherePosOffset, secondSphereRadius);

    }
}
