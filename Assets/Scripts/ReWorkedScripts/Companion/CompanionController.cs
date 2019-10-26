using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour {

    public AnimationCurve curve;
    public float amplitute;
    public float frecMultiplier;
    public Vector3 locomotions;
    Vector3 helixLocomotions;

    public Transform[] helixes;

    Animator _anim;

	void Start ()
    {
        _anim = GetComponentInChildren<Animator>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	void Execute ()
    {
        locomotions.x = Mathf.Clamp(locomotions.x, -1,1);
        locomotions.y = Mathf.Clamp(locomotions.y, -1, 1);
        locomotions.z = Mathf.Clamp(locomotions.z, -1, 1);
        _anim.SetFloat("Forward", locomotions.y);
        _anim.SetFloat("Right", locomotions.x);
        _anim.SetFloat("Up", locomotions.z);
        transform.position += transform.up * curve.Evaluate(Time.time/ frecMultiplier) * amplitute;

        helixLocomotions = new Vector3(0, locomotions.y, -0.1f);

        for (int i = 0; i < helixes.Length; i++)
        {
            helixes[i].up = helixLocomotions;
        }

	}

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
