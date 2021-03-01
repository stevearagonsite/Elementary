using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Skills;

public class CheatLevelSelect : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-01", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-02", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-03", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-04", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-05", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-06", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            UnloadCurrentLevel();
            SceneLoadManager.instance.LoadSceneAsync("Level-07", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            GameObject.Find("Character").GetComponent<SkillManager>().isActive = true;
        }
    }

    private void UnloadCurrentLevel()
    {
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name.Contains("Level"))
            {
                SceneLoadManager.instance.UnloadSceneAsync(scene.name);
            }
        }
    }
}
