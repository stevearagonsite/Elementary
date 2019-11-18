using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWaterObject {

    bool isWet{ get; set; }
    void WetThis();
}
