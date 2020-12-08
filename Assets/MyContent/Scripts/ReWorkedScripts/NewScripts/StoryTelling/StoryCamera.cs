using UnityEngine;

public class StoryCamera : MonoBehaviour
{
    void Start()
    {
        CameraManager.instance.RegisterCamera(GetComponent<Camera>());
    }

    private void OnDestroy()
    {
        CameraManager.instance.RemoveCameraFromRegister(GetComponent<Camera>());
    }

}
