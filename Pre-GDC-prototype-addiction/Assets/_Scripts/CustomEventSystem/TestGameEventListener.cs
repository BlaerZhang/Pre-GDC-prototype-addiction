using UnityEngine;

namespace _Scripts.CustomEventSystem
{
    public class TestGameEventListener : GameEventListenerBase
    {
        public override void OnEventRaised()
        {
            Debug.Log("custom event triggered");
        }
    }
}