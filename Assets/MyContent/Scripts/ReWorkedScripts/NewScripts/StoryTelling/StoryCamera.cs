using UnityEngine;

public class StoryCamera : MonoBehaviour
{
    void Awake()
    {
        CameraManager.instance.RegisterCamera(GetComponent<Camera>());
    }

    private void OnDestroy()
    {
        CameraManager.instance.RemoveCameraFromRegister(GetComponent<Camera>());
    }

}
