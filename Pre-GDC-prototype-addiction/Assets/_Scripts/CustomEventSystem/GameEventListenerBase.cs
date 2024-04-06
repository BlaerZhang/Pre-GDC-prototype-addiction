using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.CustomEventSystem
{
    /// <summary>
    /// attached to an object that will listen to a custom game event
    /// </summary>
    public abstract class GameEventListenerBase : MonoBehaviour
    {
        public GameEvent Event;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        // real implementation of the triggered event
        public virtual void OnEventRaised() {}
    }
}