using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(PlayerMoveOnCinematic))]
public class SwitchGuide : MonoBehaviour
{
    public float guideSpeed;
    public UnityEvent onReachGoal;

    private ParticleSystem[] _particles;
    private List<CinematicNode> _nodes;
    private int _actualNode;


    private void Start()
    {
        _particles = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].Stop();
        }
        _nodes = GetComponent<PlayerMoveOnCinematic>().nodes;

    }

    public void StartGuide()
    {
        _actualNode = 0;
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].Play();
        }
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    private void Execute()
    {
        if(Vector3.Distance(transform.position, _nodes[_actualNode].position) > _nodes[_actualNode].radius)
        {
            var dir = (_nodes[_actualNode].position - transform.position).normalized;
            transform.position += dir * guideSpeed * Time.deltaTime;
        }
        else
        {
            _actualNode++;
        }
        if(_actualNode == _nodes.Count)
        {
            onReachGoal.Invoke();
            Destroy(this);
        }

    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
