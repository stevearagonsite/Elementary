using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandEffect
{
    void StopEffect();
    void StartEffect();
    void StartEjectEffect();
    void TerminateEffect();
    bool IsPlaying();

}
