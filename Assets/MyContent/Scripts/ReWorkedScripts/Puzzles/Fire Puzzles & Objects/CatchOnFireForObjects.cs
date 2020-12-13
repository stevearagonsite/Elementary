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

    public bool consumable;

    public bool isOnFire
    {
        get{ return _isOnFire; }
        set{ _isOnFire = value; }
    }

    public void SetOnFire()
    {
        isOnFire = true;
        fireParticle.Play();
    }

    void Start()
    {
        isOnFire = false;
        currentLife = maxLife;
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
