using UnityEngine;

public interface IVacuumObject
{
    bool isAbsorved { get; set; }
    bool isAbsorvable { get; }
    bool isBeeingAbsorved { get; set; }
    Rigidbody rb { get; set; }

    void SuckIn(Transform origin, float atractForce);
    void BlowUp(Transform origin, float atractForce, Vector3 direction);
    void ReachedVacuum();
    void Shoot(float shootForce, Vector3 direction);
    void ViewFX(bool active);
    void Exit();
}
