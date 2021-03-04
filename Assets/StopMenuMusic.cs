using UnityEngine;

public class StopMenuMusic : MonoBehaviour
{
    private AudioClipHandler _clipHandler;
    void Start()
    {
        _clipHandler = GetComponent<AudioClipHandler>();
        EventManager.AddEventListener(GameEvent.START_GAME, OnStartGame);
    }

    private void OnStartGame(object[] p)
    {
        _clipHandler.StopFadeOut(1);
    }
}
