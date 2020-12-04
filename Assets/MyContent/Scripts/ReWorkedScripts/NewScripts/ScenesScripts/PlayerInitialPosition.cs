using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialPosition : MonoBehaviour
{
    GameObject _hero;
    // Start is called before the first frame update
    void Start()
    {
        _hero = GameObject.Find("Character");
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_DEMO, RepositionPlayer);
    }

    void RepositionPlayer(object[] p)
    {
        if (_hero == null)
        {
            _hero = GameObject.Find("Character");
        }

        if(_hero != null)
        {
            _hero.transform.position = transform.position;
            _hero.transform.rotation = transform.rotation;
            EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_DEMO, RepositionPlayer);
            Destroy(this);
        }
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
