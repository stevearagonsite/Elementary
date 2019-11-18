using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramObject : MonoBehaviour {

    Material _mat;
    BoxCollider _bc;

    const float MIN_OPACITY = 0.05f;
    const float MAX_OPACITY = 1;

    bool _isHeroInside;

    public float timeMultiplier;
    float _opacity;

    // Use this for initialization
    void Start()
    {
        _bc = GetComponent<BoxCollider>();
        _mat = GetComponent<Renderer>().material;

        _bc.isTrigger = false;
        _opacity = MIN_OPACITY;
        _mat.SetFloat("_Opacity", _opacity);
        gameObject.layer = 10;
    }


	public void ActivateCollider()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, ExecuteColliderActivation);
    }

    void ExecuteColliderActivation()
    {
        if (!_isHeroInside)
        {
            _bc.isTrigger = false;
            gameObject.layer = 8;
            _opacity += Time.deltaTime * timeMultiplier;
            _mat.SetFloat("_Opacity", _opacity);
            if (_opacity >= MAX_OPACITY)
            {
                UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, ExecuteColliderActivation);
            }
        }
    }

    public void DeactivateCollider()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, ExecuteColliderDeactivation);
    }

    void ExecuteColliderDeactivation()
    {
        _bc.isTrigger = true;
        gameObject.layer = 10;
        _opacity -= Time.deltaTime * timeMultiplier;
        _mat.SetFloat("_Opacity", _opacity);
        if (_opacity <= MIN_OPACITY)
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, ExecuteColliderDeactivation);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            _isHeroInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            _isHeroInside = false;
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, ExecuteColliderActivation);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, ExecuteColliderDeactivation);
    }
}
