using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadScreenManager : MonoBehaviour {

    AsyncOperation operation;
    float _progress;
    public float progress { get { return _progress; } }

    void Start ()
    {
        StartCoroutine(LoadSceneAsync(MasterManager.nextScene));
	}
	
	IEnumerator LoadSceneAsync(int sceneIndex)
    {
        operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            var prog = Mathf.Clamp01(operation.progress / 0.9f);
            _progress = prog;
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScreen");
    }


}
