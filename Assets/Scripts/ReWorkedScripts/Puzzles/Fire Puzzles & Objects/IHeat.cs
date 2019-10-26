using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeat
{
    Transform Transform { get; }
    float temperature { get; }

    void Hit(float damage);
}
