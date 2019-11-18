using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitions : MonoBehaviour
{

    public void OnFadeInFinish()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_FINISH);
    }

    public void OnFadeOutWin()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEOUT_WIN_FINISH);
    }

    public void OnFadeOutLose()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEOUT_LOSE_FINISH);
    }

    #region DemoFades
    public void OnDemoFadeOut()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEOUT_DEMO);
    }

    public void OnDemoFadeIn()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_DEMO);
    }
    #endregion



}
