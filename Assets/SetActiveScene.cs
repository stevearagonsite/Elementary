using UnityEngine;
using UnityEngine.SceneManagement;

public class SetActiveScene : MonoBehaviour
{
    public string actualScene;
    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(actualScene));
        Destroy(this);
    }
   
}
