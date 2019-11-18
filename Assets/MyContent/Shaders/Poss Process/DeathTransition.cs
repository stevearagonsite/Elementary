using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTransition : MonoBehaviour {

    public Material mat;
    public float distanceTolerance;
    public float timmerFade;
    [SerializeField]float tick;
    bool isActive;
    bool goForward;
    bool uniqueDispatch;

    Vector3 oldPos;

	void Start () {
        tick = 0;
        oldPos = transform.position;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        EventManager.AddEventListener(GameEvent.PLAYER_DIE, Activate);
	}

    void Execute()
    {
        if (isActive)
        {
            if (goForward)
            {
                if(tick >= timmerFade)
                {
                    if (!uniqueDispatch)
                    {
                        //EventManager.DispatchEvent(GameEvent.TRANSITION_DEATH_END);
                        uniqueDispatch = true;
                    }
                    CheckCameraIsSpleeping();
                }
                else
                {
                    tick += Time.deltaTime;
                    mat.SetFloat("_Lerp", tick / timmerFade);
                }
                
            }
            else
            {
                if(tick > 0)
                {
                    tick -= Time.deltaTime;
                    mat.SetFloat("_Lerp", tick);
                }
                else
                {
                    isActive = false;
                    uniqueDispatch = false;
                }
            }
            
        }
    }

    private void CheckCameraIsSpleeping()
    {
        goForward = !(Math.Abs((oldPos - transform.position).magnitude) < distanceTolerance);
        oldPos = transform.position;
    }

    public void Activate(params object[] pC)
    {
        isActive = true;
        goForward = true;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (isActive)
        {
            Graphics.Blit(src, dst, mat);
        }
        else
        {
            Graphics.Blit(src, dst);
        }
    }
}
