using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{

    private static SceneLoadManager _instance;
    public static SceneLoadManager instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    /// <summary>
    /// load scene and dispatch event to notice it
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="mode"></param>
    public void LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
        var sceneToLoad = SceneManager.GetSceneByName(sceneName);
        if (!sceneToLoad.isLoaded)
        {
            var sceneLoadData = SceneManager.LoadSceneAsync(sceneName, mode);
            EventManager.DispatchEvent(GameEvent.START_LOAD_SCENE, sceneName);
            StartCoroutine(SceneLoadCoroutine(sceneLoadData));
        }
    }

    /// <summary>
    /// Controlls when the scene finish its load
    /// </summary>
    /// <param name="sceneLoadData"></param>
    /// <returns></returns>
    IEnumerator SceneLoadCoroutine(AsyncOperation sceneLoadData)
    {
        while (sceneLoadData.progress < 0.9)
        {
            yield return null;
        }
        EventManager.DispatchEvent(GameEvent.LOAD_SCENE_COMPLETE);
    }

    /// <summary>
    /// Unload scene, to have scene managment behaviour encapsulated
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnloadSceneAsync(string sceneName)
    {
        var sceneToLoad = SceneManager.GetSceneByName(sceneName);
        if (sceneToLoad.isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    
}
