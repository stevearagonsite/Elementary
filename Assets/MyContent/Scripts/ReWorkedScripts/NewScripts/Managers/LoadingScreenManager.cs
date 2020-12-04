using UnityEngine;
using System.Collections;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    private LoadingScreenManager _instance;
    public LoadingScreenManager instance { get { return _instance;}}

    private int amountOfLoadingScreens;

    public Canvas loadingCanvas;

    void Awake()
    {
        if(_instance == null) _instance = this;
    }

    private void Start()
    {
        EventManager.AddEventListener(GameEvent.START_LOAD_SCENE, onStartLoadScene);
        EventManager.AddEventListener(GameEvent.LOAD_SCENE_COMPLETE, onCompleteLoadScene);
        loadingCanvas.gameObject.SetActive(false);
    }

    private void onCompleteLoadScene(object[] parameterContainer)
    {
        if(amountOfLoadingScreens > 0)
        {
            amountOfLoadingScreens--;
        }
        Debug.Log("Complete load, amountOfScenesInQueue: " + amountOfLoadingScreens); 
        if(amountOfLoadingScreens == 0)
        {
            loadingCanvas.gameObject.SetActive(false);
            EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_DEMO);
            Debug.Log("End Scenes Queue");
            //TODO: Que sea transicion y no de golpe
        }
    }

    private void onStartLoadScene(object[] parameterContainer)
    {
        amountOfLoadingScreens++;
        if (!loadingCanvas.gameObject.activeSelf)
        {
            loadingCanvas.gameObject.SetActive(true);
            //TODO: Que sea transicion y no de golpe
        }
    }
}
