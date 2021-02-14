using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSoundCheck : MonoBehaviour
{
    private Dictionary<string, STEP_TYPE> type;
    private CharacterSounds _cS;
    public STEP_TYPE currentType;

    private void Start()
    {
        type = new Dictionary<string, STEP_TYPE>();
        type.Add("Sand", STEP_TYPE.SAND);
        type.Add("Water", STEP_TYPE.WATER);
        type.Add("Stone", STEP_TYPE.STONE);

        _cS = GetComponentInParent<CharacterSounds>();
    }

    public STEP_TYPE ForceGetcurrentType()
    {
        var spheres = Physics.OverlapSphere(transform.position, 0.5f);
        for (int i = 0; i < spheres.Length; i++)
        {
            if (type.ContainsKey(spheres[i].tag))
            {
                currentType = type[spheres[i].tag];
                break;
            }
        }
        return currentType;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (type.ContainsKey(other.tag))
        {
            currentType = type[other.tag];
            _cS.fssc = this;
        }
    }
}
