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
            Application.backgroundLoadingPriority = ThreadPriority.High;
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
        Debug.Log(other.name + "Entered");
    }

    private void OnTriggerExit(Collider other)
    {
        UnLoadScene();
    }
}
