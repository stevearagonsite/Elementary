using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    [Range(-100,100)]
    public float rotationSpeed = 0f;

    [Range(0, 1)]
    public float x,y,z = 0f;

    private void Start() { UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute); }
    private void OnDestroy() { UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute); }


    void Execute()
    {
        transform.Rotate(new Vector3(x, y, z) * Time.deltaTime * this.rotationSpeed);
    }
}
