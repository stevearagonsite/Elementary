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
    STORY_START,
    STORY_NEXT,
    STORY_END,
    TIME_UP,
    ELEVATOR_ON,
    KEY_TAKE,
    KEY_GIVE,
    GET_KEY_EVENT,
    VACUUM_STUCK,
    VACUUM_FREE,
    START_LOAD_SCENE,
    LOAD_SCENE_COMPLETE,
    START_LEVEL_TRANSITION,
    START_GAME,
    SKILL_ACTIVATE_VACUUM,
    SKILL_ACTIVATE_FIRE,
    SKILL_ACTIVATE_ELECTRIC,
    SKILL_DEACTIVATE_FIRE,
    SKILL_DEACTIVATE_ELECTRIC,
    SKILL_DEACTIVATE,
    TRIGGER_TUTORIAL,
    TRIGGER_TUTORIAL_STOP,
    DOUBLE_STORM_PLAY,
    DOUBLE_STORM_STOP,
    SPAWN_RINGS
}
