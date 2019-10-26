using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    public delegate void EventReciever(params object[] parameterContainer);
    private static Dictionary<GameEvent, EventReciever> _events;

    public static void AddEventListener(GameEvent eT, EventReciever listener)
    {
        if(_events == null)
        {
            _events = new Dictionary<GameEvent, EventReciever>();
        }
        if(!_events.ContainsKey(eT))
        {
            _events.Add(eT, null);
        }
        _events[eT] += listener;
    }

    public static void RemoveEventListener(GameEvent eT, EventReciever listener)
    {
        if(_events != null)
        {
            if(_events.ContainsKey(eT))
            {
                _events[eT] -= listener;
            }
        }
    }

    public static void DispatchEvent(GameEvent eT)
    {
        DispatchEvent(eT, null);
    }

    public static void DispatchEvent(GameEvent eT, params object[] paramsWrapper)
    {
        if(_events == null)
        {
            Debug.Log("No events suscribed" + "Event: " + eT.ToString());
            return;
        }
        if(_events.ContainsKey(eT))
        {
            if (_events[eT] != null)
                _events[eT](paramsWrapper);
        }
    }
}

public enum GameEvent
{
    Null,
    CAMERA_FIXPOS,
    CAMERA_NORMAL,
    CAMERA_STORY,
    CAMERA_FIXPOS_END,
    TRANSITION_FADEIN_FINISH,
    TRANSITION_FADEIN_DEMO,
    TRANSITION_FADEOUT_LOSE_FINISH,
    TRANSITION_FADEOUT_WIN_FINISH,
    TRANSITION_FADEOUT_DEMO,
    PLAYER_DIE,
    ON_SKILL_CHANGE,
    STORY_NEXT,
    STORY_END,
    TIME_UP,
    ELEVATOR_ON,
    SAVEDISK_ENTER,
    SAVEDISK_END
}
