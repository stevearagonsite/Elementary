using UnityEngine;
using System.Collections;

public class ElectricityVFX : IHandEffect
{
    private ElectricParticleEmitter emitter;

    public bool IsPlaying()
    {
        return emitter.IsPlaying();
    }

    public void StartEffect()
    {
        
    }

    public void StartEjectEffect()
    {
        emitter.StartEffect();
    }

    public void StopEffect()
    {
    }

    public void TerminateEffect()
    {
        
    }
}
