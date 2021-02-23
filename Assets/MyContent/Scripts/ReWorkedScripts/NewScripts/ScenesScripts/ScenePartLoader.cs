using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePartLoader : MonoBehaviour
{
    private bool _isLoaded = false;

    private void LoadScene() 
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).name == gameObject.name)
            {
                _isLoaded = true;
                break;
            }
        }
        if (!_isLoaded) 
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            SceneLoadManager.instance.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            _isLoaded = true;
        }
    }

    private void UnLoadScene() 
    {
        if (_isLoaded) 
        {
            SceneLoadManager.instance.UnloadSceneAsync(gameObject.name);
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
