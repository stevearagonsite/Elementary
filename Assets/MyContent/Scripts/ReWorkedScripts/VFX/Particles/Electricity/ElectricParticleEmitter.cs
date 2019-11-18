using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricParticleEmitter : MonoBehaviour, IHandEffect {

    public GameObject particlePrefab;
    public Transform start;
    public List<Transform> end;

    public int particleAmount;
    public float particleTimmer;
    public float timmerDispersion;

    LineRenderer line;
    Vector3[] linePositions;
    [Header("Line Attributes")]
    public float lineDispersion;

    [Header("Particle Attributes")]
    public float timmer;
    public float dispersion;
    public float rayWidth;
    public Color rayStartColor;
    public Color rayEndColor;

    [Header("Particle Details")]
    public ParticleSystem particleBall;
    public ParticleSystem sparks;

    float _randomParticleTimmer;
    float _tick;

    public bool _isPlaying;

    public void Initialize(List<Transform> ends)
    {
        end = ends;
        particleBall.gameObject.SetActive(false);
    }

    public void Initialize(Transform end)
    {
        this.end = new List<Transform>();
        this.end.Add(end);
    }

    void Execute()
    {
		if(_tick > _randomParticleTimmer)
        {
            foreach (var tr in end)
            {
                for (int i = 0; i < particleAmount; i++)
                {
                    _tick = 0;
                    _randomParticleTimmer = particleTimmer + UnityEngine.Random.Range(-timmerDispersion, timmerDispersion);
                    var pgo = Instantiate(particlePrefab, start.position, start.rotation,transform);
                    pgo.GetComponent<ElectricParticle>().Initialize(start, tr, timmer, dispersion , rayWidth, rayStartColor, rayEndColor, sparks);
                }
                SetLine(tr);  
            }
        }
        if(linePositions != null)
            MoveLinePoints();
        _tick += Time.deltaTime;

        //line.enabled = end.Count != 0;

	}

    public void SetLine(Transform tr)
    {
        var distance = Vector3.Distance(start.position, tr.position);
        var dir = (tr.position - start.position).normalized;
        var points = UnityEngine.Random.Range(5, 10);
        var increment = distance / points;
        linePositions = new Vector3[points];

        start.forward = dir;
        linePositions[0] = start.position;

        for (int i = 1; i < points - 1 ; i++)
        {
            linePositions[i] = start.position + (dir * increment * i);
            linePositions[i] += start.up * UnityEngine.Random.Range(-lineDispersion, lineDispersion) + start.right * UnityEngine.Random.Range(-lineDispersion, lineDispersion);
        }
        linePositions[points - 1] = tr.position;
        line.positionCount = points;
        line.SetPositions(linePositions);
    }

    void MoveLinePoints()
    {
        var count = line.positionCount;
        for (int i = 0; i < count - 1; i++)
        {
            var pos = line.GetPosition(i);
            var lerpPos = Vector3.Lerp(pos, linePositions[i], 0.5f);
            line.SetPosition(i,lerpPos);
        }
        line.SetPosition(count - 1, end[0].position);
    }

    #region IHandEffect
    public void StopEffect()
    {
        _isPlaying = false;
        if(line != null)
            line.enabled = false;
        particleBall.gameObject.SetActive(false);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    public void StartEffect()
    {
        if (!_isPlaying)
        {
            if(line != null)
                line.enabled = true;
            _isPlaying = true;
            gameObject.SetActive(true);
            _randomParticleTimmer = particleTimmer + UnityEngine.Random.Range(-timmerDispersion, timmerDispersion);
            line = GetComponent<LineRenderer>();
            particleBall.gameObject.SetActive(true);
            particleBall.Play();
            sparks.Stop();
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
    }

    public void TerminateEffect()
    {
        StopEffect();
        gameObject.SetActive(false);
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }
    #endregion

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

}
