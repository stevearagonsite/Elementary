using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public string actualLevel;
    public string nextLevel;

    private void Start()
    {
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, GoToNextLevel);
    }

    public void GoToNextLevel(object[] p)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name.Contains(actualLevel))
            {
                SceneLoadManager.instance.UnloadSceneAsync(scene.name);
            }
        }
        SceneLoadManager.instance.LoadSceneAsync(nextLevel, LoadSceneMode.Additive);
    }
}
