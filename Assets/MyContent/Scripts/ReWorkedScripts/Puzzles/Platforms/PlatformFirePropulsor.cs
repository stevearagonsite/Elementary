using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFirePropulsor : MonoBehaviour, IFlamableObjects {

    public direction dir;

    bool _isOnFire;
    public bool isOnFire { get { return _isOnFire; } set { _isOnFire = value; } }

    public void SetOnFire()
    {
        _isOnFire = true;
    }

    public enum direction
    {
        X,
        X_NEGATIVE,
        Z,
        Z_NEGATIVE
    }
}
