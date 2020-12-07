using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricityManager : MonoBehaviour, IHandEffect
{
    private bool _isActive;

    public bool isActive { get => _isActive; set => _isActive = value; }

    List<Transform> _objectsToInteract;
    public Transform[] electricBeams;
    public Transform[] electricBeamTargets;

    private void Awake()
    {
        _objectsToInteract = new List<Transform>();
    }
    public void StartEffect(int index) 
    {
        var vfx = electricBeams[index].GetComponent<VisualEffect>();
        vfx.Play();
        electricBeams[index].gameObject.SetActive(true);
    }

    public void SetTargets(List<Transform> objectsToInteract)
    {
        var startEffect = false;
        if(objectsToInteract.Count == 0)
        {
            TerminateEffect();
            return;
        }
        if (_objectsToInteract.Count != objectsToInteract.Count) 
        {
            Debug.Log("renuevo");
            _objectsToInteract.Clear();
            for (int i = 0; i < objectsToInteract.Count; i++)
            {
                _objectsToInteract.Add(objectsToInteract[i]);
            }
            startEffect = true;
        }
        for (int i = 0; i < _objectsToInteract.Count; i++)
        {
            if(electricBeams.Length > i)
            {
                electricBeams[i].up = (transform.position - _objectsToInteract[i].transform.position).normalized;
                electricBeamTargets[i].position = _objectsToInteract[i].transform.position;
                Debug.Log("pos" + i);
                if (startEffect)
                {
                    StartEffect(i);
                    Debug.Log("start Effect " + i);
                }
            }
        }
    }

    public void StopEffect()
    {
        TerminateEffect();
    }

    public void StartEffect()
    {
        
    }

    public void StartEjectEffect()
    {
        _isActive = true;
    }

    public void TerminateEffect()
    {
        for (int i = 0; i < electricBeams.Length; i++)
        {
            var vfx = electricBeams[i].GetComponent<VisualEffect>();
            vfx.Stop();
            electricBeams[i].gameObject.SetActive(false);
        }
        _isActive = false;
        _objectsToInteract.Clear();
    }

    public bool IsPlaying()
    {
        return _isActive;
    }
}
