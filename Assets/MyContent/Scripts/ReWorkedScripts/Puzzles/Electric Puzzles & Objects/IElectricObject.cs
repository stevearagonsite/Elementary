using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IElectricObject  {

    Transform transform { get; }
    bool isElectrified { set; get; }
    void Electrify();
}
