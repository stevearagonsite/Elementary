using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchOnFireForObjects : MonoBehaviour, IFlamableObjects {

    bool _isOnFire;
    
    public float maxLife;
    public float fireSensitivity;
    float currentLife;

    public ParticleSystem fireParticle;
    Renderer rend;

    private AudioClipHandler _clipHandler;

    public bool consumable;

    public bool isOnFire
    {
        get{ return _isOnFire; }
        set{ _isOnFire = value; }
    }

    public void SetOnFire()
    {
        if (_clipHandler != null && !_clipHandler.isPlaying)
            _clipHandler.PlayFadeIn(1);
        isOnFire = true;
        if(fireParticle != null)
            fireParticle.Play();
    }

    void Start()
    {
        _clipHandler = GetComponent<AudioClipHandler>();
        isOnFire = false;
        currentLife = maxLife;
        if (fireParticle != null)
            fireParticle.Stop();
        rend = GetComponent<Renderer>();
        foreach (var item in rend.materials)
        {
            item.SetFloat("_DisolveAmount", 0);
        }
        if(consumable)
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }
	
	void Execute ()
    {
        if (isOnFire)
        {
            if(currentLife > 0)
            {
                currentLife -= Time.deltaTime * fireSensitivity;
                FireEffect();
            }
            else
            {
                if (fireParticle != null)
                    fireParticle.Stop();
                Die();
            }
        }
	}

    void FireEffect()
    {
        //Just for burn effect
        var scale = currentLife / maxLife;
        foreach (var item in rend.materials)
        {
            item.SetFloat("_DisolveAmount", 1 - scale);
        } 
    }
    
    void Die()
    {

        fireParticle.transform.SetParent(null);
        fireParticle.Stop();
        Invoke("KillParticle", 2);
        if (_clipHandler != null)
            _clipHandler.StopFadeOut(0.5f);
        GetComponent<Collider>().isTrigger = true;
        Invoke("DestroyAll", 2f);
        
    }

    void DestroyAll()
    {
        Destroy(gameObject);
    }

    void KillParticle()
    {
        Destroy(fireParticle);
    }

    private void OnDestroy()
    {
        if(consumable)
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        

    }
}
