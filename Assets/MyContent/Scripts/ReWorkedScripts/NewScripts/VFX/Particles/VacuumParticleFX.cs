using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumParticleFX : MonoBehaviour {

    public ParticleSystem[] ps;
    public Transform target;
    public float[] t;

    void Start()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute ()
    {
        for (int i = 0; i < ps.Length; i++)
        {
            var particles = new ParticleSystem.Particle[ps[i].particleCount];
            var count = ps[i].GetParticles(particles);

            for (int j = 0; j < count; j++)
            {
                particles[j].position = Vector3.Lerp(particles[j].position, target.position, t[i]);
            }

            ps[i].SetParticles(particles, count);
        }
	}

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
