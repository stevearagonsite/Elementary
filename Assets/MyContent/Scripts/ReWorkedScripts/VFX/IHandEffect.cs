using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandEffect
{
    void StopEffect();
    void StartEffect();
    void TerminateEffect();
    bool IsPlaying();

}
