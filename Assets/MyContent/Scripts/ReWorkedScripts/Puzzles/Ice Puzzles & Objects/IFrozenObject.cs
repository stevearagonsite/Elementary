using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFrozenObject {

	bool isFrozen { get; set; }
    void Freeze();
}
