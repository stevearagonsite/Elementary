using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricParticle : MonoBehaviour {

    Transform _start;
    Transform _end;

    Vector3 _direction;
    float _distance;
    float _increment;
    public int pointCount;
    float _timmer;
    float _dispersion;
    float _tick;

    TrailRenderer trail;

    public Vector3[] positions;
    public int actualPosition;

    ParticleSystem _sparkles;

    public void Initialize(Transform start, Transform end, float timmer, float dispersion, float rayWidth, 
                           Color rayStartColor, Color rayEndColor, ParticleSystem sparkles)
    {
        trail = GetComponent<TrailRenderer>();
        trail.widthMultiplier = rayWidth;
        trail.startColor = rayStartColor;
        trail.endColor = rayEndColor;


        _sparkles = sparkles;
        _start = start;
        _end = end;

        _direction = (_end.position - _start.position).normalized;
        _distance = Vector3.Distance(_start.position, _end.position);
        pointCount = Random.Range(5, 10);
        _increment = _distance / pointCount;

        positions = new Vector3[pointCount];

        _timmer = timmer;
        _dispersion = dispersion;

        for (int i = 0; i < pointCount-1; i++)
        {
            positions[i] = _start.position + (_direction * _increment * i);
        }
        positions[pointCount - 1] = end.position;
        RandomizePoint();
        actualPosition = 0;
        transform.position = positions[0];


        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute()
    {
        if (actualPosition < pointCount)
        {
            if (_tick > _timmer)
            {
                _tick = 0;
                actualPosition++;
            }
            else
            {
                _tick += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, positions[actualPosition], 0.9f);
            }
        }
        else
        {
            Die();
        }
        

    }
    void RandomizePoint()
    {
        _start.forward = _direction;
        for (int i = 1; i < pointCount - 1; i++)
        {
            positions[i] += _start.up * Random.Range(-_dispersion, _dispersion) + _start.right * Random.Range(-_dispersion, _dispersion);
        }
    }

    void Die()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        _sparkles.gameObject.transform.position = transform.position;
        _sparkles.Stop();
        _sparkles.Play();
        Destroy(gameObject);
    }
}
