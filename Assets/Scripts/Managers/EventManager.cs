using System;
using System.Collections.Generic;

public class GameEvent { }


public static class EventManager 
{
    static readonly Dictionary<Type, Action<GameEvent>> events = new Dictionary<Type, Action<GameEvent>>();

    static readonly Dictionary<Delegate, Action<GameEvent>> eventLookups = new Dictionary<Delegate, Action<GameEvent>>();

    public static void Broadcast(GameEvent evt) {
        if (events.TryGetValue(evt.GetType(), out var action)) {
            action.Invoke(evt);
        }
    } 

    public static void AddListener<T>(Action<T> evt) where T : GameEvent {
        if (!eventLookups.ContainsKey(evt)) {
            //instantiate a new game event action
            //event lookups basically keep track of the latest action added to a certain type of game event
            Action<GameEvent> newAct = (e) => evt((T) e);
            eventLookups[evt] = newAct;

            //if theres another action, basically combine delegates
            if (events.TryGetValue(typeof(T), out Action<GameEvent> tempAct)) {
                events[typeof(T)] = tempAct += newAct;
            } else {
                events[typeof(T)] = newAct;
            }
        }
    }

    public static void RemoveListener<T>(Action<T> evt) where T: GameEvent {
        if (eventLookups.TryGetValue(evt, out Action<GameEvent> newAct)) {
            if (events.TryGetValue(typeof(T), out var tempAction)) {
                tempAction -= newAct;
                if (tempAction == null)
                    events.Remove(typeof(T));
                else
                    events[typeof(T)] = tempAction;
            }

            eventLookups.Remove(evt);
        }
    }

    public static void Cleanup() {
        events.Clear();
        eventLookups.Clear();
    }
}
