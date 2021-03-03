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
        if (!CheckPointManager.instance.isGoingToCheckPoint)
        {
            GameObjectiveManager.instance.ResetKeyHold();
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

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, GoToNextLevel);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1, 0, 1, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
#endif
}
