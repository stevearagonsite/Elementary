using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePlayerForCinematic : MonoBehaviour
{
    public Transform targetPos;
    private Transform _player;
    private void OnTriggerEnter(Collider other)
    {
        if(_player == null)
        {
            _player = other.transform;
        }
        
    }
    public void CallPlacePlayer()
    {
        StartCoroutine(PlacePlayer());
    }

    private IEnumerator PlacePlayer()
    {
        yield return new WaitForSeconds(0.5f);
        _player.position = targetPos.position;
        _player.rotation = targetPos.rotation;
        Destroy(this);
    }
}
