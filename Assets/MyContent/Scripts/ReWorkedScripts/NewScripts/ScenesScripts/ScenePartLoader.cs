using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePartLoader : MonoBehaviour
{
    private bool _isLoaded = false;

    private void LoadScene() 
    {
        if (!_isLoaded) 
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            _isLoaded = true;
        }
    }

    private void UnLoadScene() 
    {
        if (_isLoaded) 
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            _isLoaded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LoadScene();
        Debug.Log("Entro al trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        UnLoadScene();
        Debug.Log("Salgo del trigger");
    }
}
