using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.CustomEventSystem
{
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "CustomEventSystem/GameEvent", order = 0)]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListenerBase> listeners = new List<GameEventListenerBase>();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListenerBase listenerBase)
        {
            listeners.Add(listenerBase);
        }

        public void UnregisterListener(GameEventListenerBase listenerBase)
        {
            listeners.Remove(listenerBase);
        }
    }
}